using System;
using TrafficSimulation.DataStructures.Segments;
using UnityEditor;
using UnityEngine;

namespace TrafficSimulation.Editor
{
    public class CurrentVehiclesDataWindow : EditorWindow
    {
        public static RoadSegment RoadSegment;
        
        [MenuItem("Traffic Simulation/Vehicles Data")]
        static void OpenWindow()
        {
            var window = GetWindow<CurrentVehiclesDataWindow>();
            window.Show();
        }

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (RoadSegment == null)
            {
                RoadSegment = GameObject.FindObjectOfType<RoadSegment>();
                if (RoadSegment == null) return;
            }

            var vehiclesDict = RoadSegment.Vehicles;
            if (vehiclesDict == null) return;

            foreach (var vehicle in vehiclesDict.Keys)
            {
                vehicle.RenderAsEditorGUILayout();
                EditorGUILayout.Space(10);
            }
        }
    }
}