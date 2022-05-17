using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClickPoint : MonoBehaviour
{
    GameObject player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= (player.GetComponent<NavMeshAgent>().stoppingDistance + 2))
        {
            Destroy(this.gameObject);
        }
    }
}
