using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureBase : MonoBehaviour
{
    private GameManager gameManager;
    private CreatureAI aiComponent;
    [SerializeField] private Collider detectorRange;
    private int triggersHit = 0;

    [SerializeField] private float maxHunger = 10.0f;
    [SerializeField] private float hunger = 10.0f;
    [SerializeField] private float hungryStateStart;
    private bool isHungry = false;

    [SerializeField] private float maxHealth = 20.0f;
    [SerializeField] private float health = 20.0f;

    [SerializeField] private GameObject canvasObject;
    [SerializeField] private Scrollbar hungerScrollBar;
    [SerializeField] private Scrollbar healthScrollBar;

    [SerializeField] private List<FoodSource> FoodInRange;
    private bool movingToFood = false;


    [SerializeField] private GameObject DetectionRange;
    [SerializeField] private GameObject InteractionRange;
    private const float detectionRangeSize = 20;
    private const float interactionRangeSize = 5;

    /// <summary>
    /// all ranges for the creature, ordered from biggest to smallest
    /// </summary>
    private enum CreatureRanges
    {
        DetectionRange = 1,
        InteractionRange,
    }

    private void Awake()
    {
        FoodInRange = new List<FoodSource>();
    }

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        aiComponent = GetComponent<CreatureAI>();

        hungryStateStart = maxHunger / gameManager.hungryRangeSlice;
        hunger = maxHunger;

        DetectionRange.transform.localScale = Vector3.zero;
        StartCoroutine(ExpandRange(DetectionRange, detectionRangeSize));

        InteractionRange.transform.localScale = Vector3.zero;
        StartCoroutine(ExpandRange(InteractionRange, interactionRangeSize));
    }

    IEnumerator ExpandRange(GameObject range, float rangeSize)
    {
        range.transform.localScale = Vector3.zero;

        do
        {
            range.transform.localScale += Vector3.one;
            yield return null;
        } while (range.transform.localScale.x < rangeSize);

        if (range.transform.localScale.x > rangeSize)
        {
            range.transform.localScale = new Vector3(rangeSize, rangeSize, rangeSize);
        }
    }

    private void Update()
    {
        hungerScrollBar.size = hunger / maxHunger;
        healthScrollBar.size = health / maxHealth;

        canvasObject.transform.LookAt(gameManager.GetPlayerCamera().transform);
    }

    private void FixedUpdate()
    {
        CheckHunger();
    }

    private void CheckHunger()
    {
        if (!isHungry && hunger <= hungryStateStart)
        {
            isHungry = true;

            Debug.Log("Creature is Hungry");
        }
        if (hunger > 0)
        {
            hunger = DrainStatOverTime(hunger);
            if (hunger <= hungryStateStart)
            {
                if (!movingToFood)
                {
                    if (FindClosestFoodSource() != null)
                    {
                        aiComponent.WanderToLocation(FindClosestFoodSource().transform.position);
                        movingToFood = true;

                        Debug.Log("Wandering to nearest Food");
                    }
                }
            }
        }
        else if (hunger < 0)
        {
            hunger = 0;
            health = DrainStatOverTime(health);
        }
        else if (hunger == 0)
        {
            health = DrainStatOverTime(health);
            if (health <= 0)
            {
                gameManager.RemoveCreature(aiComponent);
                Destroy(gameObject);
            }
        }
    }

    public void FoodDetected(FoodSource foodSource)
    {
        if (!FoodInRange.Contains(foodSource))
        {
            FoodInRange.Add(foodSource);
            if (isHungry)
            {
                aiComponent.WanderToLocation(foodSource.transform.position);
            }
        }

        Debug.Log("Food Source Found");
    }

    public void RemoveFoodSource(FoodSource foodSource)
    {
        if (FoodInRange.Contains(foodSource))
        {
            FoodInRange.Remove(foodSource);
        }

        Debug.Log("Food Source Away");
    }

    private float DrainStatOverTime(float stat)
    {
        return stat -= Time.deltaTime * gameManager.hungerDrainRate;
    }

    public void AddFood(float foodAmount)
    {
        hunger += foodAmount;
        if (hunger > maxHunger)
        {
            hunger = maxHunger;
        }
        movingToFood = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger Entered");
        if (other.TryGetComponent<FoodSource>(out FoodSource foodSource))
        {
            if (!FoodInRange.Contains(foodSource))
            {
                FoodDetected(foodSource);
            }
            else
            {
                AddFood(FindClosestFoodSource().TakeFood());
            }
            /*
            if (triggersHit == (int)CreatureRanges.DetectionRange)
            {
                FoodDetected(foodSource);
            }
            else if (triggersHit == (int)CreatureRanges.InteractionRange)
            {
                AddFood(FindClosestFoodSource().TakeFood());
            }
            */
            triggersHit++;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        triggersHit--;
    }

    private FoodSource FindClosestFoodSource()
    {
        if (FoodInRange.Count > 0)
        {
            FoodSource closestFood = new FoodSource();

            foreach (FoodSource foodSource in FoodInRange)
            {
                if (closestFood == null)
                {
                    closestFood = foodSource;
                }
                else
                {
                    if (Vector3.Distance(foodSource.transform.position, transform.position) < Vector3.Distance(closestFood.transform.position, transform.position))
                    {
                        closestFood = foodSource;
                    }
                }
            }

            return closestFood;
        }
        else
        {
            return null;
        }
    }
}
