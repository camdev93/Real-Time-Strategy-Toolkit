using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    public Vector3 offset;
    public float zoomSpeed = 4f;
    public float min = 5f, max = 15f;
    public float pitch = 2f, yawSpeed = 100f;
    private float currentZoom = 10f, yawInput = 0f;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, min, max);

        yawInput -= Input.GetAxis("Horizontal") * yawSpeed * Time.deltaTime;
    }

    void LateUpdate()
    {
        transform.position = target.position - offset * currentZoom;
        transform.LookAt(target.position + Vector3.up * pitch);

        transform.RotateAround(target.position, Vector3.up, yawInput);
    }
}
