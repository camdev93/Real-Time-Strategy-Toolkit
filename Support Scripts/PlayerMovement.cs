using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public Camera cam;
    NavMeshAgent agent;
    [Range(0, 5f)]
    public float playerSphereSize;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        cam = GameObject.Find("RTS_Camera").GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                agent.destination = hit.point;
                Debug.Log(hit.transform.position);
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, playerSphereSize);
    }
}
