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

    // Method to select the next cell to collapse based on the lowest entropy (i.e., the cell with the fewest possible rooms). If a cell with the lowest entropy is found, it randomly selects one of its possible rooms, collapses the cell, and checks for conflicts with adjacent cells. If no uncollapsed cells are found, it randomly selects a cell and collapses it.
    public void SelectNextRoom()
    {

        int lowestEntropy = possibleRooms.Count;
        int cellIndex = -1;

        // Iterate through all cells to find the one with the lowest entropy (fewest possible rooms) that is not yet collapsed.
        for (int i = 0; i < cells.Count; i++)
        {
            if (!cells[i].collapsed)
            {

                if (cells[i].possibleRooms.Count < lowestEntropy)
                {
                    lowestEntropy = cells[i].possibleRooms.Count;
                    cellIndex = i;
                    Debug.Log("Lowest entropy found at cell index: " + cellIndex + " with entropy: " + lowestEntropy);
                }

            }
        }

        // If a cell with the lowest entropy is found, randomly select one of its possible rooms, collapse the cell, and check for conflicts with adjacent cells.
        if (cellIndex != -1)
        {
            int randomRoomIndex = Random.Range(0, cells[cellIndex].possibleRooms.Count);
            cells[cellIndex].room = cells[cellIndex].possibleRooms[randomRoomIndex];
            cells[cellIndex].collapsed = true;
            cells[cellIndex].possibleRooms = new List<Rooms> { cells[cellIndex].room };
            CheckForConflicts();
        }
        else
        {
            int randomCellIndex = Random.Range(0, cells.Count);
            int randomRoomIndex = Random.Range(0, cells[randomCellIndex].possibleRooms.Count);
            cells[randomCellIndex].room = cells[randomCellIndex].possibleRooms[randomRoomIndex];
            cells[randomCellIndex].collapsed = true;
            cells[randomCellIndex].possibleRooms = new List<Rooms> { cells[randomCellIndex].room };
            CheckForConflicts();
        }

    }

    // Method to check for conflicts between adjacent cells based on their possible rooms and direction types. If a conflict is found, it removes the conflicting room from the adjacent cell's possible rooms. If an adjacent cell is left with only one possible room, it collapses that cell as well. If any cell has no possible rooms left, it sets the failed flag to true and logs an error message.
    public void CheckForConflicts()
    {

        // Iterate through all cells to check for conflicts with adjacent cells.
        for (int i = 0; i < cells.Count; i++)
        {

            // Skip the cell if it is already collapsed, as we only need to check for conflicts in uncollapsed cells.
            if (cells[i].collapsed)
            {
                continue;
            }

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
                    if (tempCells.possibleRooms[j].CheckDirectionConflicts(i - 1, tempCells.possibleRooms[j].directionTypes[3]))
                    {

                        if (cells[i - 1].collapsed)
                        {
                            Debug.LogError("Cell at index: " + (i - 1) + " is already collapsed. Cannot remove possible room.");
                            failed = true;
                            break;
                        }

                        cells[i - 1].possibleRooms.Remove(tempCells.possibleRooms[j]);
                        j--;

                        if (cells[i - 1].possibleRooms.Count == 1)
                        {
                            cells[i - 1].room = cells[i - 1].possibleRooms[0];
                            cells[i - 1].collapsed = true;

                        }
                    }
                }

            }

            // Check for conflicts with the right adjacent cell (if it exists).
            if (i % (int)GRID_SIZE.x != (int)GRID_SIZE.x - 1)
            {
                Cells tempCells = cells[i + 1];

                if (tempCells.possibleRooms.Count == 0)
                {
                    Debug.LogError("No possible rooms for cell at index: " + (i - 1));
                    failed = true;
                    break;
                }

                // Iterate through the possible rooms of the right adjacent cell and check for direction conflicts with the current cell. If a conflict is found, remove the conflicting room from the right adjacent cell's possible rooms.
                for (int j = 0; j < tempCells.possibleRooms.Count; j++)
                {
                    if (tempCells.possibleRooms[j].CheckDirectionConflicts(i + 1, tempCells.possibleRooms[j].directionTypes[2]))
                    {
                        if (cells[i + 1].collapsed)
                        {
                            Debug.LogError("Cell at index: " + (i + 1) + " is already collapsed. Cannot remove possible room.");
                            failed = true;
                            break;
                        }

                        cells[i + 1].possibleRooms.Remove(tempCells.possibleRooms[j]);
                        j--;

                        if (cells[i + 1].possibleRooms.Count == 1)
                        {
                            cells[i + 1].room = cells[i + 1].possibleRooms[0];
                            cells[i + 1].collapsed = true;
                        }
                    }
                }

            }

            // Check for conflicts with the top adjacent cell (if it exists).
            if (i / (int)GRID_SIZE.x != 0)
            {
                Cells tempCells = cells[i - (int)GRID_SIZE.x];

                if (tempCells.possibleRooms.Count == 0)
                {
                    Debug.LogError("No possible rooms for cell at index: " + (i - 1));
                    failed = true;
                    break;
                }

                // Iterate through the possible rooms of the top adjacent cell and check for direction conflicts with the current cell. If a conflict is found, remove the conflicting room from the top adjacent cell's possible rooms.
                for (int j = 0; j < tempCells.possibleRooms.Count; j++)
                {
                    if (tempCells.possibleRooms[j].CheckDirectionConflicts(i - (int)GRID_SIZE.x, tempCells.possibleRooms[j].directionTypes[0]))
                    {
                        if (cells[i - 1].collapsed)
                        {
                            Debug.LogError("Cell at index: " + (i - 1) + " is already collapsed. Cannot remove possible room.");
                            failed = true;
                            break;
                        }

                        cells[i - (int)GRID_SIZE.x].possibleRooms.Remove(tempCells.possibleRooms[j]);
                        j--;

                        if (cells[i - (int)GRID_SIZE.x].possibleRooms.Count == 1)
                        {
                            cells[i - (int)GRID_SIZE.x].room = cells[i - (int)GRID_SIZE.x].possibleRooms[0];
                            cells[i - (int)GRID_SIZE.x].collapsed = true;
                        }
                    }
                }

            }

            // Check for conflicts with the bottom adjacent cell (if it exists).
            if (i / (int)GRID_SIZE.x != (int)GRID_SIZE.y - 1)
            {

                Cells tempCells = cells[i + (int)GRID_SIZE.x];

                if (tempCells.possibleRooms.Count == 0)
                {
                    Debug.LogError("No possible rooms for cell at index: " + (i - 1));
                    failed = true;
                    break;
                }

                // Iterate through the possible rooms of the bottom adjacent cell and check for direction conflicts with the current cell. If a conflict is found, remove the conflicting room from the bottom adjacent cell's possible rooms.
                for (int j = 0; j < tempCells.possibleRooms.Count; j++)
                {
                    if (tempCells.possibleRooms[j].CheckDirectionConflicts(i + (int)GRID_SIZE.x, tempCells.possibleRooms[j].directionTypes[1]))
                    {
                        cells[i + (int)GRID_SIZE.x].possibleRooms.Remove(tempCells.possibleRooms[j]);
                        j--;
                        if (cells[i + (int)GRID_SIZE.x].possibleRooms.Count == 1)
                        {
                            cells[i + (int)GRID_SIZE.x].room = cells[i + (int)GRID_SIZE.x].possibleRooms[0];
                            cells[i + (int)GRID_SIZE.x].collapsed = true;
                        }
                    }
                }
   
            }

        }

    }

}
