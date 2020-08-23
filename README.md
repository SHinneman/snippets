# Snippets
## Unity/Gridmanager
Gridmanager tool created in Unity 2019.3.4f1.
### GridManager.cs
Containes base classes for the gridmanager tool.
Provides classes to create cells, grids and a manager. Classes are serializable by Unity.
### Editor/GridManagerEditor.cs
The GridManagerEditor provides a user interface for the Unity inspector to visualize the GridManager options.

**Grid Visibility**

* Toggling visibility of grids and their cells.
* Toggling of drawing grid outline, outlines of cells in grid and centerpoints of cells in grid.

**Managing Grids**

* Delete single grids.
* Delete all grids.
* Create grid with given amount of rows, columns, cell size and position.

**In Scene interactions**

* Grids can be separately moved with Unity gizmo when gridmanager is selected  
