using UnityEngine;

public class RoomBuilder : MonoBehaviour
{

    public GameObject groundPrefab;
    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public GameObject hallwayPrefab; 


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

    public void BuildRoom(Rooms room, Vector3 position)
    {
        // Instantiate the ground prefab at the specified position
        Instantiate(groundPrefab, position, Quaternion.identity);

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
                        offset = new Vector3(0, 0, 2.5f);
                        rotation = new Vector3(0, 0, 0);
                        break;
                    case 1: // East
                        offset = new Vector3(2.5f, 0, 0);
                        rotation = new Vector3(0, 90, 0);
                        break;
                    case 2: // South
                        offset = new Vector3(0, 0, -2.5f);
                        rotation = new Vector3(0, 180, 0);
                        break;
                    case 3: // West
                        offset = new Vector3(-2.5f, 0, 0);
                        rotation = new Vector3(0, 270, 0);
                        break;
                }
                Instantiate(prefab, position + offset, Quaternion.Euler(rotation));
            }
        }
    }



}
