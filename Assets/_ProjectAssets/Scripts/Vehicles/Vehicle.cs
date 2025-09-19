using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KalkuzSystems;
using KalkuzSystems.DataStructures.Pooling;
using TrafficSimulation.Settings;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TrafficSimulation.Vehicles
{
    [RequireComponent(typeof(PoolObject))]
    public abstract class Vehicle : MonoBehaviour
    {
        #region Static Region

        public const float SPEED_MS_2_PEELING_DISTANCE_MULTIPLIER = 1.8f;
        public const float SPEED_KMH_2_PEELING_DISTANCE_MULTIPLIER = 0.5f;
        public const float MS_2_KMH_MULTIPLIER = 3.6f;
        public const float KMH_2_MS_MULTIPLIER = 1 / 3.6f;

        #endregion

        #region References
        
        [SerializeField] protected VehicleData vehicleData;      
        
        private PoolObject poolObjectComponent;

        #endregion

        #region Fields
        
        /// <summary>
        /// Current speed of the vehicle in terms of m/s
        /// </summary>
        private float currentSpeed;

        private bool isBraking;
        private float requiredDeceleration = 0f;
        
        private float brakingResponseTime;
        private float timePassedForBrakingAction;

        #endregion

        #region Properties

        public VehicleData VehicleData => vehicleData;
        
        public bool Crashed { get; set; }
        
        /// <inheritdoc cref="currentSpeed"/>
        public float CurrentSpeed => currentSpeed;
        
        public float AccelerationInterpolator { get; set; }

        public float PeelingDistance => currentSpeed * SPEED_MS_2_PEELING_DISTANCE_MULTIPLIER;
        
        public float BrakeDistance { get; set; }
        
        public float DistanceOnRoad { get; set; }

        #endregion

        // Experimental Zone
        private List<string> logs = new List<string>();
        
        protected void Awake()
        {
            poolObjectComponent = GetComponent<PoolObject>();
        }

        protected void Start()
        {
            if (vehicleData == null) throw new Exception("Vehicle data cannot be null");
        }

        protected virtual void Update()
        {
            if (Crashed)
            {
                currentSpeed = 0f;
                return;
            }

            var netAcceleration = 0f;
            if (AccelerationInterpolator > 0.5f)
            {
                if (isBraking) isBraking = false;
                netAcceleration = Maths.Remap(AccelerationInterpolator, 0.5f, 1f, 0f, vehicleData.AccelerationPotential);
                currentSpeed = Mathf.Clamp(currentSpeed + netAcceleration * Time.deltaTime, 0, vehicleData.MaxSpeed * KMH_2_MS_MULTIPLIER);
            }
            else
            {
                if (!isBraking)
                {
                    requiredDeceleration = Mathf.Max(ComputeRequiredDeceleration(BrakeDistance), vehicleData.DecelerationPotential);
                    
                    // Debug.Log($"Required Decel: {requiredDeceleration}");
                    isBraking = true;
                    
                    // brakingResponseTime = Random.Range(0.5f, 1.5f);
                    // timePassedForBrakingAction = 0f;
                }
                
                // timePassedForBrakingAction += Time.deltaTime;
                
                netAcceleration = requiredDeceleration;
                currentSpeed = Mathf.Clamp(currentSpeed + netAcceleration * Time.deltaTime, 0, vehicleData.MaxSpeed * KMH_2_MS_MULTIPLIER);
                
                // if (timePassedForBrakingAction > brakingResponseTime)
                // {
                //     currentSpeed = Mathf.Clamp(currentSpeed + netAcceleration * Time.deltaTime, 0, vehicleData.MaxSpeed * KMH_2_MS_MULTIPLIER);
                // }
            }
            
            if (SimulationSettingsProvider.Settings.LogsEnabled)
            {
                if (currentSpeed != 0f)
                {
                    var t = $"{(Time.time).ToString("F",System.Globalization.CultureInfo.InvariantCulture)}";
                    var s = $"{(DistanceOnRoad).ToString("F", System.Globalization.CultureInfo.InvariantCulture)}";
                    var v = $"{(currentSpeed * MS_2_KMH_MULTIPLIER).ToString("F",System.Globalization.CultureInfo.InvariantCulture)}";
                    var ps = $"{(PeelingDistance).ToString("F",System.Globalization.CultureInfo.InvariantCulture)}";
                    var a = $"{(requiredDeceleration).ToString("F",System.Globalization.CultureInfo.InvariantCulture)}";
                    logs.Add($"{t},{s},{v},{ps},{a}");
                }
            }
        }

        private float ComputeRequiredDeceleration(float distance)
        {
            return -Mathf.Pow(currentSpeed, 2) / (2f * distance);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * PeelingDistance);
        }

        public void Enable()
        {
            enabled = true;
            gameObject.SetActive(true);
            
            currentSpeed = Random.Range(0, Math.Min(vehicleData.MaxSpeed * KMH_2_MS_MULTIPLIER, 30f));
            Crashed = false;
        }

        public void Disable()
        {
            LogBlackBox();
            
            enabled = false;
            gameObject.SetActive(false);
            poolObjectComponent.ReturnToPool();
        }

        public void Crash()
        {
            LogBlackBox();
            
            Crashed = true;
        }

        public void LogBlackBox()
        {
            if (!SimulationSettingsProvider.Settings.LogsEnabled || vehicleData.ActAsObstacle) return;
                
            var path = Path.Combine($"{Application.persistentDataPath}", $"log_{vehicleData.name}_{DateTime.Now.Ticks}.csv");
                
            logs.Insert(0, "Time,Speed,Peeling Distance,Braking");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(string.Join("\n", logs));
                writer.Close();
            }
            logs = new List<string>();
        }

#if UNITY_EDITOR

        private bool foldout = true;
        public void RenderAsEditorGUILayout()
        {
            if (vehicleData.ActAsObstacle) return;
            
            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, name);
            if (foldout)
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);
                EditorGUILayout.FloatField("Current Speed", currentSpeed);
                EditorGUILayout.FloatField("Max Speed", vehicleData.MaxSpeed);
                EditorGUILayout.FloatField("Deceleration Potential", vehicleData.AccelerationPotential);
                EditorGUILayout.FloatField("Acceleration Potential", vehicleData.DecelerationPotential);
                GUI.enabled = true;
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
#endif
    }
}