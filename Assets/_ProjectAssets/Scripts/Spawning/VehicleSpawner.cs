using System;
using TrafficSimulation.DataStructures.Grids;
using TrafficSimulation.DataStructures.Segments;
using UnityEngine;

namespace TrafficSimulation.Spawning
{
    public abstract class VehicleSpawner : MonoBehaviour
    {
        [SerializeField] protected float spawnFrequency;

        public abstract void Spawn(RoadSegment segment);
    }
}