using FishNet.Connection;
using FishNet.Object;
using FishNet.Managing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NPCSpawner: NetworkBehaviour
{
    static GameObject gameManager = GameObject.Find("GameManager");
    public static GameObject[] SpawnNPCs(Room room, Tile[] objects, RoomTheme theme = RoomTheme.Rock, GameDifficulty difficulty = GameDifficulty.Normal, bool isBossRoom = false)
    {
        GameObject[] npcs = new GameObject[0];

        if (theme == RoomTheme.Rock)
        {
            // Append the npcs array with the npcs spawned in the room
            npcs = npcs.Concat(SpawnSkeletonHorde(room, objects, difficulty)).Where(obj => obj != null).ToArray();

        }

        return npcs;
    }

    private static GameObject[] SpawnSkeletonHorde(Room room, Tile[] objects, GameDifficulty difficulty)
    {
        GameObject[] npcs = new GameObject[0];

        int numSkeletons = 0;
        switch (difficulty)
        {
            case GameDifficulty.Easy:
                numSkeletons = 5;
                break;
            case GameDifficulty.Normal:
                numSkeletons = 10;
                break;
            case GameDifficulty.Hard:
                numSkeletons = 12;
                break;
        }

        for (int i = 0; i < numSkeletons; i++)
        {
            Vector3 position = GetValidPosition(room, objects);


            // Spawn and append
            npcs = npcs.Concat(new GameObject[] { NPCSpawn(position, NPCDictionary.GetNPC(1)) }).Where(obj => obj != null).ToArray();
        }

        return npcs;
    }

    private static GameObject NPCSpawn(Vector3 pos, NPC npc)
    {
        // increase NPC count in game manager

        // Get the gamemanager object
       
        if (gameManager != null)
        {
            gameManager.GetComponent<GameManager>().IncrementNPC(npc);
        }
        else
        {
            Debug.Log("Game Manager not found");
        }


        GameObject go = Instantiate(npc.prefab, pos, Quaternion.identity);
        go.transform.localScale = npc.size;


        return go;
    }

    public static Vector3 GetValidPosition(Room room, Tile[] objects)
    {
        // A valid position is there is no object in vicinity

        Vector3 offset = new Vector3(room.position.x, room.position.y, room.position.z);

        Vector3 position = new Vector3(Random.Range((offset.x * 2) + 3.5f, (offset.x * 2) + (room.size.x * 2.0f) - 3.5f), (offset.y * 2) + 2.15f, Random.Range((offset.z * 2) + 3.5f, (offset.z * 2) + (room.size.z * 2.0f) - 3.5f));

        // Now we check if position is invalid due a decorative floor tile or other object, we try to move towards the oppsite direction until we can place the object

        bool checking = true;

        while (checking)
        {
            bool valid = true;
            Vector3 direction = new Vector3(0, 0, 0);


            foreach (Tile obj in objects)
            {

                if (Vector3.Distance(obj.position, position) < 1.5f)
                {
                    valid = false;
                    direction = obj.position - position;
                    break;
                }
            }

            if (!valid)
            {
                position = position - direction;
            }
            else
            {

                checking = false;
            }
        }


        return position;
    }
}
