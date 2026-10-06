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

    public void SelectFirstRoom()
    {

        for (int i = 0; i < cells.Count; i++)
        {
            if (!cells[i].collapsed)
            {
                int randomIndex = Random.Range(0, cells[i].possibleRooms.Count);
                cells[i].room = cells[i].possibleRooms[randomIndex];
                cells[i].collapsed = true;
            }
        }

    }

    public void CheckForConflict(int cellIndex)
    {

        if (cells[cellIndex].collapsed)
        {
            return;
        }

        if (cells[cellIndex].possibleRooms.Count == 0)
        {
            Debug.LogError("No possible rooms for cell at index: " + cellIndex);
            return;
        }

        if (cellIndex % (int)GRID_SIZE.x != 0)
        {
            
            Cells tempCells = cells[cellIndex - 1];

            if (tempCells.room.CheckDirectionConflicts(cellIndex - 1, tempCells.room.directionTypes[3]))
            {
                cells[cellIndex].possibleRooms.Remove(tempCells.room);

                if (cells[cellIndex].possibleRooms.Count == 1)
                {
                    cells[cellIndex].room = cells[cellIndex].possibleRooms[0];
                    cells[cellIndex].collapsed = true;

                }

            }

        }

    }

}
