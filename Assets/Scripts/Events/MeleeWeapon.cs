using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : NetworkBehaviour, IInteractable
{
    // private float damage = 30f;
    // private float attackSpeed = 1;
    // private float range = 0.5f;
    private bool isPickedUp = false;
    private GameObject player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void Interact(int clientId)
    {
        if (!isPickedUp)
        {
            NetworkConnection interactingClient = ServerManager.Clients[clientId];
            PickUpWeapon(interactingClient);
        }
    }

    public void PickUpWeapon(NetworkConnection interactingClient)
    {
        player = null;
        transform.parent = player.transform;
        transform.localPosition = new Vector3(0, 0, 0);
        isPickedUp = true;
        
    }
}
