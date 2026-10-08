using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//Class representing a cell in the Wave Function Collapse algorithm, which holds possible rooms, the selected room, and its collapsed state.
public class Cells
{

    public List<Rooms> possibleRooms = new List<Rooms>();

    public Rooms room; 

    public bool collapsed = false; 

    public void GeneratePossibleRooms(List<Rooms> rooms)
    {
        possibleRooms = new List<Rooms>(rooms);
    }

}

//Class responsible for generating a grid of cells using the Wave Function Collapse algorithm, managing the generation process, and checking for conflicts between adjacent cells.
public class WFCGenerator : MonoBehaviour
{

    // Grid size in terms of number of cells in x and y directions
    public Vector2 GRID_SIZE = new Vector2(10, 10);

    // Cell size in terms of world units
    public Vector2 CELL_SIZE = new Vector2(10, 10);

    // Offset for the grid in world space
    public Vector2 GRID_OFFSET = new Vector2(0, 0);

    // List of possible room types that can be assigned to cells
    public List<Rooms> possibleRooms = new List<Rooms>();

    // List of cells in the grid
    private List<Cells> cells = new List<Cells>();

    // Flag indicating whether the generation process has failed due to conflicts or lack of possible rooms
    public bool failed = false;

    // Flag indicating whether the generation process is complete
    public bool generationComplete = false;

    // Method to generate the grid of cells and initialize their possible rooms based on the provided list of room types.
    public void GenerateCells()
    {

        for (int x = 0; x < GRID_SIZE.x; x++)
        {
            for (int y = 0; y < GRID_SIZE.y; y++)
            {
                Cells cell = new Cells();
                cell.GeneratePossibleRooms(possibleRooms);
                cells.Add(cell);
            }
        }

        RemovePossbilitiesFromOuterWallCells();
        CheckForConflicts(); 

    }

    // Method to check if all cells in the grid have been collapsed (i.e., assigned a room). If any cell is uncollapsed, it returns false; otherwise, it sets the generationComplete flag to true and returns true.
    public bool CheckForUncollapsedCells()
    {

        for (int i = 0; i < cells.Count; i++)
        {
            if (!cells[i].collapsed)
            {
                return false;
            }
        }

        generationComplete = true;

        return generationComplete; 

    }

    public bool CheckAllRoomsConnectivity()
    {

        bool allConnected = true;

        int startingIndex = -1; 
        int[] dx = new int[4] { 0, 1, 0, -1 }; 
        int[] dy = new int[4] { -1, 0, 1, 0 };

        bool[] visitedCells = new bool[cells.Count];

        for (int i = 0; i < cells.Count; i++)
        {

            foreach (DirectionType direction in cells[i].room.directionTypes)
            {

                if (direction == DirectionType.Door || direction == DirectionType.Hallway)
                {
                    startingIndex = i;
                    visitedCells[i] = true;
                    i = cells.Count;
                    break;

                }
            }
        }

        Queue<int> cellsToCheck = new Queue<int>();

        if (startingIndex == -1)
        {
            Debug.LogError("No starting cell found with a door or hallway.");
            return false;
        }

        cellsToCheck.Enqueue(startingIndex);

        do
        {

            int currentIndex = cellsToCheck.Dequeue();

            for (int directionIndex = 0; directionIndex < 4; directionIndex++)
            {
                DirectionType currentDirection = cells[currentIndex].room.directionTypes[directionIndex];
                if (currentDirection == DirectionType.Door || currentDirection == DirectionType.Hallway)
                {
                    int neighborX = (currentIndex % (int)GRID_SIZE.x) + dx[directionIndex];
                    int neighborY = (currentIndex / (int)GRID_SIZE.x) + dy[directionIndex];
                    if (neighborX >= 0 && neighborX < GRID_SIZE.x && neighborY >= 0 && neighborY < GRID_SIZE.y)
                    {
                        int neighborIndex = neighborY * (int)GRID_SIZE.x + neighborX;
                        if (!visitedCells[neighborIndex] && (cells[neighborIndex].room.directionTypes[(directionIndex + 2) % 4] == DirectionType.Door || cells[neighborIndex].room.directionTypes[(directionIndex + 2) % 4] == DirectionType.Hallway))
                        {
                            visitedCells[neighborIndex] = true;
                            cellsToCheck.Enqueue(neighborIndex);
                        }
                    }
                }
            }

        }
        while (cellsToCheck.Count > 0);

        for (int i = 0; i < visitedCells.Length; i++)
        {
            if (!visitedCells[i])
            {
                allConnected = false;
                break; 
            }
        }

        return allConnected; 

    }

    public void RemovePossbilitiesFromOuterWallCells()
    {

        int width = (int)GRID_SIZE.x;
        int height = (int)GRID_SIZE.y;

        for (int i = 0; i < cells.Count; i++)
        {
            int column = i % width;
            int row = i / width;

            cells[i].possibleRooms.RemoveAll(room =>
                (row == 0 && room.directionTypes[0] != DirectionType.Wall) ||
                (column == width - 1 && room.directionTypes[1] != DirectionType.Wall) ||
                (row == height - 1 && room.directionTypes[2] != DirectionType.Wall) ||
                (column == 0 && room.directionTypes[3] != DirectionType.Wall)
            );

            if (cells[i].possibleRooms.Count == 0)
            {
                failed = true;
                Debug.LogError("No possible rooms for outer wall cell at index: " + i);
                return;
            }
        }

    }

    // Method to select the next cell to collapse based on the lowest entropy (i.e., the cell with the fewest possible rooms). If a cell with the lowest entropy is found, it randomly selects one of its possible rooms, collapses the cell, and checks for conflicts with adjacent cells. If no uncollapsed cells are found, it randomly selects a cell and collapses it.
    public void SelectNextRoom()
    {

        // Iterate through all cells to find the one with the lowest entropy (fewest possible rooms) that is not yet collapsed.
        int lowestEntropy = int.MaxValue;
        List<int> candidates = new List<int>();

        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].collapsed)
                continue;

            int entropy = cells[i].possibleRooms.Count;

            if (entropy == 0)
            {
                failed = true;
                return;
            }

            if (entropy < lowestEntropy)
            {
                lowestEntropy = entropy;
                candidates.Clear();
            }

            if (entropy == lowestEntropy)
                candidates.Add(i);
        }

        if (candidates.Count == 0)
        {
            generationComplete = true;
            return;
        }

        int cellIndex = candidates[Random.Range(0, candidates.Count)];
        Cells cell = cells[cellIndex];

        int roomIndex = Random.Range(0, cell.possibleRooms.Count);
        cell.room = cell.possibleRooms[roomIndex];
        cell.possibleRooms = new List<Rooms> { cell.room };
        cell.collapsed = true;

        CheckForConflicts();

    }

    private void OnDrawGizmos()
    {
        if (cells == null || GRID_SIZE.x < 1)
            return;

        int width = (int)GRID_SIZE.x;

        for (int i = 0; i < cells.Count; i++)
        {
            int column = i % width;
            int row = i / width;

            Vector3 position = transform.position + new Vector3(
                GRID_OFFSET.x + column * CELL_SIZE.x,
                0f,
                GRID_OFFSET.y - row * CELL_SIZE.y
            );

            Cells cell = cells[i];

            Gizmos.color = cell.possibleRooms.Count == 0
                ? Color.red
                : cell.collapsed ? Color.green : Color.gray;

            Gizmos.DrawWireCube(
                position,
                new Vector3(CELL_SIZE.x, 0.1f, CELL_SIZE.y)
            );

            if (!cell.collapsed || cell.room == null)
                continue;

            float halfWidth = CELL_SIZE.x * 0.5f;
            float halfDepth = CELL_SIZE.y * 0.5f;

            Vector3 northWest = position + new Vector3(-halfWidth, 0, halfDepth);
            Vector3 northEast = position + new Vector3(halfWidth, 0, halfDepth);
            Vector3 southEast = position + new Vector3(halfWidth, 0, -halfDepth);
            Vector3 southWest = position + new Vector3(-halfWidth, 0, -halfDepth);

            // North
            Gizmos.color = GetDirectionColor(cell.room.directionTypes[0]);
            Gizmos.DrawLine(northWest, northEast);

            // East
            Gizmos.color = GetDirectionColor(cell.room.directionTypes[1]);
            Gizmos.DrawLine(northEast, southEast);

            // South
            Gizmos.color = GetDirectionColor(cell.room.directionTypes[2]);
            Gizmos.DrawLine(southEast, southWest);

            // West
            Gizmos.color = GetDirectionColor(cell.room.directionTypes[3]);
            Gizmos.DrawLine(southWest, northWest);

        }


    }

    private Color GetDirectionColor(DirectionType type)
    {
        switch (type)
        {
            case DirectionType.Wall: return Color.red;
            case DirectionType.Door: return Color.yellow;
            case DirectionType.Hallway: return Color.cyan;
            default: return Color.gray;
        }
    }

    // Method to check for conflicts between adjacent cells based on their possible rooms and direction types. If a conflict is found, it removes the conflicting room from the adjacent cell's possible rooms. If an adjacent cell is left with only one possible room, it collapses that cell as well. If any cell has no possible rooms left, it sets the failed flag to true and logs an error message.
    public void CheckForConflicts()
    {

        bool changed = true; 

        while (changed)
        {

            changed = false;

            // Iterate through all cells to check for conflicts with adjacent cells.
            for (int i = 0; i < cells.Count; i++)
            {

                // Check if the current cell has any possible rooms left. If not, log an error and set the failed flag to true.
                if (cells[i].possibleRooms.Count == 0)
                {
                    Debug.LogError("No possible rooms for cell at index: " + i);
                    failed = true;
                    break;
                }

                // Check for conflicts with the left adjacent cell (if it exists).
                if (i % (int)GRID_SIZE.x != 0)
                {

                    Cells tempCells = cells[i - 1];

                    if (tempCells.possibleRooms.Count == 0)
                    {
                        Debug.LogError("No possible rooms for cell at index: " + (i - 1));
                        failed = true;
                        break;
                    }

                    // Iterate through the possible rooms of the left adjacent cell and check for direction conflicts with the current cell. If a conflict is found, remove the conflicting room from the left adjacent cell's possible rooms.
                    for (int j = 0; j < tempCells.possibleRooms.Count; j++)
                    {

                        bool supported = false;

                        for (int k = 0; k < cells[i].possibleRooms.Count; k++)
                        {
                            if (!tempCells.possibleRooms[j].CheckDirectionConflicts(3, cells[i].possibleRooms[k].directionTypes[3]))
                            {

                                supported = true;

                            }
                        }

                        if (supported == false)
                        {
                            tempCells.possibleRooms.Remove(tempCells.possibleRooms[j]);
                            changed = true;
                            j--;

                        }
                    }

                    if (tempCells.possibleRooms.Count == 0)
                    {
                        failed = true;
                        return;
                    }

                    if (tempCells.possibleRooms.Count == 1)
                    {
                        tempCells.room = tempCells.possibleRooms[0];
                        tempCells.collapsed = true;
                    }
                }

                // Check for conflicts with the right adjacent cell (if it exists).
                if (i % (int)GRID_SIZE.x != (int)GRID_SIZE.x - 1)
                {
                    Cells tempCells = cells[i + 1];

                    if (tempCells.possibleRooms.Count == 0)
                    {
                        Debug.LogError("No possible rooms for cell at index: " + (i + 1));
                        failed = true;
                        break;
                    }

                    // Iterate through the possible rooms of the right adjacent cell and check for direction conflicts with the current cell. If a conflict is found, remove the conflicting room from the right adjacent cell's possible rooms.
                    for (int j = 0; j < tempCells.possibleRooms.Count; j++)
                    {

                        bool supported = false;

                        for (int k = 0; k < cells[i].possibleRooms.Count; k++)
                        {

                            if (!tempCells.possibleRooms[j].CheckDirectionConflicts(1, cells[i].possibleRooms[k].directionTypes[1]))
                            {
                                supported = true;
                            }
                        }

                        if (supported == false)
                        {
                            tempCells.possibleRooms.Remove(tempCells.possibleRooms[j]);
                            changed = true;
                            j--;

                        }

                    }

                    if (tempCells.possibleRooms.Count == 0)
                    {
                        failed = true;
                        return;
                    }

                    if (tempCells.possibleRooms.Count == 1)
                    {
                        tempCells.room = tempCells.possibleRooms[0];
                        tempCells.collapsed = true;

                    }

                }

                // Check for conflicts with the top adjacent cell (if it exists).
                if (i / (int)GRID_SIZE.x != 0)
                {
                    Cells tempCells = cells[i - (int)GRID_SIZE.x];

                    if (tempCells.possibleRooms.Count == 0)
                    {
                        Debug.LogError("No possible rooms for cell at index: " + (i - (int)GRID_SIZE.x));
                        failed = true;
                        break;
                    }

                    // Iterate through the possible rooms of the top adjacent cell and check for direction conflicts with the current cell. If a conflict is found, remove the conflicting room from the top adjacent cell's possible rooms.
                    for (int j = 0; j < tempCells.possibleRooms.Count; j++)
                    {

                        bool supported = false;

                        for (int k = 0; k < cells[i].possibleRooms.Count; k++)
                        {
                            if (!tempCells.possibleRooms[j].CheckDirectionConflicts(0, cells[i].possibleRooms[k].directionTypes[0]))
                            {
                                supported = true;
                            }
                        }

                        if (supported == false)
                        {
                            tempCells.possibleRooms.Remove(tempCells.possibleRooms[j]);
                            changed = true;
                            j--;

                        }

                    }

                    if (tempCells.possibleRooms.Count == 0)
                    {
                        failed = true;
                        return;
                    }

                    if (tempCells.possibleRooms.Count == 1)
                    {
                        tempCells.room = tempCells.possibleRooms[0];
                        tempCells.collapsed = true;

                    }
                }

                // Check for conflicts with the bottom adjacent cell (if it exists).
                if (i / (int)GRID_SIZE.x != (int)GRID_SIZE.y - 1)
                {

                    Cells tempCells = cells[i + (int)GRID_SIZE.x];

                    if (tempCells.possibleRooms.Count == 0)
                    {
                        Debug.LogError("No possible rooms for cell at index: " + (i + (int)GRID_SIZE.x));
                        failed = true;
                        break;
                    }

                    // Iterate through the possible rooms of the bottom adjacent cell and check for direction conflicts with the current cell. If a conflict is found, remove the conflicting room from the bottom adjacent cell's possible rooms.
                    for (int j = 0; j < tempCells.possibleRooms.Count; j++)
                    {

                        bool supported = false;

                        for (int k = 0; k < cells[i].possibleRooms.Count; k++)
                        {
                            if (!tempCells.possibleRooms[j].CheckDirectionConflicts(2, cells[i].possibleRooms[k].directionTypes[2]))
                            {
                                supported = true;
                            }
                        }

                        if (supported == false)
                        {
                            tempCells.possibleRooms.Remove(tempCells.possibleRooms[j]);
                            changed = true; 
                            j--;

                        }

                    }

                    if (tempCells.possibleRooms.Count == 0)
                    {
                        failed = true;
                        return;
                    }

                    if (tempCells.possibleRooms.Count == 1)
                    {
                        tempCells.room = tempCells.possibleRooms[0];
                        tempCells.collapsed = true;

                    }

                }

            }

        }

    }

}
