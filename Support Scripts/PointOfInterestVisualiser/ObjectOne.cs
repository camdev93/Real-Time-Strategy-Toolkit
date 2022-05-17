using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOne : MonoBehaviour
{
    [HideInInspector]
    [Range(3f, 100f)]
    public float startSphere;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 5);
    }
}
