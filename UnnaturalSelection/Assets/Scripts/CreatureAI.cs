using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private GameObject target;
    private GameManager gameManager;

    [SerializeField] private float wanderDistance = 5.0f;

    private float wanderTimer;
    [SerializeField] private float WANDER_COOLDOWN_MAX = 10.0f;
    [SerializeField] private float WANDER_COOLDOWN_MIN = 3.0f;

    [SerializeField] private GameObject face;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material baseFaceMaterial;
    [SerializeField] private Material deleteMaterial;

    private Vector3 targetOffset;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindAnyObjectByType<GameManager>();

        gameManager.RegisterCreature(this);

        GenerateWanderCooldown();
    }

    private void Update()
    {
        if (wanderTimer > 0)
        {
            wanderTimer -= Time.deltaTime;
        }
        else
        {
            RandomWander();
        }

        if (targetOffset != null)
        {
            target.transform.position = targetOffset;
        }
    }

    public void WanderToLocation(Vector3 location)
    {
        target.transform.position = location;

        agent.SetDestination(target.transform.position);

        GenerateWanderCooldown();

        targetOffset = target.transform.position;
    }

    private void RandomWander()
    {
        float timer = 0.0f;

        do
        {
            target.transform.localPosition = new Vector3(GetRandomDistance(), 0.0f, GetRandomDistance());

            timer += Time.deltaTime;
            if (timer >= 2.0f)
            {
                break;
            }
        } while (Vector2.Distance(target.transform.position, Vector2.zero) >= 15.0f);
        agent.SetDestination(target.transform.position);

        GenerateWanderCooldown();
        
        targetOffset = target.transform.position;
    }

    private void GenerateWanderCooldown()
    {
        wanderTimer = Random.Range(WANDER_COOLDOWN_MIN, WANDER_COOLDOWN_MAX);
        //Debug.Log("Wandering Begins, going to " + target.transform.position + "\nCooldown: " + wanderTimer);
    }

    private float GetRandomDistance()
    {
        return Random.Range(wanderDistance * -1.0f, wanderDistance);
    }

    public void ResetMaterial()
    {
        gameObject.GetComponent<MeshRenderer>().material = baseMaterial;
        face.GetComponent<MeshRenderer>().material = baseFaceMaterial;
    }

    public void MarkForDeletion()
    {
        gameObject.GetComponent<MeshRenderer>().material = deleteMaterial;
        face.GetComponent<MeshRenderer>().material = deleteMaterial;
    }
}
