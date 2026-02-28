using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private GameManager gameManager;

    private GameObject holoCreature;
    private bool holoActive = false;

    private CreatureAI creatureMarked;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        holoCreature = gameManager.GetHoloCreature();
        if (holoCreature != null)
        {
            holoCreature.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Holo Creature not assigned in editor!");
        }
    }


    private void Update()
    {
        CheckSpawnMouseInputs();
        CheckDeleteMouseInputs();
        UpdateQuickActionTimer();
    }

    private void CheckDeleteMouseInputs()
    {
        if (Input.GetMouseButton(1))
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, gameManager.GetCreatureLayerMask()))
            {
                if (hit.collider.TryGetComponent<CreatureAI>(out CreatureAI creature))
                {
                    if (creatureMarked == null)
                    {
                        creatureMarked = creature;
                        creatureMarked.MarkForDeletion();
                    }
                    else if (creature != creatureMarked)
                    {
                        creatureMarked.ResetMaterial();
                        creatureMarked = creature;
                        creature.MarkForDeletion();
                    }
                }
            }
            else if (creatureMarked != null)
            {
                creatureMarked.ResetMaterial();
                creatureMarked = null;
            }

            if (Input.GetKey(KeyCode.LeftShift) && creatureMarked != null)
            {
                RemoveMarkedCreature();
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            if (creatureMarked != null)
            {
                RemoveMarkedCreature();
            }
        }
    }

    private void CheckSpawnMouseInputs()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, gameManager.GetGroundLayerMask()))
            {
                holoActive = true;
                holoCreature.SetActive(true);
                holoCreature.transform.position = hit.point;
            }
        }
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, gameManager.GetGroundLayerMask()))
            {
                if (holoActive == false)
                {
                    holoCreature.SetActive(true);
                    holoActive = true;
                }
                holoCreature.transform.position = hit.point + new Vector3(0.0f, 1.0f);
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    TryCreatureSpawn();
                }
            }
            else
            {
                holoActive = false;
                holoCreature.SetActive(false);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            TryCreatureSpawn();
        }
    }

    private void RemoveMarkedCreature()
    {
        if (quickActionTimer <= 0)
        {
            gameManager.RemoveCreature(creatureMarked);
            Destroy(creatureMarked.gameObject);
            creatureMarked = null;
            StartQuickActionTimer();
        }
    }

    private void TryCreatureSpawn()
    {
        if (quickActionTimer <= 0)
        {
            if (holoCreature.activeInHierarchy)
            {
                gameManager.SpawnCreature(holoCreature.transform.position, holoCreature.transform.rotation);
                holoCreature.SetActive(false);
                holoActive = false;
            }
        }
    }

    private void StartQuickActionTimer() { quickActionTimer = QUICK_ACTION_SPEED; }

    private void UpdateQuickActionTimer()
    {
        if (quickActionTimer > 0)
        {
            quickActionTimer -= Time.deltaTime;
        }
        else if (quickActionTimer < 0)
        {
            quickActionTimer = 0;
        }
    }    

    private const float QUICK_ACTION_SPEED = 0.01f;
    private float quickActionTimer = 0.0f;
}
