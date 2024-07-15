using Controls;
using FishNet;
using FishNet.Managing.Timing;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Combat : NetworkBehaviour, IDamageable
{
    public float attackRange = 0.5f;
    public float attackRate = 2f;
    public int attackDamage = 10;
    public LayerMask attackMask;

    private float nextAttackTime = 0f;
    [SerializeField]
    private int Health = 100;
    [SerializeField]
    private ProgressBar HealthBar;

    private float MaxHealth;
    private NavMeshAgent _agent;
    private Animator _animator;

    GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        MaxHealth = Health;
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    public override void OnStartClient()
    {
        HealthBar.SetProgress(Health / MaxHealth, 3);
        Canvas canvas = GameObject.Find("HealthCanvas").GetComponent<Canvas>();
        GetComponent<Combat>().SetupHealthBar(canvas, Camera.main);
    }

    public void Update()
    {
        Attack();
    }

    public void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position + new Vector3(0, 1, 0) + transform.forward, attackRange, attackMask);

            foreach (Collider enemy in hitEnemies)
            {
                if (_animator.GetCurrentAnimatorStateInfo(1).IsName("Idle"))
                {
                    _animator.SetInteger("ComboHit", 1);
                }else if (_animator.GetCurrentAnimatorStateInfo(1).IsName("Combo1"))
                {
                    _animator.SetInteger("ComboHit", 2);
                }else if (_animator.GetCurrentAnimatorStateInfo(1).IsName("Combo2"))
                {
                    _animator.SetInteger("ComboHit", 3);
                }else if (_animator.GetCurrentAnimatorStateInfo(1).IsName("Combo3"))
                {
                    _animator.SetInteger("ComboHit", 4);
                }else if (_animator.GetCurrentAnimatorStateInfo(1).IsName("Combo4"))
                {
                    _animator.SetInteger("ComboHit", 1);
                }

                enemy.GetComponent<IDamageable>().OnTakeDamage(-1, attackDamage);
                break;
            }

            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnTakeDamage(int clientid, int Damage)
    {
        TakeDamage(Damage);
    }

    [ObserversRpc]
    public void TakeDamage(int Damage)
    {
        Health -= Damage;
        HealthBar.SetProgress(Health / MaxHealth, 3);

        if (Health < 0)
        {
            OnDied();
        }
    }

    private void OnDied()
    {   
        // Make sure this is called only once

        Destroy(gameObject, 1f);
        Destroy(HealthBar.gameObject, 1f);

        gameManager.DecrementNPC(gameObject);
    }

    public void SetupHealthBar(Canvas Canvas, Camera Camera)
    {
        HealthBar.transform.SetParent(Canvas.transform);
        if (HealthBar.TryGetComponent<FaceCamera>(out FaceCamera faceCamera))
        {
            faceCamera.Camera = Camera;
        }
    }
}