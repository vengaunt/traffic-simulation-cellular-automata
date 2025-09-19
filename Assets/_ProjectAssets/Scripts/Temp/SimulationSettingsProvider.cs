using System;
using UnityEngine;

namespace TrafficSimulation.Settings
{
    public class SimulationSettingsProvider : MonoBehaviour
    {
        public static SimulationSettingsProvider Instance { get; set; }

        [SerializeField] private SimulationSettings simulationSettings;

        public static SimulationSettings Settings => Instance.simulationSettings;
        
        private void Awake()
        {
            Instance = this;
        }
    }
}