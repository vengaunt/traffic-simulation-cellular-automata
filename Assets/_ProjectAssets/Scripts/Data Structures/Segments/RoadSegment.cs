using System;
using System.Collections.Generic;
using System.Linq;
using KalkuzSystems;
using KalkuzSystems.Attributes;
using KalkuzSystems.DataStructures.Pooling;
using KalkuzSystems.Utility.Arrays;
using KalkuzSystems.Utility.Transform;
using PathCreation;
using TrafficSimulation.DataStructures.Grids;
using TrafficSimulation.Settings;
using TrafficSimulation.Utility;
using TrafficSimulation.Vehicles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Debugger = KalkuzSystems.Analysis.Debugger.Debugger;
using HeaderAttribute = KalkuzSystems.Attributes.HeaderAttribute;
using Random = UnityEngine.Random;

namespace TrafficSimulation.DataStructures.Segments
{
    public class RoadSegment : MonoBehaviour
    {
        [LineSeparator(1, 10), Header("References", headerAlignment: HeaderAlignment.CENTER, order = 1)] [SerializeField]
        private PathCreator pathCreator;
        
        [LineSeparator(1, 10), Header("Grid", headerAlignment: HeaderAlignment.CENTER, order = 1)]
        [SerializeField] private Grids.Grid grid;
        [SerializeField, Min(1)] private uint laneCount = 1;
        [SerializeField, Min(1)] private float cellUnitLength = 1;

        private Dictionary<Vehicle, Vector2> vehicles;
        private List<GridCell> dirtyCells;
        private List<GridCell> hotCells;

        public Dictionary<Vehicle, Vector2> Vehicles => vehicles;

        private void Awake()
        {
            if (pathCreator == null)
            {
                Debugger.Error("Path creator is null. Terminating the program.");
                AppHelper.Quit();
            }
            
            vehicles = new Dictionary<Vehicle, Vector2>();
            
            dirtyCells = new List<GridCell>();
            hotCells = new List<GridCell>();
            
            grid = new Grids.Grid(laneCount, (uint)Math.Ceiling(pathCreator.path.length / cellUnitLength));
        }

        #region Update Operations

        private void Update()
        {
            UpdateDirtyCells();
            UpdateHotCells();
            UpdateVehicles();
        }

        public void UpdateDirtyCells()
        {
            foreach (var dirtyCell in dirtyCells.ToList())
            {
                dirtyCell.occupation -= SimulationSettings.Instance.CellOccupancyDecreasingRate * Time.deltaTime;
                if (dirtyCell.occupation < 0f)
                {
                    dirtyCell.occupation = 0f;
                    dirtyCells.Remove(dirtyCell);
                }
            }
        }
        
        public void UpdateHotCells()
        {
            foreach (var hotCell in hotCells.ToList())
            {
                hotCell.heat -= SimulationSettings.Instance.CellHeatDecreasingRate * Time.deltaTime;
                if (hotCell.heat < 0f)
                {
                    hotCell.heat = 0f;
                    hotCells.Remove(hotCell);
                }
            }
        }

        public void UpdateVehicles()
        {
            var currentVehicles = vehicles.Keys.ToList();
            foreach (var vehicle in currentVehicles)
            {
                var gridIndices = vehicles[vehicle];

                // new indices in this update
                var newIndices = gridIndices + Vector2.up * vehicle.CurrentSpeed * Time.deltaTime / cellUnitLength;
                
                // lane details
                var baseLaneIndex = Mathf.FloorToInt(newIndices.x);
                var laneOverflow = newIndices.x % 1f;
                
                // distance details
                var baseCellIndex = Mathf.FloorToInt(newIndices.y + 0.5f);
                var occupiesBackCell = baseCellIndex - newIndices.y > 0f;
                var cellOverflow = newIndices.y % 1f;
                if (occupiesBackCell) cellOverflow = 1 - cellOverflow;
                
                // continue if next distance is out of bounds
                if (!grid.Cells.ContainsIndices(baseLaneIndex, baseCellIndex))
                {
                    vehicle.Disable();

                    vehicles.Remove(vehicle);
                    continue;
                }

                var baseCell = grid.Cells[baseLaneIndex, baseCellIndex];
                // register dirts
                if(!dirtyCells.Contains(baseCell)) dirtyCells.Add(baseCell);
                if(!hotCells.Contains(baseCell)) hotCells.Add(baseCell);
                
                // apply dirts
                baseCell.occupation = 1 - cellOverflow;
                baseCell.heat += SimulationSettings.Instance.CellHeatingFactor * Time.deltaTime * (1 - (vehicle.CurrentSpeed / vehicle.VehicleData.MaxSpeed));

                vehicle.DistanceOnRoad = newIndices.y * cellUnitLength; 
                // no need to update below instructions if this vehicle crashed
                if (vehicle.Crashed) continue;

                // adjust peeling distance
                var vehicleHalfLength = vehicle.VehicleData.Length * 0.5f;
                var vehicleHalfWidth = vehicle.VehicleData.Width * 0.5f;
                var brakingOffset = cellUnitLength + vehicleHalfLength;
                float peelingDistance = vehicle.PeelingDistance + brakingOffset;
                
                var cellsInFront = Mathf.Min(newIndices.y + peelingDistance / cellUnitLength, grid.Cells.GetLength(1));
                
                // find vehicles in front
                var obstacleVehicles = vehicles.Where(kvp =>
                {
                    var vLane = kvp.Value.x;
                    var vDistance = kvp.Value.y;
                    var vHalfLength = kvp.Key.VehicleData.Length * 0.5f;
                    var vHalfWidth = kvp.Key.VehicleData.Width * 0.5f;
                    var laneDist = Mathf.Abs(vLane - newIndices.x) * cellUnitLength;
                    
                    return (kvp.Key != vehicle) // ben degilsem
                           && (laneDist < Mathf.Max(cellUnitLength, vehicleHalfWidth + vHalfWidth)) // ikimizin lane i arası uzaklık, 
                           && (newIndices.y <= vDistance) // aracın arkasında isem
                           && (vDistance - (vHalfLength / cellUnitLength) < cellsInFront); // diger aracın poposu benim önümdeyse
                }).OrderBy(kvp => kvp.Value.y).ToList();
                
                // check if the vehicle is obstacling the road
                float occupiedRoadDistance = float.MaxValue;
                if (obstacleVehicles.Count > 0)
                {
                    var vehiclePair = obstacleVehicles[0];
                    var obstacleVehicle = vehiclePair.Key;
                    var obstacleDistance = vehiclePair.Value.y;
                    var obstacleHalfLength = obstacleVehicle.VehicleData.Length * 0.5f;
                    
                    brakingOffset += obstacleHalfLength;

                    // Check collision
                    if (Mathf.Abs((obstacleDistance - newIndices.y) * cellUnitLength) < obstacleHalfLength + vehicleHalfLength)
                    {
                        Debug.LogWarning("Vehicle Crashed!", vehicle);
                        Debug.LogWarning($"To: {obstacleVehicle}", obstacleVehicle);
                        
                        vehicle.Crash();
                        obstacleVehicle.Crash();
                        if (SimulationSettingsProvider.Settings.StopSimulationWhenAccidentHappens) Time.timeScale = 0f;
                    }
                    
                    occupiedRoadDistance = (obstacleDistance - (newIndices.y + 1)) * cellUnitLength;
                }

                if (Mathf.Abs(occupiedRoadDistance - float.MaxValue) > 0.001f)
                {
                    // onumuz dolu serit degistir
                    Debug.Log("Serit degistirmem lazım");
                    
                    // sol serite bak 
                    var leftObstacleVehicles = vehicles.Where(kvp =>
                    {
                        var vLane = kvp.Value.x;
                        var vDistance = kvp.Value.y;
                        var vHalfWidth = kvp.Key.VehicleData.Width * 0.5f;
                        var vHalfLength = kvp.Key.VehicleData.Length * 0.5f;

                        return (kvp.Key != vehicle)
                               && vLane < newIndices.x
                               && peelingDistance > (vDistance - newIndices.y) * cellUnitLength
                               && Mathf.Abs(newIndices.x - vLane) * cellUnitLength <= vHalfWidth + vehicleHalfWidth;
                    }).OrderByDescending(kvp => kvp.Value.x).ToList();

                    if (leftObstacleVehicles.Count > 0 || newIndices.x < 0.001f)
                    {
                        Debug.Log("sag seride bakıyom ha");
                        // sag serite bak
                        var rightObstacleVehicles = vehicles.Where(kvp =>
                        {
                            var vLane = kvp.Value.x;
                            var vDistance = kvp.Value.y;
                            var vHalfWidth = kvp.Key.VehicleData.Width * 0.5f;
                            var vHalfLength = kvp.Key.VehicleData.Length * 0.5f;

                            return (kvp.Key != vehicle)
                                   && vLane > newIndices.x
                                   && peelingDistance > (vDistance - newIndices.y) * cellUnitLength
                                   && Mathf.Abs(newIndices.x - vLane) * cellUnitLength <= vHalfWidth + vehicleHalfWidth;
                        }).OrderBy(kvp => kvp.Value.x).ToList();

                        if (rightObstacleVehicles.Count > 0 || newIndices.x > laneCount - 1.001f)
                        {
                            Debug.Log("sagda biri var");
                        }
                        else newIndices.x = Mathf.Min(newIndices.x + Time.deltaTime, laneCount - 1);
                    }
                    else newIndices.x = Mathf.Max(newIndices.x - Time.deltaTime, 0);
                }
                
                // update position 
                vehicles[vehicle] = newIndices;
                vehicle.transform.position = CellToWorldCoordinates(newIndices.x, newIndices.y - 0.5f);
                vehicle.transform.forward = CellToWorldForwardDirection(newIndices.x, newIndices.y);
                
                // decide acceleration
                vehicle.AccelerationInterpolator = Mathf.Clamp01(Maths.Remap(occupiedRoadDistance, 0f, 2 * peelingDistance, 0f, 1f));
                vehicle.BrakeDistance = Mathf.Max(0.001f, occupiedRoadDistance - (brakingOffset));
                
                // occupation overflow updates
                if (occupiesBackCell && baseCellIndex - 1 >= 0)
                {
                    var backCell = grid.Cells[baseLaneIndex, baseCellIndex - 1];
                    if(!dirtyCells.Contains(backCell)) dirtyCells.Add(backCell);
                    
                    backCell.occupation = cellOverflow;
                }
                else if (!occupiesBackCell && baseCellIndex + 1 < grid.Cells.GetLength(1))
                {
                    var nextCell = grid.Cells[baseLaneIndex, baseCellIndex + 1];
                    if(!dirtyCells.Contains(nextCell)) dirtyCells.Add(nextCell);
                    
                    nextCell.occupation = cellOverflow;
                }
                
                // if(vehicle.CurrentSpeed == 0f) Debug.Log(newIndices.y);
            }
        }

        #endregion
        
        public void Spawn(Vehicle vehicle)
        {
            var vehicleObj = vehicle.gameObject;

            var lane = Random.Range(0, grid.Cells.GetLength(0));
            var dist = Random.Range(0f, grid.Cells.GetLength(1));
            var position = CellToWorldCoordinates(lane, dist);

            vehicleObj.transform.position = position;

            vehicles[vehicle] = new Vector2(lane, dist);

            vehicle.Enable();
        }
        public void Spawn(Vehicle vehicle, float atDistance)
        {
            var vehicleObj = vehicle.gameObject;

            var lane = Random.Range(0, grid.Cells.GetLength(0));
            var dist = Mathf.Clamp(atDistance, 0f, grid.Cells.GetLength(1) - 1);
            var position = CellToWorldCoordinates(lane, dist);

            vehicleObj.transform.position = position;

            vehicles[vehicle] = new Vector2(lane, dist);

            vehicle.Enable();
        }
        public void Spawn(Vehicle vehicle, float atDistance, float onLane)
        {
            var vehicleObj = vehicle.gameObject;

            var xDim = grid.Cells.GetLength(0);
            var lane = (onLane + xDim) % xDim;
            
            var dist = atDistance % grid.Cells.GetLength(1);
            var position = CellToWorldCoordinates(lane, dist);

            vehicleObj.transform.position = position;

            vehicles[vehicle] = new Vector2(lane, dist);

            vehicle.Enable();
        }

        #region Cell Operations

        public Vector3 CellToWorldCoordinates(float lane, float distance, float laneOffset = 0.5f)
        {
            var segmentLength = grid.Cells.GetLength(1);
            if (lane < laneCount && distance < segmentLength)
            {
                var path = pathCreator.path;
                
                float dist = distance * cellUnitLength;
                
                Vector3 distancePoint = path.GetPointAtDistance(dist);
                Vector3 normal = path.GetNormalAtDistance(dist);
                
                return distancePoint + (lane + laneOffset) * normal * cellUnitLength;
            }

            throw new Exception("Indices was not in the range.");
        }
        
        public Vector3 CellToWorldForwardDirection(float lane, float distance, float laneOffset = 0.5f)
        {
            var segmentLength = grid.Cells.GetLength(1);
            if (lane < laneCount && distance < segmentLength)
            {
                var path = pathCreator.path;
                
                float dist = distance * cellUnitLength;
                
                Vector3 direction = path.GetDirectionAtDistance(dist);

                return direction;
            }

            throw new Exception("Indices was not in the range.");
        }

        #endregion

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (pathCreator == null || grid.Cells == null) return;

            var segmentLength = grid.Cells.GetLength(1);

            #region Grid Gizmos

            for(int x=0; x < laneCount; x++)
            {
                for(int y=1; y < segmentLength; y++)
                {
                    // Handles.color = SimulationSettings.Instance.EvaluateHeatColor(grid.Cells[x, y].heat);
                    // Handles.DrawAAPolyLine(50, new Vector3[] { CellToWorldCoordinates(x-1, y, 1), CellToWorldCoordinates(x, y, 1) });
                    // Handles.DrawAAPolyLine(50, new Vector3[] { CellToWorldCoordinates(x, y - 1, 1), CellToWorldCoordinates(x, y, 1) });
                    
                    Handles.color = Color.white;
                    // Handles.Label(CellToWorldCoordinates(x, y) + Vector3.up, $"{grid.Cells[x, y].heat}");

                    Handles.DrawAAPolyLine(3, CellToWorldCoordinates(x-1, y, 1), CellToWorldCoordinates(x, y, 1));
                    Handles.DrawAAPolyLine(3, CellToWorldCoordinates(x, y - 1, 1), CellToWorldCoordinates(x, y, 1));
                    Handles.Label(CellToWorldCoordinates(x, y - 0.5f) + Vector3.up, $"{grid.Cells[x, y].occupation}");
                }
            }

            #endregion
        }
        #endif
    }
}
