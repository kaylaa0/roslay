using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDamageable
{
    public void OnTakeDamage(int clientId, int damage);
}

