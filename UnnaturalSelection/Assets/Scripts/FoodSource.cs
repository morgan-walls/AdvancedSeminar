using TMPro;
using UnityEngine;

public class FoodSource : MonoBehaviour
{
    /// <summary>
    /// Amount of foodText given per individual foodText piece
    /// </summary>
    [SerializeField] private float foodValue = 5.0f;
    /// <summary>
    /// Rate (compared to Food Grow Time) at which foodText is regrown
    /// </summary>
    [SerializeField] private float foodGrowRate = 1.0f;
    /// <summary>
    /// How much time it takes to replenish 1 foodText piece
    /// </summary>
    [SerializeField] private float foodGrowTime = 10.0f;
    private float currentGrowTime = 0.0f;

    [SerializeField] TextMeshProUGUI foodText;

    private const int MAX_FOOD_PIECES = 5;
    [SerializeField] private int currentFoodPieces;

    private void Start()
    {
        currentFoodPieces = MAX_FOOD_PIECES;
    }

    private void Update()
    {
        CheckFoodRegrowth();
        UpdateFoodText();
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
                currentGrowTime = 0.0f;
            }
            else
            {
                currentGrowTime += Time.deltaTime * foodGrowRate;
            }
        }
    }

    private void UpdateFoodText()
    {
        foodText.text = "Food: " + currentFoodPieces;
    }

    private void RegrowFood() { currentFoodPieces++; }
}
