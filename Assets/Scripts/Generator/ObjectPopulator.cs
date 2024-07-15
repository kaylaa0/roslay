using FishNet.Object;
using GameKit.Dependencies.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectPopulator : NetworkBehaviour
{
    public static void PopulateObjects(Room room, RoomTheme theme = RoomTheme.Rock, int roomCount = -1, bool isBossRoom = false)
    {
        Tile[] objects = new Tile[0];


        if (theme == RoomTheme.Rock)
        {
            // Append the generated columns array with the columns spawned in the room
            objects = objects.Concat(FillColumns(room, objects)).Where(obj => obj != null).ToArray();
            // Append the generated tables array with the tables spawned in the room
            objects = objects.Concat(FillTables(room, objects)).Where(obj => obj != null).ToArray();
            // Append the generated chairs array with the chairs spawned in the room
            objects = objects.Concat(FillChairs(room, objects)).Where(obj => obj != null).ToArray();
            // Append the generated barrels array with the barrels spawned in the room
            objects = objects.Concat(FillBarrels(room, objects)).Where(obj => obj != null).ToArray();
            // Append the generated boxes array with the boxes spawned in the room
            objects = objects.Concat(FillBoxes(room, objects)).Where(obj => obj != null).ToArray();
            // Append the generated kegs array with the kegs spawned in the room
            objects = objects.Concat(FillKegs(room, objects)).Where(obj => obj != null).ToArray();
            // Append the generated crates array with the crates spawned in the room
            objects = objects.Concat(FillCrates(room, objects)).Where(obj => obj != null).ToArray();

        }

        room.SetObjects(objects);
    }

    public static Tile[] FillTables(Room room, Tile[] objects)
    {
        int RoomSize = (int)room.size.x * (int)room.size.z;
        // If room size is bigger than 20, we can place more tables 
        int numTables = RoomSize > 150 ? Random.Range(3, 7) : (RoomSize > 100 ? Random.Range(2, 6) : Random.Range(1, 4));

        Tile[] tables = new Tile[numTables];

        for (int i = 0; i < numTables; i++)
        {
            Vector3 position = GetValidPosition(room, objects.Concat(tables).Where(obj => obj != null).ToArray());
            

            tables[i] = Instantiate(ObjectDictionary.GetObject(Random.Range(2, 16)));
            tables[i].position = position;

            // Randomly rotate it 0, 90 degrees or 180 or 270
            tables[i].rotation = new Quaternion(0, Random.Range(0, 4) * 90, 0, 0);
        }

        return tables;
    }

    public static Tile[] FillChairs(Room room, Tile[] objects)
    {
        int RoomSize = (int)room.size.x * (int)room.size.z;
        int numChairs = RoomSize > 150 ? Random.Range(5, 10) : (RoomSize > 100 ? Random.Range(3, 7) : Random.Range(1, 5));

        Tile[] chairs = new Tile[numChairs];

        for (int i = 0; i < numChairs; i++)
        {
            Vector3 position = GetValidPosition(room, objects.Concat(chairs).Where(obj => obj != null).ToArray());

            chairs[i] = Instantiate(ObjectDictionary.GetObject(Random.Range(0, 2)));
            chairs[i].position = position;

            chairs[i].rotation = new Quaternion(0, Random.Range(0, 4) * 90, 0, 0);
        }

        return chairs;
    }

    public static Tile[] FillBarrels(Room room, Tile[] objects)
    {
        int numBarrels = Random.Range(1, 5);

        Tile[] barrels = new Tile[numBarrels];

        for (int i = 0; i < numBarrels; i++)
        {
            Vector3 position = GetValidPosition(room, objects.Concat(barrels).Where(obj => obj != null).ToArray());

            barrels[i] = Instantiate(ObjectDictionary.GetObject(Random.Range(16, 20)));
            barrels[i].position = position;

            barrels[i].rotation = new Quaternion(0, Random.Range(0, 4) * 90, 0, 0);
        }

        return barrels;
    }

    public static Tile[] FillBoxes(Room room, Tile[] objects)
    {
        int numBoxes = Random.Range(1, 5);

        Tile[] boxes = new Tile[numBoxes];

        for (int i = 0; i < numBoxes; i++)
        {
            Vector3 position = GetValidPosition(room, objects.Concat(boxes).Where(obj => obj != null).ToArray());

            boxes[i] = Instantiate(ObjectDictionary.GetObject(Random.Range(20, 24)));
            boxes[i].position = position;

            boxes[i].rotation = new Quaternion(0, Random.Range(0, 4) * 90, 0, 0);
        }

        return boxes;
    }

    public static Tile[] FillKegs(Room room, Tile[] objects)
    {
        int numKegs = Random.Range(1, 3);

        Tile[] kegs = new Tile[numKegs];

        for (int i = 0; i < numKegs; i++)
        {
            Vector3 position = GetValidPosition(room, objects.Concat(kegs).Where(obj => obj != null).ToArray());

            kegs[i] = Instantiate(ObjectDictionary.GetObject(Random.Range(24, 26)));
            kegs[i].position = position;

            kegs[i].rotation = new Quaternion(0, Random.Range(0, 4) * 90, 0, 0);
        }

        return kegs;
    }

    public static Tile[] FillCrates(Room room, Tile[] objects)
    {
        int numCrates = Random.Range(1, 2);

        Tile[] crates = new Tile[numCrates];

        for (int i = 0; i < numCrates; i++)
        {
            Vector3 position = GetValidPosition(room, objects.Concat(crates).Where(obj => obj != null).ToArray());

            crates[i] = Instantiate(ObjectDictionary.GetObject(26));
            crates[i].position = position;

            crates[i].rotation = new Quaternion(0, Random.Range(0, 4) * 90, 0, 0);
        }

        return crates;
    }

    public static Tile[] FillColumns(Room room, Tile[] objects)
    {

        int numColumns = Random.Range(1, 5);

        Tile[] columns = new Tile[numColumns];

        for (int i = 0; i < numColumns; i++)
        {
            
            Vector3 position = GetValidPosition(room, objects.Concat(columns).Where(obj => obj != null).ToArray());

            columns[i] = Instantiate(ObjectDictionary.GetObject(27));
            columns[i].position = position;

            columns[i].rotation = new Quaternion(0, Random.Range(0, 4) * 90, 0, 0);
        }

        return columns;
    }

    public static Vector3 GetValidPosition(Room room, Tile[] objects)
    {
        // A valid position is that in the room, in the vicinty is no decorative floor. Which is carpet layer has to be 3
        // Also in the vicinty there are no other objects
        // Also clam 3.5f from edges making sure we do not place objects too close to the walls

        Vector3 offset = new Vector3(room.position.x, room.position.y, room.position.z);

        Vector3 position = new Vector3(Random.Range((offset.x*2) + 3.5f, (offset.x*2) + (room.size.x*2.0f) - 3.5f), (offset.y*2) + 2f, Random.Range((offset.z*2) + 3.5f, (offset.z*2) + (room.size.z*2.0f) - 3.5f));

        // Now we check if position is invalid due a decorative floor tile or other object, we try to move towards the oppsite direction until we can place the object

        bool checking = true;

        while(checking)
        {

            // Check if the position is valid
            bool valid = true;
            Vector3 direction = new Vector3(0,0,0);


            foreach (Tile obj in objects)
            {

                if (Vector3.Distance(obj.position, position) < 1.5f)
                {
                    valid = false;
                    direction = obj.position - position;
                    break;
                }
            }

            // Room tiles is 3D array so iterate over all items and check



            if (!valid)
            {
                // Move towards the opposite direction
                position = position - direction;
            }
            else
            {
                // All [1,x,x] that are in visinity of room.tiles need to be set to 3

                int x = (int)(position.x - (offset.x * 2));
                int z = (int)(position.z - (offset.z * 2));
                int y = (int)(position.y - (offset.y * 2));

                for (int a = -2; a < 4; a++)
                {
                    for (int b = -2; b < 4; b++)
                    {
                        if (room.GetTile(x + a, y, z + b) != null)
                        {
                            room.SetTile(x + a, y, z + b, TileDictionary.GetTile(3));
                        }
                    }
                }

                checking = false;
            }
        }
       

        return position;
    }



}
