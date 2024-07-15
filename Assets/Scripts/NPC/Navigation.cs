using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : NetworkBehaviour
{
    public float aggroRadius = 10f; // Set your desired aggro radius here
    public float forgetRadius = 14f; // Set your desired forget radius here
    public float forgetTimeout = 5f; // Set your desired forget timeout here
    NavMeshAgent agent;
    GameObject player;
    Animator _animator;

    // animation IDs
    private int _animIDSpeed;
    //private int _animIDGrounded;
    //private int _animIDJump;
    //private int _animIDFreeFall;
    //private int _animIDMotionSpeed;

    private float lastSeenTime = Mathf.NegativeInfinity;

    public override void OnStartServer()
    {
        //Create NavMeshAgent
        gameObject.AddComponent<NavMeshAgent>();
        agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = 360;
        // Find object with tag player
        player = GameObject.FindGameObjectWithTag("Player");

        _animator = GetComponent<Animator>();
        AssignAnimationIDs();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayer(GameObject setPlayer)
    {
        if (IsServerInitialized)
        {
            player = setPlayer;

            agent.SetDestination(player.transform.position);
        }
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServerInitialized)
        {
            if(player == null)
            {
                // Check if there is a player within the aggro radius
                Collider[] colliders = Physics.OverlapSphere(transform.position, aggroRadius);
                foreach (Collider col in colliders)
                {
                    if (col.CompareTag("Player"))
                    {
                        player = col.gameObject;
                        lastSeenTime = Time.time;
                        
                        return; // Exit the loop if a player is found
                    }
                }
            }

            // Check if the player is within the forget radius and the timeout hasn't elapsed
            if (player != null && Vector3.Distance(transform.position, player.transform.position) >= forgetRadius && Time.time - lastSeenTime >= forgetTimeout)
            {
                // If no player is found within aggro radius or the player is outside the forget radius or the timeout has elapsed, stop aggroing
                player = null;
                agent.SetDestination(transform.position);
                _animator.SetFloat(_animIDSpeed, 0f);
                return;
            }

            if (player != null)
            {
                if(Vector3.Distance(transform.position, player.transform.position) >= 1.0f)
                {
                    // target is 1.0f radious around the player, so get as close as 1.0f to the player but stop at 1.0f
                    Vector3 targetPosition = player.transform.position + (transform.position - player.transform.position).normalized * 1.0f;
                    agent.SetDestination(targetPosition);
                }
                _animator.SetFloat(_animIDSpeed, agent.velocity.magnitude);
            }
        }
    }
}
