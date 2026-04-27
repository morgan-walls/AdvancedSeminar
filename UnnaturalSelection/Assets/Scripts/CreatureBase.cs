using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CreatureBase : MonoBehaviour
{
    private GameManager gameManager;
    public CreatureAI aiComponent;
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
    [SerializeField] private Scrollbar mateScrollBar;
    [SerializeField] private TextMeshProUGUI statsText;

    [SerializeField] private List<FoodSource> FoodInRange;
    private bool movingToFood = false;
    [SerializeField] private List<FoodSource> FoodInInteractionRange;

    [SerializeField] private List<CreatureBase> creaturesInRange;
    [SerializeField] private List<CreatureBase> creaturesInInteractionRange;


    [SerializeField] private GameObject DetectionRange;
    [SerializeField] private GameObject InteractionRange;
    private float detectionRangeSize = 20;
    private float interactionRangeSize = 5;

    public bool readyToMate = true;
    [SerializeField] private float mateCooldownTimer = 10.0f;
    [SerializeField] private float mateCooldown = 0.0f;

    //Creature stats
    public float speedMultiplier = 1.0f;
    public float reproductionMultiplier = 1.0f;
    public float perceptionMultiplier = 1.0f;
    public float hungerMultiplier = 1.0f;

    public float MUTATION_CHANCE = 10.0f;
    public float MUTATION_CHANGE_MAX = 0.5f;

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
        FoodInInteractionRange = new List<FoodSource>();
        creaturesInRange = new List<CreatureBase>();
        creaturesInInteractionRange = new List<CreatureBase>();
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

    public void ApplyStatChanges()
    {
        if (aiComponent != null)
        {
            aiComponent.agent.speed *= speedMultiplier;
        }
        detectionRangeSize *= perceptionMultiplier; interactionRangeSize *= perceptionMultiplier;
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
        mateScrollBar.size = 1 - mateCooldown / mateCooldownTimer;
        statsText.text = "Speed: " + Mathf.Round(speedMultiplier * 100f) * 0.01f + "x\nReproduction: " + Mathf.Round(reproductionMultiplier * 100f) * 0.01f + "x\nHunger: " + Mathf.Round(hungerMultiplier * 100f) * 0.01f + "x\nPerception: " + Mathf.Round(perceptionMultiplier * 100f) * 0.01f + "x";

        canvasObject.transform.LookAt(gameManager.GetPlayerCamera().transform);
    }

    private void FixedUpdate()
    {
        CheckHunger();
        UpdateRanges();
        CheckReproduction();
    }

    private void CheckReproduction()
    {
        if (readyToMate && !isHungry)
        {
            foreach (CreatureBase creature in creaturesInInteractionRange)
            {
                if (creature.readyToMate && !creature.isHungry)
                {
                    CreatureBase newCreature = gameManager.SpawnCreature(transform.position, transform.rotation);
                    CheckForMutations(newCreature);
                    MateCooldown();
                    creature.MateCooldown();
                    break;
                }
            }
        }
        if (!isHungry && mateCooldown > 0)
        {
            mateCooldown -= Time.deltaTime * gameManager.mateCooldownRate * reproductionMultiplier;
            if (mateCooldown < 0)
            {
                mateCooldown = 0;
                readyToMate = true;
            }    
        }
    }

    public void CheckForMutations(CreatureBase newCreature)
    {
        newCreature.perceptionMultiplier = CheckStatMutation(perceptionMultiplier);
        newCreature.speedMultiplier = CheckStatMutation(speedMultiplier);
        newCreature.hungerMultiplier = CheckStatMutation(hungerMultiplier);
        newCreature.reproductionMultiplier = CheckStatMutation(reproductionMultiplier);
    }

    public float CheckStatMutation(float currentStat)
    {
        if (CheckForStatChange((int)MUTATION_CHANCE))
        {
            currentStat = RandomStatChange(currentStat);
        }

        return currentStat;
    }

    private bool CheckForStatChange(int max)
    {
        bool StatChanges = false;

        if (Random.Range(0, max) == 0)
        {
            StatChanges = true;
        }

        return StatChanges;
    }

    private float RandomStatChange(float value)
    {
        float newValue = Random.Range(value - value * MUTATION_CHANGE_MAX, value + value * MUTATION_CHANGE_MAX);
        return newValue;
    }

    public void MateCooldown()
    {
        mateCooldown = mateCooldownTimer;
        readyToMate = false;
    }

    private void CheckHunger()
    {
        if (!isHungry && hunger <= hungryStateStart)
        {
            isHungry = true;
        }
        if (isHungry)
        {
            if (FoodInInteractionRange.Count > 0)
            {
                AddFood(FindClosestFoodSource().TakeFood());
            }
            else if (!movingToFood)
            {
                if (FoodInRange.Count > 0)
                {
                    aiComponent.WanderToLocation(FindClosestFoodSource().transform.position);
                    movingToFood = true;
                }
            }
        }
        if (hunger > 0)
        {
            hunger = DrainStatOverTime(hunger);
            if (health < maxHealth)
            {
                health += Time.deltaTime * gameManager.healthRegenRate;
                if (health >  maxHealth)
                {
                    health = maxHealth;
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
                gameManager.RemoveCreature(this);
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
    }

    public void RemoveFoodSource(FoodSource foodSource)
    {
        if (FoodInRange.Contains(foodSource))
        {
            FoodInRange.Remove(foodSource);
        }
    }

    private float DrainStatOverTime(float stat)
    {
        return stat -= Time.deltaTime * gameManager.hungerDrainRate * 1/hungerMultiplier;
    }

    public void AddFood(float foodAmount)
    {
        hunger += foodAmount;
        if (hunger > maxHunger)
        {
            hunger = maxHunger;
        }
        if (hunger > hungryStateStart)
        {
            isHungry = false;
        }
        movingToFood = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FoodSource>(out FoodSource foodSource))
        {
            if (!FoodInRange.Contains(foodSource) && Vector3.Distance(transform.position, foodSource.transform.position) <= detectionRangeSize)
            {
                FoodDetected(foodSource);
            }
            else if (!FoodInInteractionRange.Contains(foodSource) && Vector3.Distance(transform.position, foodSource.transform.position) <= interactionRangeSize)
            {
                FoodInInteractionRange.Add(foodSource);
                AddFood(FindClosestFoodSource().TakeFood());
            }
        }
        else if (other.TryGetComponent<CreatureBase>(out CreatureBase creature))
        {
            if (!creaturesInRange.Contains(creature) && Vector3.Distance(transform.position, creature.transform.position) <= detectionRangeSize)
            {
                creaturesInRange.Add(creature);
            }
            if (!creaturesInInteractionRange.Contains(creature) && Vector3.Distance(transform.position, creature.transform.position) <= interactionRangeSize)
            {
                creaturesInInteractionRange.Add(creature);
            }
        }
    }

    private void UpdateRanges()
    {
        List<FoodSource> tempFoodList = new List<FoodSource>();

        //Food sources
        
        foreach (FoodSource foodSource in FoodInInteractionRange)
        {
            if (Vector3.Distance(transform.position, foodSource.transform.position) > interactionRangeSize)
            {
                tempFoodList.Add(foodSource);
            }
        }
        foreach (FoodSource foodSource in tempFoodList)
        {
            FoodInInteractionRange.Remove(foodSource);
        }
        tempFoodList.Clear();
        foreach (FoodSource foodSource in FoodInRange)
        {
            if (Vector3.Distance(transform.position, foodSource.transform.position) > detectionRangeSize)
            {
                tempFoodList.Add(foodSource);
            }
        }
        foreach (FoodSource foodSource in tempFoodList)
        {
            FoodInRange.Remove(foodSource);
        }
        tempFoodList.Clear();

        //Creatures
        
        List<CreatureBase> tempCreatureList = new List<CreatureBase>();

        foreach (CreatureBase creature in creaturesInInteractionRange)
        {
            if (creature != null)
            {
                if (Vector3.Distance(transform.position, creature.transform.position) > interactionRangeSize)
                {
                    tempCreatureList.Add(creature);
                }
            }
        }
        foreach (CreatureBase creature in tempCreatureList)
        {
            creaturesInInteractionRange.Remove(creature);
        }
        tempCreatureList.Clear();
        foreach (CreatureBase creature in creaturesInRange)
        {
            if (creature != null)
            {
                if (Vector3.Distance(transform.position, creature.transform.position) > detectionRangeSize)
                {
                    tempCreatureList.Add(creature);
                }
            }
        }
        foreach (CreatureBase creature in tempCreatureList)
        {
            creaturesInRange.Remove(creature);
        }
        tempCreatureList.Clear();
    }

    public void RemoveCreature(CreatureBase creature)
    {
        if (creaturesInInteractionRange.Contains(creature))
        {
            creaturesInInteractionRange.Remove(creature);
        }    
        if(creaturesInRange.Contains(creature))
        {
            creaturesInRange.Remove(creature);
        }
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
