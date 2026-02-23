using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private CameraMovement playerCamera;
    private PlayerScript player;

    private List<CreatureAI> creatureList;
    [SerializeField] private GameObject creaturePrefab;

    private void Awake()
    {
        creatureList = new List<CreatureAI>();
    }

    private void Start()
    {
        playerCamera = FindAnyObjectByType<CameraMovement>();
        player = playerCamera.GetComponent<PlayerScript>();
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
}
