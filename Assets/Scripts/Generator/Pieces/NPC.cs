using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : ScriptableObject
{
    public int id;
    public string npcName;
    public Vector3 size;
    public GameObject prefab;
    public System.Action<GameObject> specialFunction;
    public ProcessingState ProcessingState;
    public int health;
    public int damage;
    public int speed;
    public AttackType attackType;
    public int attackRange;
    public int attackSpeed;
}
