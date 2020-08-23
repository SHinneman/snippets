using System;
using System.Collections.Generic;
using UnityEngine;

// Todo: Add serialization when prototype is approved.

namespace GridUtilities
{
    [Serializable]
    public class Cell
    {
        /// <summary>
        /// Representation of a 2D cell in 3D space.
        /// </summary>

        public int Id { get { return id; } protected set { id = value;} }
        public float Width { get { return width; } protected set { width = value; } }
        public float Height { get { return height; } protected set { height = value; } }
        public Vector3 TopLeft { get { return topLeft; } protected set { topLeft = value; } }
        public Vector3 TopRight { get { return topRight; } protected set { topRight = value; } }
        public Vector3 BottomLeft { get { return bottomLeft; } protected set { bottomLeft = value; } }
        public Vector3 BottomRight { get { return bottomRight; } protected set { bottomRight = value; } }
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                UpdateBounds();
            }
        }

        [SerializeField]     
        private int id = 0;
        [SerializeField]      
        private float width = 0;
        [SerializeField]    
        private float height = 0;
        [SerializeField]     
        private Vector3 position = new Vector3();
        [SerializeField]    
        private Vector3 topLeft = new Vector3();
        [SerializeField]    
        private Vector3 topRight = new Vector3();
        [SerializeField]     
        private Vector3 bottomLeft = new Vector3();
        [SerializeField]     
        private Vector3 bottomRight = new Vector3();

        /// <summary>
        /// Initializes a new instance of the Cell class.
        /// </summary>
        /// <param name="id">Unique identifier of the cell</param>
        /// <param name="position">Position in 3d space representing the center of the cell.</param>
        /// <param name="width">Width of the cell (represented on the x-axis).</param>
        /// <param name="height">Height of the cell (representeded on the z-axis).</param>
        public Cell(int id, Vector3 position, float width, float height)
        {
            Id = id;
            Width = width;
            Height = height;
            Position = position;

            UpdateBounds();
        }

        protected Cell() {}
        
        /// <summary>
        /// Calculate and set bounds (TopLeft, BottomRight, ...) with the present members.
        /// </summary>
        protected virtual void UpdateBounds()
        {
            float halfWidth = Width / 2.0f;
            float halfHeigth = Height / 2.0f;
            float left = Position.x - halfWidth;
            float right = Position.x + halfWidth;
            float top = Position.z + halfHeigth;
            float bottom = Position.z - halfHeigth;
            TopLeft = new Vector3(left, Position.y, top);
            TopRight = new Vector3(right, Position.y, top);
            BottomLeft = new Vector3(left, Position.y, bottom);
            BottomRight = new Vector3(right, Position.y, bottom);
        }
    }

    [Serializable]
    public class Grid : ISerializationCallbackReceiver
    {
        /// <summary>
        /// Representation of a 2D grid of cells in 3D space.
        /// </summary>
        public int Id { get { return id; } protected set { id = value; } }
        public int Rows { get { return rows; } protected set { rows = value; } }
        public int Columns { get { return columns; } protected set { columns = value; } }
        public float CellSize { get { return cellSize; } protected set { cellSize = value; } }
        public float Width { get { return width; } protected set { width = value; } }
        public float Height { get { return height; } protected set { height = value; } }
        public Vector3 Position
        {
            get { return position; }
            set
            {
                Vector3 delta = value - position;
                MoveCells(delta);
                position = value;
                UpdateBounds();
            }
        }
        public Vector3 TopLeft { get { return topLeft; } protected set { topLeft = value; } }
        public Vector3 TopRight { get { return topRight; } protected set { topRight = value; } }
        public Vector3 BottomLeft { get { return bottomLeft; } protected set { bottomLeft = value; } }
        public Vector3 BottomRight { get { return bottomRight; } protected set { bottomRight = value; } }
        public Dictionary<int, Cell> Cells = new Dictionary<int, Cell>();
        public bool DrawGrid = true;
        public bool DrawOutline = true;
        public bool DrawCells = true;
        public bool DrawCellCenters = true;

        [SerializeField]
        private int id = 0;
        [SerializeField]
        private int rows = 0;
        [SerializeField]
        private int columns = 0;
        [SerializeField]
        private float cellSize = 0;
        [SerializeField] 
        private float width = 0;      
        [SerializeField]    
        private float height = 0;
        [SerializeField]    
        private Vector3 position = new Vector3();
        [SerializeField]       
        private Vector3 topLeft = new Vector3();
        [SerializeField]      
        private Vector3 topRight = new Vector3();
        [SerializeField]    
        private Vector3 bottomLeft = new Vector3();
        [SerializeField]    
        private Vector3 bottomRight = new Vector3();
        [SerializeField]
        private List<int> cellKeys = new List<int>();
        [SerializeField]
        private List<Cell> cellValues = new List<Cell>();

        /// <summary>
        /// Initializes a new instance of the Grid class.
        /// </summary>
        /// <param name="columns">Amount of columns to create in this grid.</param>
        /// <param name="rows">Amount of rows to create in this grid.</param>
        /// <param name="cellSize">The size of cells in this grid.</param>
        /// <param name="position">Position in 3d space representing the center of the grid.</param>
        public Grid(int id, int columns, int rows, float cellSize, Vector3 position)
        {
            Id = id;
            Columns = columns;
            Rows = rows;
            CellSize = cellSize;
            Position = position;

            UpdateBounds();
            CreateCells();
        }

        public void OnBeforeSerialize()
        {
            cellKeys.Clear();
            cellValues.Clear();

            foreach (KeyValuePair<int, Cell> pair in Cells)
            {
                cellKeys.Add(pair.Key);
                cellValues.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Cells = new Dictionary<int, Cell>();

            for (int i = 0; i != Math.Min(cellKeys.Count, cellValues.Count); i++)
            {
                Cells.Add(cellKeys[i], cellValues[i]);
            }
        }

        protected Grid() {}

        /// <summary>
        /// Create the cells with the set members.
        /// </summary>
        private void CreateCells()
        {
            float halfCellHeight = CellSize / 2.0f;
            float halfCellWidth = CellSize / 2.0f;
            Vector3 origin = TopLeft;
            int cellCount = 0;
            for (int r = 0; r < Rows; r++)
            {
                Vector3 nextPoint = new Vector3(origin.x, origin.y, origin.z - (CellSize * r));
                for (int c = 0; c < Columns; c++)
                {
                    Vector3 cellPosition = new Vector3(nextPoint.x + halfCellWidth, Position.y, nextPoint.z - halfCellHeight);
                    Cell tempCell = new Cell(cellCount, cellPosition, CellSize, CellSize);
                    Cells[cellCount] = tempCell;

                    nextPoint = tempCell.TopRight;
                    cellCount++;
                }
            }
        }

        /// <summary>
        /// Move the cells origin, contained in this grid, by the given delta. 
        /// </summary>
        /// <param name="deltaPostion">Difference to move the origin of the cells with.</param>
        private void MoveCells(Vector3 deltaPostion)
        {
            foreach (KeyValuePair<int, Cell> entry in Cells)
            {
                Vector3 newPos = entry.Value.Position + deltaPostion;
                entry.Value.Position = newPos;
            }
        }

        /// <summary>
        /// Calculate and set bounds (TopLeft, BottomRight, ...) with the present members.
        /// </summary>
        private void UpdateBounds()
        {
            Width = Columns * CellSize;           
            Height = Rows * CellSize;
            float halfWidth = Width / 2.0f;
            float halfHeigth = Height / 2.0f;
            float left = Position.x - halfWidth;
            float right = Position.x + halfWidth;
            float top = Position.z + halfHeigth;
            float bottom = Position.z - halfHeigth;
            TopLeft = new Vector3(left, Position.y, top);
            TopRight = new Vector3(right, Position.y, top);
            BottomLeft = new Vector3(left, Position.y, bottom);
            BottomRight = new Vector3(right, Position.y, bottom);
        }
    }

    [Serializable]
    public class GridManager : MonoBehaviour, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Class to manage grids. 
        /// </summary>

        public int IdCounter = 0;
        public Dictionary<int, Grid> Grids = new Dictionary<int, Grid>();

        [SerializeField]
        private List<int> gridKeys = new List<int>();
        [SerializeField]
        private List<Grid> gridValues = new List<Grid>();

        /// <summary>
        /// Add a grid to the manager with the given settings.
        /// </summary>
        /// <param name="columns">Amount of columns in the new grid.</param>
        /// <param name="rows">Amount of rows in the new grid.</param>
        /// <param name="cellSize">Size of the cells in the new grid.</param>
        /// <param name="position">Origin position of the center of the new grid.</param>
        /// <returns></returns>
        public Grid AddGrid(int columns, int rows, float cellSize, Vector3 position)
        {
            Grid grid = new Grid(IdCounter++, columns, rows, cellSize, position);
            Grids[grid.Id] = grid;
            return Grids[grid.Id];
        }

        public void RemoveGrid(Grid grid)
        {
            RemoveGrid(grid.Id);
        }

        public void RemoveGrid(int id)
        {
            Grids.Remove(id);
        }

        public void RemoveGrids(List<int> ids)
        {
            foreach(int i in ids)
            {
                RemoveGrid(i);
            }
        }

        public void OnBeforeSerialize()
        {
            gridKeys.Clear();
            gridValues.Clear();

            foreach (KeyValuePair<int, Grid> pair in Grids)
            {
                gridKeys.Add(pair.Key);
                gridValues.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Grids = new Dictionary<int, Grid>();

            for (int i = 0; i != Math.Min(gridKeys.Count, gridValues.Count); i++)
            {
                Grids.Add(gridKeys[i], gridValues[i]);
            }
        }
    }
}