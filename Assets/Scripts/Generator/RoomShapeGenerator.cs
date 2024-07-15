using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static SlimUI.ModernMenu.UIMenuManager;

public static class RoomShapeGenerator
{
    public static Vector3 GetRandomRoomSize(bool isBalcon)
    {
        int y = 10;
        if (isBalcon)
        {
            y = y+6;
        }

        int x = Random.Range(7, 15)*2;
        int z = Random.Range(7, 15)*2;

        return new Vector3(x, y, z);
    }
    public static Room GiveMeRoom(Vector3 size, Vector3 position, RoomTheme theme, int roomCount = -1)
    {
        Room room = ScriptableObject.CreateInstance<Room>();
        room.SetData(position, size);


        if (theme == RoomTheme.Rock)
        {
           PopulateFloor(room, theme);
           PopulateCarpet(room, theme);
            if(roomCount == 0)
            {
                PopulateWall(room, theme, roomCount, true);
            }
            else
            {
                PopulateWall(room, theme, roomCount);
            }

        }else if (theme == RoomTheme.Spawn)
        {
            PopulateFloor(room, theme);
            PopulateCarpet(room, theme);
            PopulateWall(room, theme, roomCount);
        }
        

    
        return room;
    }

    public static void PopulateWall(Room room, RoomTheme wallType, int roomCount = -1, bool addSpawnDoor = false)
    {
        Vector2 size = new Vector2(room.size.x, room.size.z);

        // number 10 is wall, number 11 is corner
        // We must place edges as 10 and corners as 11

        if (wallType == RoomTheme.Rock)
        {

            // Add random amount of door to random places
            int area = (int)(size.x * size.y);
            int doorCount = 2;

            /*
            if (area < 300)
            {
                doorCount = 2;
            }
            else if (area <= 600)
            {
                doorCount = Random.Range(2, 3);
            }
            else if (area <= 800)
            {
                doorCount = Random.Range(2, 4);
            }
            else
            {
                doorCount = Random.Range(3, 4);
            }
            */


            (Vector3, int)[] doorPositions = new (Vector3, int)[doorCount];
            Vector3[] skipPositions = new Vector3[0];

            Vector2 padding = new Vector2(4, 4);

            // room no 0 has a door on the right side and top, room no 1 has a door on the right side and bottom, room no 2 has a door on the left side and bottom, room no 3 has a door on the left side and top
            // door count is irrelevant it is always 2

            if (roomCount == 0)
            {
                doorPositions[0] = GetRandomPositionOnEdge(padding, room, doorPositions, 1);
                doorPositions[1] = GetRandomPositionOnEdge(padding, room, doorPositions, 3);
            }
            else if (roomCount == 1)
            {
                doorPositions[0] = GetRandomPositionOnEdge(padding, room, doorPositions, 1);
                doorPositions[1] = GetRandomPositionOnEdge(padding, room, doorPositions, 2);
            }
            else if (roomCount == 2)
            {
                doorPositions[0] = GetRandomPositionOnEdge(padding, room, doorPositions, 0);
                doorPositions[1] = GetRandomPositionOnEdge(padding, room, doorPositions, 2);
            }
            else if (roomCount == 3)
            {
                doorPositions[0] = GetRandomPositionOnEdge(padding, room, doorPositions, 0);
                doorPositions[1] = GetRandomPositionOnEdge(padding, room, doorPositions, 3);
            }


            if (addSpawnDoor)
            {
                doorPositions = doorPositions.Concat(new (Vector3, int)[] { (new Vector3(0, 1, 2), 0) }).ToArray();
            }


            for (int i = 0; i < doorPositions.Length; i++)
            {
                Vector3 position = doorPositions[i].Item1;
                int side = doorPositions[i].Item2;

                Tile doorTile = TileDictionary.GetTile(12);

                if (side == 0)
                {
                    doorTile.SetRotation(Quaternion.Euler(0,90,0));
                    room.SetTile(new Vector3((position.x), 1, (position.z) + 0.5f), doorTile);
                    skipPositions = skipPositions.Concat(new Vector3[] { new Vector3(position.x, 1, position.z + 1) }).ToArray();
                    
                }
                else if (side == 1)
                {
                    doorTile.SetRotation(Quaternion.Euler(0, 90, 0));
                    room.SetTile(new Vector3((position.x), 1, (position.z) + 0.5f), doorTile);
                    skipPositions = skipPositions.Concat(new Vector3[] { new Vector3(position.x, 1, position.z + 1) }).ToArray();
                }
                else if (side == 2)
                {
                    doorTile.SetRotation(Quaternion.Euler(0, 0, 0));
                    room.SetTile(new Vector3((position.x) - 0.5f, 1, (position.z)), doorTile);
                    skipPositions = skipPositions.Concat(new Vector3[] { new Vector3(position.x - 1, 1, position.z) }).ToArray();
                   
                }
                else if (side == 3)
                {
                    doorTile.SetRotation(Quaternion.Euler(0, 0, 0));
                    room.SetTile(new Vector3((position.x) - 0.5f, 1, (position.z)), doorTile);
                    skipPositions = skipPositions.Concat(new Vector3[] { new Vector3(position.x - 1, 1, position.z) }).ToArray();
                }
            }

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    if (x == 0 || x == size.x - 1 || y == 0 || y == size.y - 1)
                    {
                        //We should skip the corners

                        if ((x == 0 && y == 0) || (x == 0 && y == size.y - 1) || (x == size.x - 1 && y == 0) || (x == size.x - 1 && y == size.y - 1))
                        {
                            continue;
                        }

                        // Skip door positions

                        if (doorPositions.Any(door => door.Item1.x == x && door.Item1.z == y))
                        {
                            continue;
                        }

                        if (skipPositions.Any(skip => skip.x == x && skip.z == y))
                        {
                            continue;
                        }

                        Tile tile = TileDictionary.GetTile(10);

                        
                        if(x == 0 || x == size.x - 1)
                        {
                            tile.SetRotation(Quaternion.Euler(0, 90, 0));
                            room.SetTile(new Vector3(x, 1, (y - 0.5f)), tile);
                        }
                        else if (y == 0 || y == size.y - 1)
                        {
                            tile.SetRotation(Quaternion.Euler(0, 0, 0));
                            room.SetTile(new Vector3(x + 0.5f, 1, y), tile);
                        }
                        else
                        {
                            tile.SetRotation(Quaternion.Euler(0, 0, 0));
                            room.SetTile(new Vector3(x, 1, y), tile);
                        }
                    }
                }
            }

            room.SetTile(0, 1, 0, TileDictionary.GetTile(11));
            room.SetTile(new Vector3(0, 1, ((size.y - 1))), TileDictionary.GetTile(11).SetRotation(Quaternion.Euler(0, 90, 0)));
            room.SetTile(new Vector3((size.x - 1), 1, 0), TileDictionary.GetTile(11).SetRotation(Quaternion.Euler(0, -90, 0)));
            room.SetTile(new Vector3((size.x - 1), 1, (size.y - 1)), TileDictionary.GetTile(11).SetRotation(Quaternion.Euler(0, 180, 0)));

            
        }else if(wallType == RoomTheme.Spawn)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    if (x == 0 || x == size.x - 1 || y == 0 || y == size.y - 1)
                    {
                        if ((x == 0 && y == 0) || (x == 0 && y == size.y - 1) || (x == size.x - 1 && y == 0) || (x == size.x - 1 && y == size.y - 1))
                        {
                            continue;
                        }

                        // Also skip spawn door 0, 2

                        if ((x == size.x-1 && y == (size.y - 1)) || (x == size.x - 1 && y == (size.y - 1)-1))
                        {
                            continue;
                        }

                        room.SetTile(x, 2, y, TileDictionary.GetTile(10));
                        if (x == 0 || x == size.x - 1)
                        {
                            room.GetTile(x, 2, y).rotation = Quaternion.Euler(0, 90, 0);
                            room.GetTile(x, 2, y).position = new Vector3(x * 2, 2, (y * 2) - 1);
                        }
                        else if (y == 0 || y == size.y - 1)
                        {
                            room.GetTile(x, 2, y).rotation = Quaternion.Euler(0, 0, 0);
                            room.GetTile(x, 2, y).position = new Vector3(x * 2 + 1, 2, (y * 2));
                        }
                        else
                        {
                            room.GetTile(x, 2, y).rotation = Quaternion.Euler(0, 0, 0);
                            room.GetTile(x, 2, y).position = new Vector3(x * 2, 2, (y * 2));
                        }
                    }
                }
            }

            room.SetTile(new Vector3(0, 1, 0), TileDictionary.GetTile(11));
            room.SetTile(new Vector3(0, 1, ((size.y - 1))), TileDictionary.GetTile(11).SetRotation(Quaternion.Euler(0, 90, 0)));
            room.SetTile(new Vector3((size.x - 1), 1, 0), TileDictionary.GetTile(11).SetRotation(Quaternion.Euler(0, -90, 0)));
            room.SetTile(new Vector3((size.x - 1), 1, (size.y - 1)), TileDictionary.GetTile(11).SetRotation(Quaternion.Euler(0, 180, 0)));

            room.SetTile(new Vector3((size.x - 1), 1, (size.y - 1)-1), TileDictionary.GetTile(12).SetRotation(Quaternion.Euler(0, 90, 0)));
        }
    }

    public static (Vector3, int) GetRandomPositionOnEdge(Vector2 padding, Room room, (Vector3, int)[] doorPositions, int side)
    {
        Vector2 size = new(room.size.x, room.size.z);
        int x = 0;
        int y = 0;
       
        bool thereAlreadyIsDoor = true;

        while (thereAlreadyIsDoor)
        {
            if (side == 0)
            {
                x = 0;
                y = Random.Range((int)padding.y, (int)size.y - (int)padding.y);
            }
            else if (side == 1)
            {
                x = (int)size.x - 1;
                y = Random.Range((int)padding.y, (int)size.y - (int)padding.y);
            }
            else if (side == 2)
            {
                x = Random.Range((int)padding.x, (int)size.x - (int)padding.x);
                y = 0;
            }
            else if (side == 3)
            {
                x = Random.Range((int)padding.x, (int)size.x - (int)padding.x);
                y = (int)size.y - 1;
            }

            // Check if there is already a door in that position or it is close as 2 blocks

            if (!doorPositions.Any(door => Vector2.Distance(new Vector2(door.Item1.x, door.Item1.z), new Vector2(x, y)) <= 2))
            {
                thereAlreadyIsDoor = false;
            }
        }
        return (new Vector3(x, room.position.y, y), side);

    }
    public static void PopulateCarpet(Room room, RoomTheme carpetType)
    {
        // number 2 is normal block, number 6, 7, 8 are for decoration

        // We need to add decartions to the carpet in a random manner, ensuring they are scattered and there is only one decoration per row or column

        if (carpetType == RoomTheme.Rock)
        {
            for (int x = 0; x < room.size.x; x++)
            {
                for (int z = 0; z < room.size.z; z++)
                {
                    room.SetTile(x, 1, z, TileDictionary.GetTile(3));
                }
            }

            int[] possibleYs = new int[(int)room.size.z - 4];

            for (int i = 2; i < room.size.z - 2; i++)
            {
                possibleYs[i-2] = i;
            }

            int probablity = (int)(room.size.z / room.size.x * 100);

            for (int x = 2; x < room.size.x-2; x++)
            {

                if (Random.Range(0, 100) < (probablity*0.9))
                {
                    if(possibleYs.Length == 0)
                    {
                        break;
                    }
                    int y = possibleYs[Random.Range(0, possibleYs.Length)];

       
                    List<int> temp = new List<int>(possibleYs);
                    temp.Remove(y);
                    possibleYs = temp.ToArray();

                    bool isCandle = Random.Range(0, 100) < 40;
                    int theTile = 0;
                    if (isCandle)
                    {
                        theTile = Random.Range(7, 10);
                    }
                    else
                    {
                        theTile = Random.Range(8, 10);
                    }
                    room.SetTile(x, 1, y, TileDictionary.GetTile(theTile));

                    // Set all tiles in the same row and column to normal block

                    if (x - 1 >= 0)
                    {
                        room.SetTile(x - 1, 1, y, TileDictionary.GetTile(3));
                        if (y - 1 >= 0)
                        {
                            room.SetTile(x - 1, 1, y - 1, TileDictionary.GetTile(3));
                        }
                        if (y + 1 < room.size.z)
                        {
                            room.SetTile(x - 1, 1, y + 1, TileDictionary.GetTile(3));
                        }
                    }
                    if (x + 1 < room.size.x)
                    {
                        room.SetTile(x + 1, 1, y, TileDictionary.GetTile(3));
                        
                        if (y - 1 >= 0)
                        {
                            room.SetTile(x + 1, 1, y - 1, TileDictionary.GetTile(3));
                        }
                        if (y + 1 < room.size.z)
                        {
                            room.SetTile(x + 1, 1, y + 1, TileDictionary.GetTile(3));
                        }
                    }
                    if (y - 1 >= 0)
                    {
                        room.SetTile(x, 1, y - 1, TileDictionary.GetTile(3));
                    }
                    if (y + 1 < room.size.z)
                    {
                        room.SetTile(x, 1, y + 1, TileDictionary.GetTile(3));
                    }

 
                    continue;
                }
            }

            
        }else if (carpetType == RoomTheme.Spawn)
        {
            for (int x = 0; x < room.size.x; x++)
            {
                for (int z = 0; z < room.size.z; z++)
                {
                    room.SetTile(x, 1, z, TileDictionary.GetTile(3));
                    
                }
            }
        }

    }
    public static void PopulateFloor(Room room, RoomTheme floorType)
    {
        /*
        InsertTile(0, "floor_foundation_allsides", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/floor_foundation_allsides"), new Vector3(2, 2, 2));
          */

        Vector2 size = new Vector2(room.size.x, room.size.z);

        if (floorType == RoomTheme.Rock)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    room.SetTile(x, 0, y, TileDictionary.GetTile(1));
                }
            }
        }else if (floorType == RoomTheme.Spawn)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    room.SetTile(x, 0, y, TileDictionary.GetTile(1));
                }
            }
        }

    }
}

public enum RoomTheme
{
    Spawn,
    Dirt,
    Rock,
    Wood
}



public enum Side
{
    Top,
    Bottom,
    Left,
    Right
}
