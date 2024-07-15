using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

//This script is made by Bobsi Unity for Youtube
public class PlayerPickup : NetworkBehaviour
{
    [SerializeField] float raycastDistance;
    [SerializeField] LayerMask pickupLayer;
    [SerializeField] KeyCode pickupButton = KeyCode.E;
    [SerializeField] KeyCode dropButton = KeyCode.Q;
    [SerializeField] GameObject hand;

    Camera cam;
    bool hasObjectInHand;
    GameObject objInHand;
    Transform worldObjectHolder;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsOwner)
            enabled = false;

        cam = Camera.main;
        worldObjectHolder = GameObject.FindGameObjectWithTag("WorldObjects").transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(pickupButton))
            Pickup();

        if (Input.GetKeyDown(dropButton))
            Drop();
    }

    void Pickup()
    {

        Vector3 center = transform.position + new Vector3(0,0.5f,0);
        Vector3 halfExtents = new Vector3(0.5f, 1f, 0.5f); // Adjust the half extents according to the size of your player
        Vector3 direction = transform.forward;
        Quaternion orientation = Quaternion.identity;
        float maxDistance = raycastDistance;
        LayerMask layerMask = pickupLayer;

        RaycastHit[] boxcastHits = Physics.BoxCastAll(center, halfExtents, direction, orientation, maxDistance, layerMask);

        // Visualize the box cast
        Debug.DrawRay(center, direction * maxDistance, Color.green, 10f); // Draw the forward ray

        // Calculate the corners of the box
        Vector3 frontTopLeft = center + Vector3.Scale(halfExtents, new Vector3(-1, 1, -1));
        Vector3 frontTopRight = center + Vector3.Scale(halfExtents, new Vector3(1, 1, -1));
        Vector3 frontBottomLeft = center + Vector3.Scale(halfExtents, new Vector3(-1, -1, -1));
        Vector3 frontBottomRight = center + Vector3.Scale(halfExtents, new Vector3(1, -1, -1));
        Vector3 backTopLeft = center + Vector3.Scale(halfExtents, new Vector3(-1, 1, 1));
        Vector3 backTopRight = center + Vector3.Scale(halfExtents, new Vector3(1, 1, 1));
        Vector3 backBottomLeft = center + Vector3.Scale(halfExtents, new Vector3(-1, -1, 1));
        Vector3 backBottomRight = center + Vector3.Scale(halfExtents, new Vector3(1, -1, 1));

        // Draw the wireframe cube
        Debug.DrawLine(frontTopLeft, frontTopRight, Color.green, 10f);
        Debug.DrawLine(frontTopRight, frontBottomRight, Color.green, 10f);
        Debug.DrawLine(frontBottomRight, frontBottomLeft, Color.green, 10f);
        Debug.DrawLine(frontBottomLeft, frontTopLeft, Color.green, 10f);
        Debug.DrawLine(backTopLeft, backTopRight, Color.green, 10f);
        Debug.DrawLine(backTopRight, backBottomRight, Color.green, 10f);
        Debug.DrawLine(backBottomRight, backBottomLeft, Color.green, 10f);
        Debug.DrawLine(backBottomLeft, backTopLeft, Color.green, 10f);
        Debug.DrawLine(frontTopLeft, backTopLeft, Color.green, 10f);
        Debug.DrawLine(frontTopRight, backTopRight, Color.green, 10f);
        Debug.DrawLine(frontBottomRight, backBottomRight, Color.green, 10f);
        Debug.DrawLine(frontBottomLeft, backBottomLeft, Color.green, 10f);


        foreach (RaycastHit hit in boxcastHits)
        {
            if (!hasObjectInHand)
            {
                SetObjectInHandServer(hit.transform.gameObject, hand, gameObject);
                objInHand = hit.transform.gameObject;
                hasObjectInHand = true;
                break;
            }
            else if (hasObjectInHand)
            {
                Drop();

                SetObjectInHandServer(hit.transform.gameObject, hand, gameObject);
                objInHand = hit.transform.gameObject;
                hasObjectInHand = true;
                break;
            }
        }

       
    }

    [ServerRpc(RequireOwnership = false)]
    void SetObjectInHandServer(GameObject obj, GameObject hand, GameObject player)
    {
        SetObjectInHandObserver(obj, hand, player);
    }

    [ObserversRpc]
    void SetObjectInHandObserver(GameObject obj, GameObject hand, GameObject player)
    {
        obj.transform.position = hand.transform.position;
        obj.transform.rotation = hand.transform.rotation;
        obj.transform.parent = hand.transform;

        if (obj.GetComponent<Rigidbody>() != null)
            obj.GetComponent<Rigidbody>().isKinematic = true;
    }

    void Drop()
    {
        if (!hasObjectInHand)
            return;

        DropObjectServer(objInHand, worldObjectHolder);
        hasObjectInHand = false;
        objInHand = null;
    }

    [ServerRpc(RequireOwnership = false)]
    void DropObjectServer(GameObject obj, Transform worldHolder)
    {
        DropObjectObserver(obj, worldHolder);
    }

    [ObserversRpc]
    void DropObjectObserver(GameObject obj, Transform worldHolder)
    {
        obj.transform.parent = worldHolder;

        if (obj.GetComponent<Rigidbody>() != null)
            obj.GetComponent<Rigidbody>().isKinematic = false;
    }
}