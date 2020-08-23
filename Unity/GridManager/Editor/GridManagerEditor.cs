using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace GridUtilities
{
    [CustomEditor(typeof(GridManager))]
    public class GridManagerEditor : Editor
    {
        /// <summary>
        /// Custom editor for a GridManager instance.
        /// </summary>

        private Tool lastTool = Tool.None;
        private int columns = 4;
        private int rows = 4;
        private float cellSize = 1.0f;
        private Vector3 position = new Vector3(0, 0, 0);
        private Vector3 normal = new Vector3(0, 1, 0);
        private List<Grid> gridsToDelete = new List<Grid>();
        private string[] gridListHeaders = new string[] { "Nr.", "Position", "Show", "Outline", "Cells", "Centers", "Delete" };
        private float[] gridListColSize = new float[] { 40.0f, 200.0f, 80.0f, 80.0f, 80.0f, 80.0f, 100.0f };
        private bool[] areFoldsOpen = new bool[] {true, true};

        void OnEnable()
        {
            // This code block stops from using the default tool that Unity would when selecting the GridManager.
            lastTool = Tools.current;
            Tools.current = Tool.None;
        }

        void OnDisable()
        {
            // Reset the tool back to it's original state.
            Tools.current = lastTool;
        }

        void OnSceneGUI()
        {
            GridManager gridManager = (GridManager)target;

            // Draw the grids.
            foreach (KeyValuePair<int, Grid> pair in gridManager.Grids)
            {
                if (!pair.Value.DrawGrid)
                {
                    continue;
                }

                // Draw handles.
                DrawGridHandlesInScene(pair.Value);

                if (pair.Value.DrawOutline)
                {
                    DrawSceneGridOutline(pair.Value);
                }

                if (pair.Value.DrawCells)
                {
                    DrawSceneCellOutlines(pair.Value);
                }

                if (pair.Value.DrawCellCenters)
                {
                    DrawSceneCellCenters(pair.Value);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            GridManager gridManager = (GridManager)target;

            DrawInspectorGridOptions(gridManager, ref areFoldsOpen[0]);
            EditorGUILayout.Separator();
            DrawInspectorGridList(gridManager, ref areFoldsOpen[1]);
            EditorGUILayout.Separator();

            // Todo: Check if we can't call this less to update the scene.
            EditorUtility.SetDirty(target);
        }

        // OnSceneGUI Helpers
        private void DrawGridHandlesInScene(Grid grid)
        {
            grid.Position = Handles.PositionHandle(grid.Position, Quaternion.identity);
        }

        private void DrawSceneCellOutlines(Grid grid, float thickness = 0.01f)
        {
            foreach(KeyValuePair<int, Cell> pair in grid.Cells)
            {
                DrawSceneCellOutline(pair.Value, thickness);
            }
        }

        private void DrawSceneCellCenters(Grid grid)
        {
            for (int i = 0; i < grid.Cells.Count; i++)
            {
                Cell cell = grid.Cells[i];
                DrawSceneCellCenter(cell);
            }
        }

        private void DrawSceneGridOutline(Grid grid, float thickness = 0.1f)
        {
            Handles.DrawWireCube(grid.Position, new Vector3(grid.Width, thickness, grid.Height));
        }

        private void DrawSceneCellOutline(Cell cell, float cellThickness)
        {
            Handles.DrawWireCube(cell.Position, new Vector3(cell.Width, cellThickness, cell.Height));
        }

        private void DrawSceneCellCenter(Cell cell)
        {
            Handles.DrawLine(cell.Position, cell.TopLeft);
            Handles.DrawLine(cell.Position, cell.TopRight);
            Handles.DrawLine(cell.Position, cell.BottomLeft);
            Handles.DrawLine(cell.Position, cell.BottomRight);
            Handles.DrawWireDisc(cell.Position, normal, cell.Width*0.1f);
        }

        // OnInspectorGUI helpers
        private void DrawInspectorGridOptions(GridManager target, ref bool isFoldoutOpen)
        {
            isFoldoutOpen = EditorGUILayout.Foldout(isFoldoutOpen, "Grid options");
            if (isFoldoutOpen)
            {
                var level = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Add Grid");
                EditorGUI.indentLevel++;
                columns = EditorGUILayout.IntField("Columns:", columns);
                rows = EditorGUILayout.IntField("Rows:", rows);
                cellSize = EditorGUILayout.FloatField("Cellsize", cellSize);
                position = EditorGUILayout.Vector3Field("Position", position);

                if (GUILayout.Button("Add"))
                {
                    target.AddGrid(columns, rows, cellSize, position);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Separator();
                if (GUILayout.Button("Delete all grids"))
                {
                    target.Grids.Clear();
                }
                EditorGUI.indentLevel = level;
            }
        }

        private void DrawInspectorGridList(GridManager target, ref bool isFoldoutOpen)
        {
            isFoldoutOpen = EditorGUILayout.Foldout(isFoldoutOpen, "Grids");
            if (isFoldoutOpen)
            {
                var level = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                for (int i = 0; i < gridListHeaders.Length; i++)
                {
                    EditorGUILayout.LabelField(gridListHeaders[i], GUILayout.ExpandWidth(false), GUILayout.MaxWidth(gridListColSize[i]));
                }
                EditorGUILayout.EndHorizontal();


                foreach(KeyValuePair<int, Grid> pair in target.Grids)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                    // Col 1: Index 
                    EditorGUILayout.LabelField(String.Format("{0} ", pair.Value.Id), GUILayout.ExpandWidth(false), GUILayout.MaxWidth(gridListColSize[0]));
                    // Col 2: Position 
                    pair.Value.Position = EditorGUILayout.Vector3Field("", pair.Value.Position, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(gridListColSize[1]));
                    // Col 3: Visible
                    pair.Value.DrawGrid = EditorGUILayout.Toggle(pair.Value.DrawGrid, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(gridListColSize[2]));
                    // Col 3: Outline
                    pair.Value.DrawOutline = EditorGUILayout.Toggle(pair.Value.DrawOutline, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(gridListColSize[3]));
                    // Col 4: Cells
                    pair.Value.DrawCells = EditorGUILayout.Toggle(pair.Value.DrawCells, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(gridListColSize[4]));
                    // Col 5: Center
                    pair.Value.DrawCellCenters = EditorGUILayout.Toggle(pair.Value.DrawCellCenters, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(gridListColSize[5]));
                    // Col 6: Delete 
                    if (GUILayout.Button("Delete", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(gridListColSize[6])))
                    {
                        gridsToDelete.Add(pair.Value);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.LabelField("-----");
                }

                for(int i = 0; i < gridsToDelete.Count; i++)
                {
                    target.RemoveGrid(gridsToDelete[i]);
                }

                if(gridsToDelete.Count>0)
                    gridsToDelete.Clear();

                EditorGUI.indentLevel = level;
            }
        }
    }
}