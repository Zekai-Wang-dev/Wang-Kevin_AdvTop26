using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

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

public class WFCGenerator : MonoBehaviour
{

    public Vector2 GRID_SIZE = new Vector2(10, 10);

    public Vector2 CELL_SIZE = new Vector2(10, 10);

    public Vector2 GRID_OFFSET = new Vector2(0, 0);

    public List<Rooms> possibleRooms = new List<Rooms>();

    private List<Cells> cells = new List<Cells>();
    public bool failed = false;

    public bool generationComplete = false;

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

    public void SelectNextRoom()
    {

        int lowestEntropy = possibleRooms.Count;
        int cellIndex = -1; 

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

        if (cellIndex != -1)
        {
            int randomRoomIndex = Random.Range(0, cells[cellIndex].possibleRooms.Count);
            cells[cellIndex].room = cells[cellIndex].possibleRooms[randomRoomIndex];
            cells[cellIndex].collapsed = true;
            CheckForConflicts();
        }
        else
        {
            int randomCellIndex = Random.Range(0, cells.Count);
            int randomRoomIndex = Random.Range(0, cells[randomCellIndex].possibleRooms.Count);
            cells[randomCellIndex].room = cells[randomCellIndex].possibleRooms[randomRoomIndex];
            cells[randomCellIndex].collapsed = true;
            CheckForConflicts();
        }

    }

    public void CheckForConflicts()
    {

        for (int i = 0; i < cells.Count; i++)
        {

            if (cells[i].collapsed)
            {
                continue;
            }

            if (cells[i].possibleRooms.Count == 0)
            {
                Debug.LogError("No possible rooms for cell at index: " + i);
                failed = true;
                break; 
            }

            if (i % (int)GRID_SIZE.x != 0)
            {

                Cells tempCells = cells[i - 1];

                if (tempCells.possibleRooms.Count == 0)
                {
                    Debug.LogError("No possible rooms for cell at index: " + (i - 1));
                    failed = true;
                    break;
                }

                for (int j = 0; j < tempCells.possibleRooms.Count; j++)
                {
                    if (tempCells.possibleRooms[j].CheckDirectionConflicts(i - 1, tempCells.possibleRooms[j].directionTypes[3]))
                    {
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
            if (i % (int)GRID_SIZE.x != (int)GRID_SIZE.x - 1)
            {
                Cells tempCells = cells[i + 1];

                if (tempCells.possibleRooms.Count == 0)
                {
                    Debug.LogError("No possible rooms for cell at index: " + (i - 1));
                    failed = true;
                    break;
                }

                for (int j = 0; j < tempCells.possibleRooms.Count; j++)
                {
                    if (tempCells.possibleRooms[j].CheckDirectionConflicts(i + 1, tempCells.possibleRooms[j].directionTypes[2]))
                    {
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
            if (i / (int)GRID_SIZE.x != 0)
            {
                Cells tempCells = cells[i - (int)GRID_SIZE.x];

                if (tempCells.possibleRooms.Count == 0)
                {
                    Debug.LogError("No possible rooms for cell at index: " + (i - 1));
                    failed = true;
                    break;
                }

                for (int j = 0; j < tempCells.possibleRooms.Count; j++)
                {
                    if (tempCells.possibleRooms[j].CheckDirectionConflicts(i - (int)GRID_SIZE.x, tempCells.possibleRooms[j].directionTypes[0]))
                    {
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
            if (i / (int)GRID_SIZE.x != (int)GRID_SIZE.y - 1)
            {

                Cells tempCells = cells[i + (int)GRID_SIZE.x];

                if (tempCells.possibleRooms.Count == 0)
                {
                    Debug.LogError("No possible rooms for cell at index: " + (i - 1));
                    failed = true;
                    break;
                }

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
