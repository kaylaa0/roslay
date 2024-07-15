using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile : ScriptableObject
{
    public int id;
    public string tileName;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 size;
    public GameObject prefab;
    public System.Action<GameObject> specialFunction;
    public bool isStatic;
    public ProcessingState ProcessingState;
    public TileType tileType;

    public Tile SetPosition(Vector3 position)
    {
        this.position = position;
        return this;
    }

    public Tile SetRotation(Quaternion rotation)
    {
        this.rotation = rotation;
        return this;
    }

    public Tile SetSize(Vector3 size)
    {
        this.size = size;
        return this;
    }
}

public enum TileType
{
    Floor,
    Carpet,
    Wall,
    Door,
    FloorDecoration,
    WallDecoration,
    POI,
    Item,
    Spawner,
    Exit
}

