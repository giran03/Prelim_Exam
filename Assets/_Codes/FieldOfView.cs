using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject playerRef;
    [SerializeField] TMP_Text hideLabel;
    [SerializeField] GameObject exclamationMark;

    [Header("FOV Configs")]
    [SerializeField] float radius; [Range(0, 360)]
    [SerializeField] float angle;
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstructionMask;
    [SerializeField] bool canSeePlayer;

    [Header("Random Movement Configs")]
    [SerializeField] float range; //radius of sphere

    public NavMeshAgent npcAgent;
    bool canDamage = true;
    float defaultSpeed;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        npcAgent = GetComponent<NavMeshAgent>();
        StartCoroutine(FOVRoutine());
        defaultSpeed = npcAgent.speed;
    }

    private IEnumerator FOVRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            FieldOfViewCheck();
        }
    }

    private void Update()
    {
        if (canSeePlayer)
        {
            ChasePlayer();
        }
        else
        {

            Patrol();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }

    void Patrol()
    {
        if (!npcAgent.isActiveAndEnabled) return;

        if (npcAgent.remainingDistance <= npcAgent.stoppingDistance)
        {
            npcAgent.speed = defaultSpeed;
            Vector3 point;
            if (RandomPoint(transform.position, range, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                npcAgent.SetDestination(point);
            }
            // Disable Alert info
            hideLabel.gameObject.SetActive(false);
            exclamationMark.SetActive(false);
        }
    }

    void ChasePlayer()
    {
        if (!npcAgent.isActiveAndEnabled) return;

        //sfx
        AudioManager.Instance.PlaySFX("Alert", Camera.main.transform.position, true);

        transform.LookAt(playerRef.transform.position);
        Vector3 moveTo = Vector3.MoveTowards(transform.position, playerRef.transform.position, 5f);

        // increase npc move speed
        npcAgent.speed = 7.5f;

        npcAgent.SetDestination(moveTo);

        if (npcAgent.remainingDistance < 2.25f && canDamage)
            StartCoroutine(PlayerDamage());

        // Alert NPC info
        hideLabel.gameObject.SetActive(true);
        exclamationMark.SetActive(true);
    }

    // Damage Tick
    IEnumerator PlayerDamage()
    {
        Debug.Log("Player Damage!");

        var lives = GameManager.Instance.remainingLives;
        canDamage = false;

        if (lives > 0)
        {
            GameManager.Instance.remainingLives--;
            GameManager.Instance.PlayerHit();
        }
        else
            Debug.Log("Player has no lives left!");

        yield return new WaitForSeconds(1);
        canDamage = true;
    }

    IEnumerator ChaseTime()
    {
        Debug.Log("Chasing");
        Debug.Log("Coroutine started!");
        yield return new WaitForSeconds(3.0f);
        if (!canSeePlayer)
        {
            Debug.Log("Patrol");
            Patrol();
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}