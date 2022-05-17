using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTwo : MonoBehaviour
{
    [HideInInspector]
    [Range(3f, 100f)]
    public float endSphere;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 5);
    }
}
