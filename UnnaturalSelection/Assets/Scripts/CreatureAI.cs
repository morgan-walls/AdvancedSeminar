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
            Wander();
        }

        if (targetOffset != null)
        {
            target.transform.position = targetOffset;
        }
    }

    private void Wander()
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
}
