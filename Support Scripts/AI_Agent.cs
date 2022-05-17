using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class AI_Agent : MonoBehaviour
{
    public bool activatePatrol;
    [HideInInspector]
    public static NavMeshAgent agent;
    GameObject[] points;
    Vector3 target;
    Camera cam;
    public Text patrolStateText;
    public GameObject clickPoint;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        points = GameObject.FindGameObjectsWithTag("PatrolPoint");
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        patrolStateText = GameObject.Find("INFO").GetComponentInChildren<Text>();

        activatePatrol = false;

        if (!activatePatrol)
        {
            patrolStateText.text = "Activate Patrolling";
        }
        else
        {
            patrolStateText.text = "Diactivate Patrolling";
        }

        int point = Random.Range(0, points.Length);
        target = points[point].transform.position;
    }

    void Update()
    {
        if (!activatePatrol)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    // Destroys any click points before another one gets instantiated
                    Destroy(GameObject.Find("ClickPoint(Clone)"));

                    //____
                    agent.SetDestination(hit.point);
                    GameObject _clickPoint = Instantiate(clickPoint, hit.point, hit.transform.rotation);
                }
            }
        }
        else if(activatePatrol)
        {
            if (Vector3.Distance(transform.position, target) < 2f)
            {
                UpdateDestination();
            }
            else
            {
                agent.SetDestination(target);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PatrolState();
        }
    }

    void UpdateDestination()
    {
        int point = Random.Range(0, points.Length);
        target = points[point].transform.position;
    }

    public void PatrolState()
    {
        if (!activatePatrol)
        {
            activatePatrol = true;

            patrolStateText.text = "Diactivate Patrolling";
        }
        else if(activatePatrol)
        {
            activatePatrol = false;

            patrolStateText.text = "Activate Patrolling";
        }
    }
}
