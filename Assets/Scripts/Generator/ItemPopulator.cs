using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPopulator : MonoBehaviour
{
    public static void PopulateItems(Room room, RoomTheme theme = RoomTheme.Rock, int roomCount = -1, bool isBossRoom = false)
    {
        Item[] items = new Item[0];

        if (theme == RoomTheme.Rock)
        {
            // Append the generated weapons array with the weapons spawned in the room
            items = items.Concat(FillWeapons(room, items)).Where(obj => obj != null).ToArray();
            // Append the generated armors array with the armors spawned in the room
            //items = items.Concat(FillArmors(room, items)).Where(obj => obj != null).ToArray();
            // Append the generated consumables array with the consumables spawned in the room
            items = items.Concat(FillConsumables(room, items)).Where(obj => obj != null).ToArray();
            // Append the generated pickables array with the pickables spawned in the room
            items = items.Concat(FillPickables(room, items)).Where(obj => obj != null).ToArray();
            // Append the generated quests array with the quests spawned in the room
            //items = items.Concat(FillQuests(room, items)).Where(obj => obj != null).ToArray();
            // Append the generated junks array with the junks spawned in the room
            //items = items.Concat(FillJunks(room, items)).Where(obj => obj != null).ToArray();
        }else if (theme == RoomTheme.Spawn)
        {
            items = items.Concat(FillSpawnItems(room, items)).Where(obj => obj != null).ToArray();
        }

        room.SetItems(items);

    }

    public static Item[] FillSpawnItems(Room room, Item[] items)
    {
        // We add two random one handed, two random two handed, a spell book, a throwable, and a shield

        if (items == null)
        {
            items = new Item[0];
        }

        // One handed GetRandomOneHanded
        Item oneHanded = ItemDictionary.GetRandomOneHanded();
        oneHanded.position = new Vector3(2, 2.5f, 2);
        items = items.Concat(new Item[] { oneHanded }).ToArray();

        // One handed GetRandomOneHanded
        Item oneHanded2 = ItemDictionary.GetRandomOneHanded();
        oneHanded2.position = new Vector3(2, 2.5f, 4);
        items = items.Concat(new Item[] { oneHanded2 }).ToArray();

        // Two handed GetRandomTwoHanded
        Item twoHanded = ItemDictionary.GetRandomTwoHanded();
        twoHanded.position = new Vector3(2, 2.5f, 6);
        items = items.Concat(new Item[] { twoHanded }).ToArray();

        // Two handed GetRandomTwoHanded
        Item twoHanded2 = ItemDictionary.GetRandomTwoHanded();
        twoHanded2.position = new Vector3(2, 2.5f, 8);
        items = items.Concat(new Item[] { twoHanded2 }).ToArray();

        // Spell book
        Item spellBook = ItemDictionary.GetItem("Spellbook");
        spellBook.position = new Vector3(2, 2.5f, 10);
        items = items.Concat(new Item[] { spellBook }).ToArray();

        // Throwable
        Item throwable = ItemDictionary.GetItem("Throwable");
        throwable.position = new Vector3(2, 2.5f, 12);
        items = items.Concat(new Item[] { throwable }).ToArray();

        // Shield
        Item shield = ItemDictionary.GetRandomShield();
        shield.position = new Vector3(2, 2.5f, 14);
        items = items.Concat(new Item[] { shield }).ToArray();



        return items;
    }

    public static Item[] FillWeapons(Room room, Item[] items)
    {


        return items;
    }

    public static Item[] FillConsumables(Room room, Item[] items)
    {

        return items;

    }

    public static Item[] FillPickables(Room room, Item[] items)
    {

        return items;
    }
}
