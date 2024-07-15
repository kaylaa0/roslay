using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door: NetworkBehaviour, IInteractable
{
    public bool isOpen = false;
    public bool isLocked = false;
    public bool isExit = false;
    public bool isSecret = false;

    public Vector3 position;
    public Quaternion rotation;

    [ServerRpc(RequireOwnership = false)]
    public void Interact(int clientId)
    {
        transform.parent.GetComponent<IInteractable>().Interact(clientId);
    }




}
