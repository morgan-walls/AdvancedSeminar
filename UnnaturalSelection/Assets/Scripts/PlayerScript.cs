using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryCreatureSpawn();
        }
    }

    private void TryCreatureSpawn()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.forward, Color.red, 3.0f);
            Debug.Log("Ray Hit: " + hit.point);
            gameManager.SpawnCreature(hit.point + new Vector3(0.0f, 1.0f, 0.0f), new Quaternion(0.0f, transform.eulerAngles.y + 180.0f, 0.0f, 1.0f));
        }
    }
}
