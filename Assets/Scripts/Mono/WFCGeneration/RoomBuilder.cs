using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour
{

    public GameObject groundPrefab;
    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public GameObject hallwayPrefab;

    public WFCGenerator wfcGenerator; 

    private GameObject getPrefabFromType(DirectionType type)
    {
        switch (type)
        {
            case DirectionType.Wall:
                return wallPrefab;
            case DirectionType.Door:
                return doorPrefab;
            case DirectionType.Hallway:
                return hallwayPrefab;
            default:
                return null;
        }
    }

    public void BuildConnector(Rooms room1, Rooms room2, Transform roomTrans, Transform room2Trans, int directionIndex)
    {

        GameObject prefab = wfcGenerator.TryGetConnector(room1, room2, directionIndex);

        if (directionIndex < 0 || directionIndex > 3)
        {
            Debug.LogError("Invalid direction index. Must be between 0 and 3.");
            return;
        }

        if (directionIndex == 0) // North
        {
            Vector3 position = (roomTrans.position + room2Trans.position) / 2 + new Vector3(0, 2.5f, roomTrans.localScale.z / 2);
            Instantiate(prefab, position, Quaternion.Euler(0, 90, 0));
        }
        else if (directionIndex == 1) // East
        {
            Vector3 position = (roomTrans.position + room2Trans.position) / 2 + new Vector3(roomTrans.localScale.x / 2, 2.5f, 0);
            Instantiate(prefab, position, Quaternion.identity);
        }
        else if (directionIndex == 2) // South
        {
            Vector3 position = (roomTrans.position + room2Trans.position) / 2 + new Vector3(0, 2.5f, -roomTrans.localScale.z / 2);
            Instantiate(prefab, position, Quaternion.Euler(0, 270, 0));
        }
        else if (directionIndex == 3) // West
        {
            Vector3 position = (roomTrans.position + room2Trans.position) / 2 + new Vector3(-roomTrans.localScale.x / 2, 2.5f, 0);
            Instantiate(prefab, position, Quaternion.Euler(0, 180, 0));
        }

    }

    public void BuildRoom(Rooms room, Vector3 position)
    {
        // Instantiate the ground prefab at the specified position
        GameObject ground = Instantiate(groundPrefab, position, Quaternion.identity);
        Transform groundTrans = ground.transform;

        Rooms EastRoom = wfcGenerator.GetAdjacentRoom(room, 1);
        Rooms NorthRoom = wfcGenerator.GetAdjacentRoom(room, 0);

        // Loop through each direction type and instantiate the corresponding prefab
        for (int i = 0; i < room.directionTypes.Length; i++)
        {
            DirectionType type = room.directionTypes[i];
            GameObject prefab = getPrefabFromType(type);

            if (prefab != null)
            {
                Vector3 offset = Vector3.zero;
                Vector3 rotation = Vector3.zero;

                switch (i)
                {
                    case 0: // North
                        offset = new Vector3(0, 2.5f,groundTrans.localScale.z / 2);
                        rotation = new Vector3(0, 90, 0);
                        break;
                    case 1: // East
                        offset = new Vector3(groundTrans.localScale.x / 2, 2.5f, 0);
                        rotation = new Vector3(0, 0, 0);
                        break;
                    case 2: // South
                        offset = new Vector3(0, 2.5f, -groundTrans.localScale.z / 2);
                        rotation = new Vector3(0, 270, 0);
                        break;
                    case 3: // West
                        offset = new Vector3(-groundTrans.localScale.x / 2, 2.5f, 0);
                        rotation = new Vector3(0, 180, 0);
                        break;
                }
                
                Instantiate(prefab, position + offset, Quaternion.Euler(rotation));
            }
        }
    }

}
