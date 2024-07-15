using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : NetworkBehaviour
{
    GameObject[] npcs;
    bool gameStarted = false;

    public AudioClip theme_sound;


    private AudioSource theme;


    private void Start()
    {
        NPCDictionary.Initialize();
        TileDictionary.Initialize();
        ObjectDictionary.Initialize();
        ItemDictionary.Initialize();

        npcs = GameObject.FindGameObjectsWithTag("NPC");

        theme = gameObject.AddComponent<AudioSource>();
        theme.clip = theme_sound;
        // quiet
        theme.volume = 0.4f;
        theme.loop = true;
        theme.Play();
    }

    private void Update()
    {
        if (gameStarted && npcs.Length <= 1)
        {
            // switch scene to win scene
            SwitchWinScene();
        }

        npcs = GameObject.FindGameObjectsWithTag("NPC");

        // If kill_all kill pressed end game
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // Damage 1000 to all npcs in whole map

            Collider[] hitEnemies = Physics.OverlapSphere(new Vector3(0, 0, 0), float.PositiveInfinity);

            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.gameObject.TryGetComponent(out IDamageable damagable))
                {
                    if (enemy.gameObject.CompareTag("NPC"))
                    {
                        enemy.GetComponent<IDamageable>().OnTakeDamage(LocalConnection.ClientId, 99999999);
                        Debug.Log(gameObject);
                        enemy.GetComponent<Navigation>().SetPlayer(gameObject);
                    }
                }

            }
        }
    }

    [ObserversRpc]
    public void SwitchWinScene()
    {
        // Remove Network Manager
        Destroy(GameObject.Find("NetworkManager"));
        // Unlock cursor
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        // switch scene to win scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("WinGame");
    }

    public void IncrementNPC(NPC npc)
    {
        if(!gameStarted)
        {
            gameStarted = true;
        }

        npcs = GameObject.FindGameObjectsWithTag("NPC");
    }

    public void DecrementNPC(GameObject npc)
    {
        npcs = GameObject.FindGameObjectsWithTag("NPC");
    }




}

public enum GameDifficulty
{
    Easy,
    Normal,
    Hard
}
