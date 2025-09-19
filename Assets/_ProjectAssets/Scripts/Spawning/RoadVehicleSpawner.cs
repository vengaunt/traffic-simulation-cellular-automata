using System;
using System.Collections.Generic;
using KalkuzSystems.DataStructures.Pooling;
using TrafficSimulation.DataStructures.Grids;
using TrafficSimulation.DataStructures.Segments;
using TrafficSimulation.Vehicles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TrafficSimulation.Spawning
{
    public class RoadVehicleSpawner : VehicleSpawner
    {
        [SerializeField] private RoadSegment segment;
        [SerializeField] private List<PoolObject> vehiclePrefabs;
        [SerializeField] private int burstCount;

        [SerializeField] private PoolObject blockObject;
        [SerializeField] private float blockDistance1;
        [SerializeField] private float blockDistance2;

        public override void Spawn(RoadSegment segment)
        {
            var vehiclePrefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Count)];
            var vehicle = UniversalPoolProvider.GetPool(vehiclePrefab.ID).Request(vehiclePrefab);

            segment.Spawn(vehicle.GetComponent<Vehicle>(), 0);
        }

        public void SpawnVehicle()
        {
            for (int i = 0; i < burstCount; i++)
            {
                Spawn(segment);
            }
        }

        public void SpawnObstacle()
        {
            var block = UniversalPoolProvider.GetPool(blockObject.ID).Request(blockObject);
            segment.Spawn(block.GetComponent<Vehicle>(), blockDistance1, 0);
            
            block = UniversalPoolProvider.GetPool(blockObject.ID).Request(blockObject);
            segment.Spawn(block.GetComponent<Vehicle>(), blockDistance2, -1);
        }

        private void Update()
        {
            return;
        }
    }
}