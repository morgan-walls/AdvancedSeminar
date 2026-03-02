using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private CameraMovement playerCamera;
    private PlayerScript player;

    private List<CreatureAI> creatureList;
    [SerializeField] private GameObject creaturePrefab;
    [SerializeField] private GameObject holoCreature;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask creatureLayerMask;

    /// <summary>
    /// To be multiplied with time to influence drain speed of creature hunger. Higher values = faster depletion. 0 disables the system.
    /// </summary>
    public float hungerDrainRate = 0.2f;
    public float hungryRangeSlice = 2;

    private void Awake()
    {
        creatureList = new List<CreatureAI>();
        playerCamera = FindAnyObjectByType<CameraMovement>();
        player = playerCamera.GetComponent<PlayerScript>();
    }

    private void Start()
    {
    }

    public void SpawnCreature(Vector3 loc, Quaternion rot)
    {
        Instantiate(creaturePrefab, loc, rot);
    }

    public void RegisterCreature(CreatureAI creature)
    {
        creatureList.Add(creature);
    }
    public void RemoveCreature(CreatureAI creature)
    {
        creatureList.Remove(creature);
    }

    public GameObject GetHoloCreature() { return holoCreature; }
    public LayerMask GetGroundLayerMask() { return groundLayerMask; }
    public LayerMask GetCreatureLayerMask() { return creatureLayerMask; }
    public CameraMovement GetPlayerCamera() { return playerCamera; }
}
