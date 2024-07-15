using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : ScriptableObject
{
    public Vector3 position;
    public Vector3 size;
    public Tile[] tiles;
    public Tile[] objects;
    public Item[] items;
    public List<Tile> doors;

    public void SetData(Vector3 position, Vector3 size)
    {
        this.position = position;
        this.size = size;
        this.tiles = new Tile[0];
        this.doors = new List<Tile>();
    }

    public void SetItems(Item[] items)
    {
        this.items = items;
    }

    public void SetTiles(Tile[] tiles)
    {
        this.tiles = tiles;
    }

    public void SetObjects(Tile[] objects)
    {
        this.objects = objects;
    }

    public void SetTile(Vector3 position, Tile tile)
    {
        this.SetTile(position.x, position.y, position.z, tile);
    }


    public void SetTile(int x, int y, int z, Tile tile)
    {
        this.SetTile((float)x, (float)y, (float)z, tile);
    }

    public void SetTile(float x, float y, float z, Tile tile)
    {
        if (tile == null)
        {
            // This means we are removing the tile if it exist
            this.tiles = tiles.Where(t => t.position != new Vector3(x * 2, y * 2, z * 2)).ToArray();
            return;
        }
        if (tile.id == 12)
        {
            AddDoor(tile);
        }

        tile.position = new Vector3(x * 2, y * 2, z * 2);
        if (tile.rotation == null)
        {
            tile.rotation = Quaternion.identity;
        }
        tiles = tiles.Concat(new Tile[] { tile }).ToArray();
    }


    public Tile GetTile(int x, int y, int z)
    {
        return this.tiles.Where(tile => tile.position == new Vector3(x*2, y * 2, z * 2)).FirstOrDefault();
    }

    public void AddDoor(Tile door)
    {
        doors.Add(door);
    }


    public (Vector3[], Quaternion[]) GetDoorPositions()
    {
        Vector3[] positions = new Vector3[doors.Count];
        Quaternion[] rotations = new Quaternion[doors.Count];

        for (int i = 0; i < doors.Count; i++)
        {
            positions[i] = doors[i].position;
            rotations[i] = doors[i].rotation;
        }

        return (positions, rotations);
    }

    public void ClearOut(Vector3 position, int radius)
    {
        this.tiles = this.tiles.Where(tile => Vector3.Distance(tile.position, position) > radius).ToArray();
    }

    public void ClearOut(Vector3 position, int radius, TileType types)
    {
        this.tiles = this.tiles.Where(tile => Vector3.Distance(tile.position, position) > radius || tile.tileType != types).ToArray();
    }

    public void ClearOut(Vector3 position, int radius, TileType[] types)
    {
        this.tiles = this.tiles.Where(tile => Vector3.Distance(tile.position, position) > radius || !types.Contains(tile.tileType)).ToArray();
    }

    public void ClearOut(Vector3 position, Vector3 area, TileType[] types)
    {
        this.tiles = this.tiles.Where(tile => !((tile.position.x >= position.x && tile.position.x <= position.x + area.x) && (tile.position.z >= position.z && tile.position.z <= position.z + area.z)) || !types.Contains(tile.tileType)).ToArray();
    }
}
