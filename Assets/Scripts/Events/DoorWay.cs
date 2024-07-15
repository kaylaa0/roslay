using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorWay : NetworkBehaviour, IInteractable
{
    GameObject doorFrame;
    bool isOpen = false;
    Animator animator;
    void Start()
    {
        doorFrame = transform.GetChild(0).gameObject;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [ServerRpc(RequireOwnership = false)]
    public void Interact(int clientId)
    {
        ToggleDoor();
    }

    [ObserversRpc]
    public void ToggleDoor()
    {
        if (isOpen)
        {
            CloseDoor();
            isOpen = false;
        }
        else
        {
            OpenDoor();
            isOpen = true;
        }
    }

    public void OpenDoor()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
        {
            animator.Play("open_door");
        }


    }
    public void CloseDoor()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
        {
            animator.Play("close_door");
        }
    }
}
