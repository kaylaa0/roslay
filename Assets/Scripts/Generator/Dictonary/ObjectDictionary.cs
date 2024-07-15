using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDictionary : MonoBehaviour
{
    public static Dictionary<int, Tile> objectsmap;
    private ObjectDictionary()
    {

    }

    public static Dictionary<int, Tile> Initialize()
    {
        if (objectsmap != null)
        {

            return objectsmap;
        }

        objectsmap = new Dictionary<int, Tile>();

        // Chair and stool
        InsertTile(0, "chair", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/chair"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(1, "stool", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/stool"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        // Tables
        InsertTile(2, "table_small", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/table_small"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(3, "table_medium", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/table_medium"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(4, "table_long", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/table_long"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(5, "table_small_decorated_A", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/table_small_decorated_A"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(6, "table_small_decorated_B", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/table_small_decorated_B"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(7, "table_medium_decorated_A", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/table_medium_decorated_A"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(8, "table_medium_tablecloth_decorated_B", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/table_medium_tablecloth_decorated_B"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(9, "table_long_decorated_A", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/table_long_decorated_A"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(10, "table_long_decorated_B", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/table_long_decorated_C"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(11, "table_medium_broken", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/table_medium_broken"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(12, "table_long_broken", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/table_long_broken"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(13, "table_medium_tablecloth", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/table_medium_tablecloth"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(14, "table_long_tablecloth", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/table_long_tablecloth"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(15, "table_long_tablecloth_decorated_A", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/table_long_tablecloth_decorated_A"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        // Barrels
        InsertTile(16, "barrel_small", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/barrel_small"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(17, "barrel_small_stack", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/barrel_small_stack"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(18, "barrel_large", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/barrel_large"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(19, "barrel_large_decorated", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/barrel_large_decorated"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        // Boxes
        InsertTile(20, "box_small", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/box_small"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(21, "box_stacked", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/box_stacked"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(22, "box_small_decorated", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/box_small_decorated"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(23, "box_large", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/box_large"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        // Kegs
        InsertTile(24, "keg", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/keg"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        InsertTile(25, "keg_decorated", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/keg_decorated"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        // Crates
        InsertTile(26, "crates_stacked", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/crates_stacked"), new Vector3(1, 1, 1), TileType.FloorDecoration);
        // Column
        InsertTile(27, "column", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/column"), new Vector3(1, 1, 1), TileType.FloorDecoration);

        return objectsmap;
    }

    private static void InsertTile(int id, string name, GameObject prefab, Vector3 size, TileType tileType, System.Action<GameObject> specialFunction = null, bool isStatic = false)
    {
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.id = id;
        tile.tileName = name;
        tile.prefab = prefab;
        tile.position = Vector3.zero;
        tile.rotation = Quaternion.identity;
        tile.size = size;
        tile.specialFunction = specialFunction;
        tile.isStatic = isStatic;
        tile.ProcessingState = ProcessingState.Unprocessed;
        tile.tileType = tileType;

        objectsmap.Add(id, tile);
    }

    public static Tile GetObject(int id)
    {
        return objectsmap[id];
    }
}
