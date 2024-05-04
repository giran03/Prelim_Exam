using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] TMP_Text livesLabel;
    [SerializeField] GameObject hitLabel;
    [SerializeField] GameObject player;
    [HideInInspector] public int remainingLives;
    public static GameManager Instance;
    GameObject hitLabelSpawn;
    NavMeshAgent navMeshAgent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        navMeshAgent = player.GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
    }

    private void Update()
    {
        livesLabel.SetText("Lives: " + remainingLives);

        if (remainingLives <= 0)
        {
            Debug.Log("Game Over!");
            StartCoroutine(GameOverCountdown());
        }

        if (hitLabelSpawn != null)
            hitLabelSpawn.transform.Translate(2f * Time.deltaTime * Vector3.up);
    }

    public void PlayerHit()
    {
        Vector3 spawnOffset = player.transform.position;
        spawnOffset.y = 2.5f;

        hitLabelSpawn = Instantiate(hitLabel, spawnOffset, Quaternion.Euler(0, 45, 0));
        Destroy(hitLabelSpawn, 1.5f);
    }

    IEnumerator GameOverCountdown()
    {
        navMeshAgent.enabled = false;
        Debug.Log("Restarting Scene~");
        yield return new WaitForSeconds(2f);
        SceneHandler.Instance.RestartScene();
    }
}
