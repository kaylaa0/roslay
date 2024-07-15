using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
    public static Dictionary<int, Item> itemsmap;

    private ItemDictionary()
    {

    }

    public static Dictionary<int, Item> Initialize()
    {
        if (itemsmap != null)
        {
            return itemsmap;
        }

        itemsmap = new Dictionary<int, Item>();

        // Weapons
        InsertItem(0, "1H_Axe", Resources.Load<GameObject>("Dungeon/Dungeon Items/1H_Axe"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(1, "1H_Axe_Offhand", Resources.Load<GameObject>("Dungeon/Dungeon Items/1H_Axe_Offhand"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(2, "1H_Crossbow", Resources.Load<GameObject>("Dungeon/Dungeon Items/1H_Crossbow"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(3, "1H_Sword", Resources.Load<GameObject>("Dungeon/Dungeon Items/1H_Sword"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(4, "1H_Sword_Offhand", Resources.Load<GameObject>("Dungeon/Dungeon Items/1H_Sword_Offhand"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(5, "1H_Wand", Resources.Load<GameObject>("Dungeon/Dungeon Items/1H_Wand"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(6, "2H_Axe", Resources.Load<GameObject>("Dungeon/Dungeon Items/2H_Axe"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(7, "2H_Crossbow", Resources.Load<GameObject>("Dungeon/Dungeon Items/2H_Crossbow"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(8, "2H_Staff", Resources.Load<GameObject>("Dungeon/Dungeon Items/2H_Staff"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(9, "2H_Sword", Resources.Load<GameObject>("Dungeon/Dungeon Items/2H_Sword"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(10, "arrow", Resources.Load<GameObject>("Dungeon/Dungeon Items/arrow"), new Vector3(1, 1, 1), TileType.Item, ItemType.Pickable);
        InsertItem(11, "arrow_bundle", Resources.Load<GameObject>("Dungeon/Dungeon Items/arrow_bundle"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(12, "Badge_Shield", Resources.Load<GameObject>("Dungeon/Dungeon Items/Badge_Shield"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(13, "Knife", Resources.Load<GameObject>("Dungeon/Dungeon Items/Knife"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(14, "Knife_Offhand", Resources.Load<GameObject>("Dungeon/Dungeon Items/Knife_Offhand"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(15, "Mug", Resources.Load<GameObject>("Dungeon/Dungeon Items/Mug"), new Vector3(1, 1, 1), TileType.Item, ItemType.Consumable);
        InsertItem(16, "quiver", Resources.Load<GameObject>("Dungeon/Dungeon Items/quiver"), new Vector3(1, 1, 1), TileType.Item, ItemType.Pickable);
        InsertItem(17, "Rectangle_Shield", Resources.Load<GameObject>("Dungeon/Dungeon Items/Rectangle_Shield"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(18, "Round_Shield", Resources.Load<GameObject>("Dungeon/Dungeon Items/Round_Shield"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(19, "Spellbook", Resources.Load<GameObject>("Dungeon/Dungeon Items/Spellbook"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(20, "Spellbook_open", Resources.Load<GameObject>("Dungeon/Dungeon Items/Spellbook_open"), new Vector3(1, 1, 1), TileType.Item, ItemType.Pickable);
        InsertItem(21, "Spike_Shield", Resources.Load<GameObject>("Dungeon/Dungeon Items/Spike_Shield"), new Vector3(1, 1, 1), TileType.Item, ItemType.Weapon);
        InsertItem(22, "Throwable", Resources.Load<GameObject>("Dungeon/Dungeon Items/Throwable"), new Vector3(1, 1, 1), TileType.Item, ItemType.Consumable);

        return itemsmap;
    }

    private static void InsertItem(int id, string name, GameObject prefab, Vector3 size, TileType tileType, ItemType itemType, System.Action<GameObject> specialFunction = null, bool isStatic = false)
    {
        Item tile = ScriptableObject.CreateInstance<Item>();
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
        tile.itemType = itemType;

        itemsmap.Add(id, tile);
    }

    public static Item GetItem(int id)
    {
        return itemsmap[id];
    }

    public static Item GetItem(string name)
    {
        foreach (KeyValuePair<int, Item> item in itemsmap)
        {
            if (item.Value.tileName == name)
            {
                return item.Value;
            }
        }
        return null;
    }

    public static Item GetRandomOneHanded()
    {
        List<Item> oneHanded = new List<Item>();
        foreach (KeyValuePair<int, Item> item in itemsmap)
        {
            if (item.Value.itemType == ItemType.Weapon && item.Value.tileName.Contains("1H"))
            {
                oneHanded.Add(item.Value);
            }
        }
        // also add knife and off hand knife
        oneHanded.Add(itemsmap[13]);
        oneHanded.Add(itemsmap[14]);
        return oneHanded[Random.Range(0, oneHanded.Count)];
    }

    public static Item GetRandomTwoHanded()
    {
        List<Item> twoHanded = new List<Item>();
        foreach (KeyValuePair<int, Item> item in itemsmap)
        {
            if (item.Value.itemType == ItemType.Weapon && item.Value.tileName.Contains("2H"))
            {
                twoHanded.Add(item.Value);
            }
        }
        return twoHanded[Random.Range(0, twoHanded.Count)];
    }

    public static Item GetRandomShield()
    {
        List<Item> shields = new List<Item>();
        foreach (KeyValuePair<int, Item> item in itemsmap)
        {
            if (item.Value.itemType == ItemType.Weapon && item.Value.tileName.Contains("Shield"))
            {
                shields.Add(item.Value);
            }
        }
        return shields[Random.Range(0, shields.Count)];
    }
}
