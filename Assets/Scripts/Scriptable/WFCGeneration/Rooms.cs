using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rooms", menuName = "Scriptable Objects/Rooms")]

//Rooms class scriptable object that holds the direction types for each room and provides methods to check for direction conflicts and rotate the room.
public class Rooms : ScriptableObject
{

    // Direction types for the room in the order of North, East, South, West
    [SerializeField]
    public DirectionType[] directionTypes = new DirectionType[4] { DirectionType.None, DirectionType.None, DirectionType.None, DirectionType.None };

    [SerializeField]
    public GameObject prefab; // Prefab for the room

    // Method to check for direction conflicts between the current room and the adjacent room based on the direction index and direction type.
    public bool CheckDirectionConflicts(int directionIndex, DirectionType directionType)
    {

        bool conflict = false;

        int oppositeDirectionIndex = (directionIndex + 2) % 4;

        // Check if the opposite direction of the adjacent room has a conflicting direction type with the current room's direction type.
        if (directionTypes[oppositeDirectionIndex] != DirectionType.None)
        {

            if (directionTypes[oppositeDirectionIndex] == DirectionType.Door && directionType == DirectionType.Wall)
            {
                conflict = true;
            }
            else if (directionTypes[oppositeDirectionIndex] == DirectionType.Wall && directionType == DirectionType.Door)
            {
                conflict = true;
            }
            else if (directionTypes[oppositeDirectionIndex] == DirectionType.Hallway && directionType == DirectionType.Wall)
            {
                conflict = true;
            }
            else if (directionTypes[oppositeDirectionIndex] == DirectionType.Wall && directionType == DirectionType.Hallway)
            {
                conflict = true;
            }

        }

        return conflict;

    }

    // Method to rotate the room's direction types either clockwise or counterclockwise based on the specified direction.
    public void RotateRoom(RotateDirection direction)
    {

        switch (direction)
        {
            case RotateDirection.Clockwise:
                DirectionType temp = directionTypes[3];
                directionTypes[3] = directionTypes[2];
                directionTypes[2] = directionTypes[1];
                directionTypes[1] = directionTypes[0];
                directionTypes[0] = temp;
                break;

            case RotateDirection.CounterClockwise:
                DirectionType temp2 = directionTypes[0];
                directionTypes[0] = directionTypes[1];
                directionTypes[1] = directionTypes[2];
                directionTypes[2] = directionTypes[3];
                directionTypes[3] = temp2;
                break;

        }

    }

}

public enum DirectionType
{
    None,
    Door,
    Wall,
    Hallway

}

public enum RotateDirection
{
    Clockwise,
    CounterClockwise
}