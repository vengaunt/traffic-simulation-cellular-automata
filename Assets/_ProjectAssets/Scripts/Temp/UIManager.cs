using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TrafficSimulation.DataStructures.Segments;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public RoadSegment segment;
    public TMP_Text vehicleText;

    private void Update()
    {
        vehicleText.text = $"Vehicle Count: {segment.Vehicles.Where(kvp => !kvp.Key.VehicleData.ActAsObstacle).ToList().Count}";
    }

    public void SwitchTimeScale()
    {
        Time.timeScale = 1.05f - Time.timeScale;
    }

    public void LogAllVehicles()
    {
        foreach (var vehicle in segment.Vehicles.Keys)
        {
            vehicle.LogBlackBox();
        }
    }
}
