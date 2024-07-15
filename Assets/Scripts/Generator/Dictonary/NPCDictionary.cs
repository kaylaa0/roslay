using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    Melee,
    Ranged
}

public class NPCDictionary : MonoBehaviour
{
    public static Dictionary<int, NPC> npcmap;
    private NPCDictionary()
    {

    }
    public static Dictionary<int, NPC> Initialize()
    {
        if(npcmap!=null)
        {

            return npcmap;
        }
        npcmap = new Dictionary<int, NPC>();

        InsertNPC(1, "skeleton_minion", Resources.Load<GameObject>("Dungeon/Dungeon NPCs/Skeleton_Minion"), new Vector3(1, 1, 1), null, 100, 10, 5, AttackType.Melee, 1, 1);

        return npcmap;
    }

    private static void InsertNPC(int id, string npcName, GameObject prefab, Vector3 size, System.Action<GameObject> specialFunction, int health, int damage, int speed, AttackType attackType, int attackRange, int attackSpeed)
    {
        npcmap.Add(id, new NPC
        {
            id = id,
            npcName = npcName,
            prefab = prefab,
            size = size,
            specialFunction = specialFunction,
            health = health,
            damage = damage,
            speed = speed,
            attackType = attackType,
            attackRange = attackRange,
            attackSpeed = attackSpeed
        });
    }

    public static NPC GetNPC(int id)
    {
        return npcmap[id];
    }
}
