using UnityEngine;

public class FoodSource : MonoBehaviour
{
    /// <summary>
    /// Amount of food given per individual food piece
    /// </summary>
    [SerializeField] private float foodValue = 5.0f;
    /// <summary>
    /// Rate (compared to Food Grow Time) at which food is regrown
    /// </summary>
    [SerializeField] private float foodGrowRate = 1.0f;
    /// <summary>
    /// How much time it takes to replenish 1 food piece
    /// </summary>
    [SerializeField] private float foodGrowTime = 10.0f;
    private float currentGrowTime = 0.0f;

    private const int MAX_FOOD_PIECES = 5;
    private int currentFoodPieces;

    private void Start()
    {
        currentFoodPieces = MAX_FOOD_PIECES;
    }

    private void Update()
    {
        CheckFoodRegrowth();
    }

    public float TakeFood()
    {
        if (currentFoodPieces > 0)
        {
            currentFoodPieces--;
            return foodValue;
        }
        else
        {
            return 0.0f;
        }
    }

    private void CheckFoodRegrowth()
    {
        if (currentFoodPieces < MAX_FOOD_PIECES)
        {
            if (currentGrowTime >= foodGrowTime)
            {
                RegrowFood();
            }
            else
            {
                currentGrowTime += Time.deltaTime * foodGrowRate;
            }
        }
    }

    private void RegrowFood()
    {
        currentFoodPieces++;
    }
}
