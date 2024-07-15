using FishNet.Managing;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.EventSystems.EventTrigger;

public struct TileDictionaryEntry
{
    public int ID;
    public string name;
    public GameObject prefab;
    public Vector3 size;
    public System.Action<GameObject> specialFunction;
    public bool isStatic;
    public ProcessingState ProcessingState;
}

public enum ProcessingState
{
    Unprocessed,
    Processing,
    Processed,
    ConnectedDoor
}

public class TileDictionary: MonoBehaviour
{
    private static Dictionary<int, Tile> tilemap;

    private TileDictionary()
    {

    }

    public static Dictionary<int, Tile> Initialize()
    {
        if (tilemap != null)
        {
            return tilemap;
        }

        tilemap = new Dictionary<int, Tile>();
        // Rock Foundations
        InsertTile(1, "floor_foundation_allsides", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/floor_foundation_allsides"), new Vector3(2, 2, 2), TileType.Floor);
        InsertTile(2, "floor_foundation_diagonal_corner", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/floor_foundation_diagonal_corner"), new Vector3(2, 2, 2), TileType.Floor);
        // Rock Carpet Tiles
        InsertTile(3, "floor_tile_small", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/floor_tile_small"), new Vector3(2, 2, 2), TileType.Carpet);
        InsertTile(4, "floor_tile_small_broken_A", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/floor_tile_small_broken_A"), new Vector3(2, 2, 2), TileType.Carpet);
        InsertTile(5, "floor_tile_small_broken_B", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/floor_tile_small_broken_B"), new Vector3(2, 2, 2), TileType.Carpet);
        InsertTile(6, "floor_tile_small_corner", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/floor_tile_small_corner"), new Vector3(2, 2, 2), TileType.Carpet);
        InsertTile(7, "floor_tile_small_decorated", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/floor_tile_small_decorated"), new Vector3(2, 2, 2), TileType.Carpet);
        InsertTile(8, "floor_tile_small_weeds_A", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/floor_tile_small_weeds_A"), new Vector3(2, 2, 2), TileType.Carpet);
        InsertTile(9, "floor_tile_small_weeds_B", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/floor_tile_small_weeds_B"), new Vector3(2, 2, 2), TileType.Carpet);
        // Rock Wall Tiles
        InsertTile(10, "wall_half", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/wall_half"), new Vector3(2, 4, 2), TileType.Wall);
        InsertTile(11, "wall_corner_small", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/wall_corner_small"), new Vector3(2, 4, 2), TileType.Wall);
        InsertTile(12, "wall_doorway", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/wall_doorway"), new Vector3(4, 4, 4), TileType.Door);
        InsertTile(13, "floor_tile_small_weeds_B", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/floor_tile_small_weeds_B"), new Vector3(2, 2, 2), TileType.Carpet);
        InsertTile(14, "floor_tile_small_weeds_B", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/floor_tile_small_weeds_B"), new Vector3(2, 2, 2), TileType.Carpet);
        InsertTile(15, "floor_tile_small_weeds_B", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/floor_tile_small_weeds_B"), new Vector3(2, 2, 2), TileType.Carpet);
        InsertTile(16, "floor_tile_small_weeds_B", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/floor_tile_small_weeds_B"), new Vector3(2, 2, 2), TileType.Carpet);
        // Barrier
        InsertTile(17, "barrier", Resources.Load<GameObject>("Dungeon/Dungeon Pieces/barrier_half"), new Vector3(2, 2, 2), TileType.Wall);
        return tilemap;
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

        tilemap.Add(id,tile); 
    }

    public static Tile GetTile(int id)
    {
        return Instantiate(tilemap[id]);
    }
}
