using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] GameObject playerCapsule;
    [SerializeField] GameObject clickEffect;
    GameObject chestCover;
    FieldOfView npc;
    float defaultCoverRotation;
    bool hidden;
    bool canHide;
    RaycastHit mouseHit;
    bool pressed;

    private void Start()
    {
        GameManager.Instance.remainingLives = 3;
        npc = Object.FindObjectOfType<FieldOfView>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray moveToPos = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(moveToPos, out mouseHit) && navMeshAgent.enabled)
                navMeshAgent.SetDestination(mouseHit.point);

            // click effect
            var temp = Instantiate(clickEffect, mouseHit.point, clickEffect.transform.rotation);
            Destroy(temp, .4f);

            //sfx
            AudioManager.Instance.PlaySFX("Click", Camera.main.transform.position);
        }

        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1) && canHide)
        {
            pressed = !pressed;
            playerCapsule.SetActive(!playerCapsule.activeSelf);
            navMeshAgent.isStopped = !navMeshAgent.isStopped;
            if (!playerCapsule.activeSelf && chestCover != null)
                navMeshAgent.velocity = Vector3.zero;
            if (pressed)
            {
                chestCover.transform.localRotation = Quaternion.Euler(0, 0, 0);
                //sfx
                AudioManager.Instance.PlaySFX("MC_Close", Camera.main.transform.position);
            }
            else
            {
                chestCover.transform.localRotation = Quaternion.Euler(0, 0, -45);
                //sfx
                AudioManager.Instance.PlaySFX("MC_Open", Camera.main.transform.position);
            }
        }
    }

    private void LateUpdate()
    {
        if (navMeshAgent.velocity.sqrMagnitude > Mathf.Epsilon)
            transform.rotation = Quaternion.LookRotation(navMeshAgent.velocity.normalized);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            Debug.Log("Level Finished!");
            StartCoroutine(Finish());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Hide"))
        {
            chestCover = other.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
            canHide = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Hide"))
        {
            canHide = false;
            chestCover = null;
        }
    }

    IEnumerator Finish()
    {
        AudioManager.Instance.PlaySFX("Finish", Camera.main.transform.position);
        npc.npcAgent.enabled = false;
        yield return new WaitForSeconds(1f);
        SceneHandler.Instance.FinishLevel();
    }
}
