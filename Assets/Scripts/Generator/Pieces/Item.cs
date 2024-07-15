using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Tile
{
    public ItemType itemType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Pickable,
    Quest,
    Junk
}