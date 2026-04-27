using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private CameraMovement playerCamera;
    private PlayerScript player;

    public List<CreatureAI> creatureList;
    [SerializeField] private GameObject creaturePrefab;
    [SerializeField] private GameObject holoCreature;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask creatureLayerMask;

    [SerializeField] private List<TimeArrow> TimescaleArrows;
    [SerializeField] private GameObject TimescalePanel;
    [SerializeField] private bool timePanelShowing = true;
    [SerializeField] private GameObject GraphPanel;
    [SerializeField] private bool graphPanelShowing = true;
    private int speedIndex = 2;

    /// <summary>
    /// To be multiplied with time to influence drain speed of creature hunger. Higher values = faster depletion. 0 disables the system.
    /// </summary>
    public float hungerDrainRate = 0.2f;
    public float hungryRangeSlice = 2;
    public float healthRegenRate = 0.5f;
    public float mateCooldownRate = 0.2f;

    private void Awake()
    {
        creatureList = new List<CreatureAI>();
        playerCamera = FindAnyObjectByType<CameraMovement>();
        player = playerCamera.GetComponent<PlayerScript>();
    }

    private void Start()
    {
        foreach (TimeArrow arrow in TimescaleArrows)
        {
            arrow.gameObject.SetActive(false);
        }

        for (int i = 0; i < speedIndex; i++)
        {
            TimescaleArrows[i].gameObject.SetActive(true);
            if (i == speedIndex - 1)
            {
                Time.timeScale = TimescaleArrows[i].TimeScale;
            }
        }
    }

    public void ToggleTimescalePanel()
    {
        if (timePanelShowing)
        {
            TimescalePanel.GetComponent<Animator>().Play("TimePanelExit");
            timePanelShowing = false;
        }
        else
        {
            TimescalePanel.GetComponent<Animator>().Play("TimePanelEnter");
            timePanelShowing = true;
        }
    }

    public void ToggleGraphPanel()
    {
        if (graphPanelShowing)
        {
            GraphPanel.GetComponent<Animator>().Play("GraphExit");
            graphPanelShowing = false;
        }
        else
        {
            GraphPanel.GetComponent<Animator>().Play("GraphEnter");
            graphPanelShowing = true;
        }
    }

    public void ChangeGameSpeed(bool SpeedUp)
    {
        if (SpeedUp && speedIndex < TimescaleArrows.Count)
        {
            speedIndex++;
        }
        else if (speedIndex > 1)
        {
            speedIndex--;
        }

        foreach (TimeArrow arrow in TimescaleArrows)
        {
            arrow.gameObject.SetActive(false);
        }

        for (int i = 0; i < speedIndex; i++)
        {
            TimescaleArrows[i].gameObject.SetActive(true);
            if (i == speedIndex - 1)
            {
                Time.timeScale = TimescaleArrows[i].TimeScale;
            }
        }
    }

    public CreatureBase SpawnCreature(Vector3 loc, Quaternion rot)
    {
        return Instantiate(creaturePrefab, loc, rot).GetComponent<CreatureBase>();
    }

    public void RegisterCreature(CreatureAI creature)
    {
        creatureList.Add(creature);
    }
    public void UnregisterCreature(CreatureAI creature)
    {
        creatureList.Remove(creature);
    }

    public void RemoveCreature(CreatureBase creature)
    {
        UnregisterCreature(creature.aiComponent);
        foreach (CreatureAI creatureAI in creatureList)
        {
            creatureAI.creatureBase.RemoveCreature(creature);
        }
        Destroy(creature.gameObject);
    }

    public GameObject GetHoloCreature() { return holoCreature; }
    public LayerMask GetGroundLayerMask() { return groundLayerMask; }
    public LayerMask GetCreatureLayerMask() { return creatureLayerMask; }
    public CameraMovement GetPlayerCamera() { return playerCamera; }
}
