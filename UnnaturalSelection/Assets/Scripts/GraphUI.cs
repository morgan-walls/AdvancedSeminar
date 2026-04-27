using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GraphUI : MonoBehaviour
{
    private GameManager gameManager;
    public float sampleInterval = 0.5f;
    public int maxPoints = 50;

    private List<float> values = new List<float>();
    private float timer;

    private LineRenderer line;

    float width = 670;   // graph width
    float height = 250;   // graph height

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
    }

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= sampleInterval)
        {
            timer = 0f;

            AddValue(gameManager.creatureList.Count);
            DrawGraph();
        }
    }

    void AddValue(float value)
    {
        /*
        if (values.Count >= maxPoints)
        {
            values.RemoveAt(0);
        }
        */

        values.Add(value);
    }

    void DrawGraph()
    {
        line.positionCount = values.Count;

        float maxValue = Mathf.Max(values.ToArray());
        if (maxValue <= 0) maxValue = 0.1f;

        for (int i = 0; i < values.Count; i++)
        {
            float x = (i / (float)values.Count) * width;
            float y = (values[i] / maxValue) * height;

            line.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}