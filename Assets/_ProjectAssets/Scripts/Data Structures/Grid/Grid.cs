using System;

namespace TrafficSimulation.DataStructures.Grids
{
    [Serializable]
    public class Grid
    {
        public GridCell[,] Cells;
        public Grid(uint width, uint height)
        {
            Cells = new GridCell[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Cells[i, j] = new GridCell();
                }
            }
        }
    }
}