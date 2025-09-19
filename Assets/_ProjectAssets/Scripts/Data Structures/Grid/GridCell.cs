using System;
using System.Collections.Generic;
using TrafficSimulation.Vehicles;

namespace TrafficSimulation.DataStructures.Grids
{
    [Serializable]
    public class GridCell
    {
        public float occupation;
        public float heat;
    }
}