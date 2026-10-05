using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rooms", menuName = "Scriptable Objects/Rooms")]
public class Rooms : ScriptableObject
{

    DirectionType[] directionTypes = new DirectionType[4] { DirectionType.None, DirectionType.None, DirectionType.None, DirectionType.None };

    public DirectionType CheckDirectionConflicts(int directionIndex, DirectionType directionType)
    {

        int oppositeDirectionIndex = (directionIndex + 2) % 4;

        DirectionType conflictType = DirectionType.None; 

        if (directionTypes[oppositeDirectionIndex] != DirectionType.None)
        {

            if (directionTypes[oppositeDirectionIndex] == DirectionType.Door && directionType == DirectionType.Wall)
            {
                conflictType = directionTypes[oppositeDirectionIndex];
            }

        }

        return conflictType; 
    }

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