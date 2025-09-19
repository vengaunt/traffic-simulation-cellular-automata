using System;
using KalkuzSystems;
using UnityEditor;
using UnityEngine;

namespace TrafficSimulation.Settings
{
    // [CreateAssetMenu(menuName = "Traffic Simulation/Simulation Settings", fileName = "Simulation Settings")]
    public class SimulationSettings : ScriptableObject
    {
        private static SimulationSettings instance;
        public static SimulationSettings Instance => instance ? instance : SimulationSettingsProvider.Settings;

        [SerializeField] private bool logsEnabled;
        [SerializeField] private bool stopSimulationWhenAccidentHappens;
        
        [SerializeField] private float cellOccupancyDecreasingRate;
        [SerializeField] private float cellHeatDecreasingRate;
        [SerializeField] private float cellHeatingFactor;

        [SerializeField] private HeatmapRange heatmapRange;

        public bool LogsEnabled => logsEnabled;
        public bool StopSimulationWhenAccidentHappens => stopSimulationWhenAccidentHappens;
        public float CellOccupancyDecreasingRate => cellOccupancyDecreasingRate;
        public float CellHeatDecreasingRate => cellHeatDecreasingRate;
        public float CellHeatingFactor => cellHeatingFactor;
        public HeatmapRange HeatmapRange => heatmapRange;

        public Color EvaluateHeatColor(float heat)
        {
            if (heat > heatmapRange.HighTrafficHeat.Density) return heatmapRange.HighTrafficHeat.Color;
            else if (heat > heatmapRange.ModerateTrafficHeat.Density)
            {
                var t = Mathf.InverseLerp(heatmapRange.ModerateTrafficHeat.Density, heatmapRange.HighTrafficHeat.Density, heat);
                return Color.Lerp(heatmapRange.ModerateTrafficHeat.Color, heatmapRange.HighTrafficHeat.Color, t);
            } 
            else if (heat > heatmapRange.LowTrafficHeat.Density)
            {
                var t = Mathf.InverseLerp(heatmapRange.LowTrafficHeat.Density, heatmapRange.ModerateTrafficHeat.Density, heat);
                return Color.Lerp(heatmapRange.LowTrafficHeat.Color, heatmapRange.ModerateTrafficHeat.Color, t);
            }
            else return heatmapRange.LowTrafficHeat.Color;
        }

        #if UNITY_EDITOR
        [MenuItem("Traffic Simulation/Create Settings")]
        private static void CreateSimulationSettings()
        {
            if (instance != null)
            {
                Debug.LogError("There is already a setting asset.", instance);
                return;
            }

            var path = EditorUtility.SaveFilePanel("Select Create Location", "Assets/_ProjectAssets/Scriptable Objects/Settings", "Simulation Settings", "asset");
            if (string.IsNullOrEmpty(path)) return;

            path = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));

            var inst = CreateInstance<SimulationSettings>();
            AssetDatabase.CreateAsset(inst, path);
            instance = inst;
            
            AssetDatabase.SaveAssets();
        }
        #endif

        private void OnEnable()
        {
            if (instance == null) instance = this;
        }
    }

    [Serializable]
    public class HeatmapRange
    {
        [Serializable]
        public class HeatmapDensityColorPair
        {
            [SerializeField] private Color color;
            [SerializeField] private float density;

            public Color Color => color;
            public float Density => density;
        }
        
        [SerializeField] private HeatmapDensityColorPair lowTrafficHeat;
        [SerializeField] private HeatmapDensityColorPair moderateTrafficHeat;
        [SerializeField] private HeatmapDensityColorPair highTrafficHeat;

        public HeatmapDensityColorPair LowTrafficHeat => lowTrafficHeat;
        public HeatmapDensityColorPair ModerateTrafficHeat => moderateTrafficHeat;
        public HeatmapDensityColorPair HighTrafficHeat => highTrafficHeat;
    }
}