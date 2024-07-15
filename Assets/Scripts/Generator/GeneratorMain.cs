using FishNet.Connection;
using FishNet.Object;
using FishNet.Managing;
using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.AI.Navigation;
using FishNet.Managing.Server;
using GameKit.Dependencies.Utilities;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine.UIElements;

public class GeneratorMain : NetworkBehaviour
{
    public Room[] generatedRooms;
    public GameObject[,,] objects;
    public Tile[] generatedObjects;
    public int[,,] currentMap;
    public Vector3[,,] currentMapPositions;
    public Vector3[,,] currentMapRotations;
    private List<Bounds> roomBoundsList = new List<Bounds>();
    public LayerMask obstacleMask;


    public override void OnStartClient()
    {
        base.OnStartClient();
        //ConnectedToServer(LocalConnection.ClientId);
    }

    public override void OnStartServer()
    {
        int roomCount = 4; // Set the desired number of rooms

        generatedRooms = new Room[roomCount + 1];

        Room spawningRoom = RoomShapeGenerator.GiveMeRoom(new Vector3(10, 10, 10), new Vector3(0, 0, 0), RoomTheme.Spawn);
        generatedRooms[0] = spawningRoom;
        AddRoomBounds(spawningRoom);

        RoomTheme theme = RoomTheme.Rock;

        Vector3[] sizes = new Vector3[roomCount];
        Vector3[] positions = new Vector3[roomCount];

        Vector3 size = RoomShapeGenerator.GetRandomRoomSize(false);
        Vector3 position = new Vector3(15, 0, 5);

        sizes[0] = size;
        positions[0] = position;

        for(int i = 1; i < roomCount; i++)
        {
            size = RoomShapeGenerator.GetRandomRoomSize(false);
            position = GetRandomPositionForRoom(i);
            sizes[i] = size;
            positions[i] = position;
        }

        for (int i = 0; i < roomCount; i++)
        {
            Room generatedRoom;

            generatedRoom = RoomShapeGenerator.GiveMeRoom(sizes[i], positions[i], theme, i);

            generatedRooms[i + 1] = generatedRoom;

            AddRoomBounds(generatedRoom);
            ObjectPopulator.PopulateObjects(generatedRoom, theme, i, false);
            ItemPopulator.PopulateItems(spawningRoom, RoomTheme.Spawn);
        }

        foreach (Room room in generatedRooms)
        {
            spawnRoom(room);
        }

        ConnectRooms();



        GenerateNavMesh();

        for (int i = 1; i <= roomCount;i++)
        {
            Room room = generatedRooms[i];

            GameObject[] npcs = NPCSpawner.SpawnNPCs(room, generatedObjects, theme, GameDifficulty.Normal, false);

            foreach (GameObject npc in npcs)
            {
                ServerManager.Spawn(npc);
            }
        }

        foreach (Item item in spawningRoom.items)
        {
            if (item != null)
            {
                spawnTile(item);
            }
        }
    }

    private void AddRoomBounds(Room room)
    {
        Bounds roomBounds = new Bounds(room.position, room.size);
        roomBoundsList.Add(roomBounds);
    }

    private bool IsOverlapping(Room room)
    {
        float buffer = 4.0f; // Buffer zone of 4 units
        Bounds newRoomBounds = new Bounds(room.position, room.size + new Vector3(buffer, 0, buffer));
        foreach (Bounds bounds in roomBoundsList)
        {
            Bounds expandedBounds = new Bounds(bounds.center, bounds.size + new Vector3(buffer, 0, buffer));
            if (newRoomBounds.Intersects(expandedBounds))
            {
                return true;
            }
        }
        return false;
    }

    private Vector3 GetRandomPositionForRoom(int i)
    {
        // 1 is in top left quadrant, 2 is top right and 3 is bottom left
        float x = 0.0f;
        float z = 0.0f;

        if (i == 1)
        {
            x = UnityEngine.Random.Range(0, 9); // Adjust range based on buffer
            z = UnityEngine.Random.Range(42, 52);
            return new Vector3(x, 0, z);
        }
        else if (i == 2)
        {
            x = UnityEngine.Random.Range(42, 52); // Adjust range based on buffer
            z = UnityEngine.Random.Range(42, 52);
            return new Vector3(x, 0, z);
        }
        else if (i == 3)
        {
            x = UnityEngine.Random.Range(42, 52); // Adjust range based on buffer
            z = UnityEngine.Random.Range(0, 9);
            return new Vector3(x, 0, z);
        }
        return new Vector3(x, 0, z);
    }
    public void ConnectRooms()
    {
        for( int xx = 0; xx< 2; xx++ ) { 
        for (int i = 0; i < generatedRooms.Length; i++)
        {
            Room room1 = generatedRooms[i];
            Tile closestDoor1 = null;
            Tile closestDoor2 = null;
            float closestDistance = float.MaxValue;

            Room target = room1;
            bool isConnected = false;

            for (int j = 0; j < generatedRooms.Length; j++)
            {
                if (i == j) continue;

                Room room2 = generatedRooms[j];

                foreach (Tile door1 in room1.doors)
                {
                    foreach (Tile door2 in room2.doors)
                    {
                        if(door1.ProcessingState == ProcessingState.ConnectedDoor || door2.ProcessingState == ProcessingState.ConnectedDoor)
                        {
                            continue;
                        }
                        float distance = Vector3.Distance(door1.position + room1.position * 2, door2.position + room2.position * 2);
                        if (distance < closestDistance)
                        {
                            target = room2;
                            closestDistance = distance;
                            closestDoor1 = door1;
                            closestDoor2 = door2;
                            isConnected = true;
                            
                        }
                       }
                }
            }

            if (closestDoor1 != null && closestDoor2 != null && isConnected)
            {
                closestDoor1.ProcessingState = ProcessingState.ConnectedDoor;
                closestDoor2.ProcessingState = ProcessingState.ConnectedDoor;
                CreateCorridor(closestDoor1.position + room1.position * 2, closestDoor2.position + target.position * 2);
            }
            else
            {
                // Handle unconnected rooms
                Debug.LogWarning("Room " + room1.position + " could not be connected.");
            }
        }
        }



    }

    public void CreateCorridor(Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        int corridorLength = Mathf.CeilToInt(distance / 2); // Assuming each tile is 2 units

        List<Vector3> corridorPositions = new List<Vector3>();

        Vector3 currentPosition = start;
        corridorPositions.Add(currentPosition);

        for (int i = 1; i <= corridorLength; i++)
        {
            // Move in the direction of the corridor
            currentPosition += direction * 2;

            // Check for collisions
            Collider[] colliders = Physics.OverlapSphere(currentPosition, 0.5f, obstacleMask);
            if (colliders.Length > 0)
            {
                // If collision detected, try random directions until a clear path is found
                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere.normalized;
                foreach (Vector3 dir in GenerateRandomDirections())
                {
                    if (!Physics.CheckBox(currentPosition + dir, Vector3.one * 0.5f, Quaternion.identity, obstacleMask))
                    {
                        currentPosition += dir;
                        break;
                    }
                }
            }

            // Add the position to the corridor positions list
            corridorPositions.Add(currentPosition);
        }

        // Spawn corridor tiles
        for (int i = 0; i < corridorPositions.Count; i++)
        {
            Vector3 position = corridorPositions[i];

            Tile corridorTile = TileDictionary.GetTile(1);

            corridorTile.position = new Vector3(position.x, 0, position.z); // set y to zero

            spawnTile(corridorTile);

         

            Vector3 left = Quaternion.Euler(0, 90, 0) * direction;
            Vector3 right = Quaternion.Euler(0, -90, 0) * direction;

            Tile leftBarrier = TileDictionary.GetTile(17);
            leftBarrier.position = position + left * 2;
            leftBarrier.rotation = Quaternion.LookRotation(left);

            Tile rightBarrier = TileDictionary.GetTile(17);
            rightBarrier.position = position + right * 2;
            rightBarrier.rotation = Quaternion.LookRotation(right);

            

            if (i != 0 || i != 1)
            {
                spawnTile(rightBarrier);

            }

            if (i != corridorLength || i!= corridorLength-1)
            {
                spawnTile(leftBarrier);
                
            }

        }
    }

    private List<Vector3> GenerateRandomDirections()
    {
        List<Vector3> directions = new List<Vector3>();
        directions.Add(Vector3.forward);
        directions.Add(Vector3.back);
        directions.Add(Vector3.left);
        directions.Add(Vector3.right);
        return directions;
    }

    public void GenerateNavMesh()
    {
        NavMeshSurface surface = GetComponent<NavMeshSurface>();
        surface.RemoveData();
        surface.BuildNavMesh();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ConnectedToServer(int clientId)
    {
        // nothing
    }

    public void spawnTile(Tile tile, Vector3 offset = new Vector3())
    {
        GameObject obj = Instantiate(tile.prefab, tile.position + (offset * 2), tile.rotation);
        if (obj.GetComponent<NetworkObject>() == null)
            obj.AddComponent<NetworkObject>();
        obj.transform.parent = this.transform;
        ServerManager.Spawn(obj);
    }

    public void spawnRoom(Room room)
    {
        foreach (Tile tile in room.tiles)
        {
            if (tile != null)
            {
                spawnTile(tile, room.position);
            }
        }
        if (room.objects != null)
        {
            foreach (Tile tile in room.objects)
            {
                if (tile != null)
                {
                    spawnTile(tile);
                }
            }
        }
    }

    public void SpawnDungeonObjects(int[] dungeonObjects, Vector3[] positions)
    {
        for (int i = 0; i < dungeonObjects.Length; i++)
        {
            GameObject newObj = Instantiate(ObjectDictionary.GetObject(dungeonObjects[i]).prefab, positions[i], Quaternion.identity);
            newObj.AddComponent<MeshCollider>();
            newObj.transform.parent = this.transform;
        }
    }

    public struct DungeonObjectsPaylod
    {
        public int[] dungeonObjects;
        public float[] positions_x;
        public float[] positions_y;
        public float[] positions_z;
        public float[] rotations_x;
        public float[] rotations_y;
        public float[] rotations_z;
    }

    public struct RoomPaylod
    {
        public int[] map;
        public int[] shape;
        public Vector3[] positions;
        public Vector3[] rotations;
    }
}