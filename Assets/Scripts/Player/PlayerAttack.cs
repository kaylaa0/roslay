using Cinemachine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Managing;
using StarterAssets;
using System;
using System.Globalization;
using UnityEditor.PackageManager;
using UnityEngine;
using FishNet.Managing.Server;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace Controls
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class PlayerAttack : NetworkBehaviour, IDamageable
    {
        [Header("Player")]
        [Tooltip("AttackCombo of character")]
        public float AttackCombo = 1.0f;

        [Tooltip("Is the character attacking state")]
        public bool IsAttacking = false;

        private Animator _animator;
        private StarterAssetsInputs _input;
        private CharacterController _characterController;
        private PlayerMovement _playerMovement;

        public float cooldownTime = 2f;
        private float nextFireTime = 0.1f;
        public static int noOfClicks = 0;
        float lastClickedTime = 0;
        float maxComboDelay = 1;
        int health = 100;
        int maxHealth;

        float attackRange = 0.5f;
        int attackPower = 26;

        public AudioClip hit_sound;
        public AudioClip block_sound;
        public AudioClip get_hit_sound;
        public AudioClip death_sound;
        public AudioClip swing_sound;

        private AudioSource hit;
        private AudioSource block;
        private AudioSource get_hit;
        private AudioSource death;
        private AudioSource swing;

        bool isInvincible = false;
        float invincibleTimeout = 0f;

        public float range;
        [SerializeField]
        // NPC tag
        private TagFieldAttribute attackMask;

        [Tooltip("Is the character blocking state")]
        public bool IsBlocking = false;

        private float blockTimeoutValue = 10f;
        private float maxBlockCount = 4;
        private float blockTimeout = 0f;
        private int blockCounter = 0;

        private PlayerBlockBar blockBar;
        private bool blockNeedsReset = false;


        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner)
            {
                this.GetComponent<PlayerAttack>().enabled = false;
            }
        }

        public void Start()
        {
            _input = GetComponent<StarterAssetsInputs>();
            _animator = GetComponent<Animator>();
            _characterController = GetComponent<CharacterController>();
            _playerMovement = GetComponent<PlayerMovement>();

            range = 4f;
            maxHealth = health;
            blockBar = GameObject.Find("Block Bar").GetComponent<PlayerBlockBar>();

            hit = gameObject.AddComponent<AudioSource>();
            hit.clip = hit_sound;
            hit.playOnAwake = false;

            block = gameObject.AddComponent<AudioSource>();
            block.clip = block_sound;
            block.playOnAwake = false;

            get_hit = gameObject.AddComponent<AudioSource>();
            get_hit.clip = get_hit_sound;
            get_hit.playOnAwake = false;

            death = gameObject.AddComponent<AudioSource>();
            death.clip = death_sound;
            death.playOnAwake = false;

            swing = gameObject.AddComponent<AudioSource>();
            swing.clip = swing_sound;
            swing.playOnAwake = false;

        }

        public void OnTakeDamage(int clientid,  int damage)
        {
            if (clientid == -1)
            {
                if (IsOwner)
                {
                    if (!isInvincible) {
                        TakeDamage(damage);
                    }
                }
            }
        }

        public void TakeDamage(int damage)
        {
            if (IsBlocking)
            {
                block.Play();
                // Trigger receive hit while blocked animation
                _animator.SetTrigger("HitWhileBlocking");
                // Reduce damage taken when blocking
                damage = Mathf.RoundToInt(damage * 0.3f);
                blockCounter++;

                if (blockCounter != 0 && blockCounter <= maxBlockCount)
                {
                    blockBar.SetProgress(1.0f - (float)blockCounter / maxBlockCount);
                }

                if (blockCounter >= maxBlockCount)
                {
                    StopBlock();
                    blockTimeout = blockTimeoutValue;
                    blockCounter = 0;
                    blockNeedsReset = true;
                }
      
                
            }
            else
            {
                get_hit.Play();
                _animator.SetTrigger("Hit");
            }

            health -= damage;
            // Get Health Bar and set playerprogressbar progress
            GameObject.Find("Health Bar").GetComponent<PlayerProgressBar>().SetProgress((float)health / (float)maxHealth, health);

            if (health < 0)
            {
                OnDied();
            }
        }

        public void OnDied()
        {
            death.Play();
            Respawn();
        }

        public void Respawn()
        {
            if (GetComponent<PlayerMovement>().enabled)
            {
                health = 100;
                GameObject.Find("Health Bar").GetComponent<PlayerProgressBar>().SetProgress(health / maxHealth, maxHealth);

                // Disable player movement
                _playerMovement.enabled = false;
                _animator.enabled = false;
                _characterController.enabled = false;
                isInvincible = true;
                invincibleTimeout = 5f;
                transform.position = GameObject.Find("Spawn").transform.position;
                _playerMovement.enabled = true;
                _animator.enabled = true;
                _characterController.enabled = true;
            }
        }

        public void NextAttack()
        {
            if(!IsOwner)
            {
                return;
            }
            if (noOfClicks >= 1  && _animator.GetCurrentAnimatorStateInfo(1).IsName("Combo1"))
            {
                swing.Play();
                _animator.SetInteger("ComboHit", 2);
            }
            if (noOfClicks >= 1 && _animator.GetCurrentAnimatorStateInfo(1).IsName("Combo2"))
            {
                swing.Play();
                _animator.SetInteger("ComboHit", 3);
            }
            if (noOfClicks >= 1 && _animator.GetCurrentAnimatorStateInfo(1).IsName("Combo3"))
            {
                swing.Play();
                _animator.SetInteger("ComboHit", 4);
            }
            if (_animator.GetCurrentAnimatorStateInfo(1).IsName("Combo4"))
            {
                swing.Play();
                _animator.SetInteger("ComboHit", 0);
            }
            noOfClicks = 0;

        }   

        private void Update()
        {
            if (invincibleTimeout > 0f)
            {
                invincibleTimeout -= Time.deltaTime;
                if (invincibleTimeout < 0f)
                {
                    isInvincible = false;
                }
            }

            if (Time.time - lastClickedTime > maxComboDelay)
            {
                noOfClicks = 0;
                _animator.SetInteger("ComboHit", 0);
            }

            //cooldown time
            if (Time.time - lastClickedTime > nextFireTime)
            {
                Attack();
            }

            if (_input.interact)
            {
                CheckInteract();
                _input.interact = false;
            }

            if(blockNeedsReset && blockCounter == 0 && blockTimeout <= 0.5f)
            {
                blockNeedsReset = false;
                blockBar.SetProgress(1.0f);
            }

            if (blockTimeout > 0)
            {
                blockTimeout -= Time.deltaTime;
                if (blockTimeout < 0)
                {
                    blockCounter = 0;
                }
            }

            if (_input.block)
            {
                Block();
            }
            else if (IsBlocking)
            {
                StopBlock();
            }
        }

        public void StopBlock()
        {
            if (!IsOwner)
            {
                return;
            }

            IsBlocking = false;
            _animator.SetBool("IsBlocking", false);
        }

        public void Block()
        {
            if (!IsOwner || IsAttacking)
            {
                return;
            }

            if(blockTimeout <= 0)
            {
                IsBlocking = true;
                _animator.SetBool("IsBlocking", true);
            }
        }

        private void CheckInteract()
        {
            Ray r = new Ray(transform.position + new Vector3(0,1,0) - transform.forward, transform.forward);
            // get second hit as first is itself
            RaycastHit[] raycastHit = Physics.RaycastAll(r, range);

            for (int i = 0; i < raycastHit.Length; i++)
            {
                if (raycastHit[i].collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact(LocalConnection.ClientId);
                    break;
                }
            }
        }

        private void CheckAttackCollider()
        {
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position + new Vector3(0, 1, 0) + transform.forward, attackRange);

            foreach (Collider enemy in hitEnemies)
            {
                if(enemy.gameObject.TryGetComponent(out IDamageable damagable))
                {
                    if (enemy.gameObject.CompareTag("NPC"))
                    {
                        hit.Play();
                        enemy.GetComponent<IDamageable>().OnTakeDamage(LocalConnection.ClientId, attackPower);
                        enemy.GetComponent<Navigation>().SetPlayer(gameObject);
                    }
                }
               
            }
        }

        private void Attack()
        {
            if (_input.fire1)
            {
                noOfClicks = Mathf.Clamp(noOfClicks, 0, 4);
                lastClickedTime = Time.time;
                if (_animator.GetCurrentAnimatorStateInfo(1).IsName("Idle"))
                {
                    swing.Play();
                    _animator.SetInteger("ComboHit", 1);
                }
                CheckAttackCollider();
                noOfClicks++;
            }
        }

    }
}