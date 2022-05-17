using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayer : MonoBehaviour
{
    CharacterController controller;
    Camera cam;
    public float speed;

    float verticalLookRotation;

    [SerializeField]
    float mouseSensitivity;

    //Jumping
    Vector3 playerDir;

    [SerializeField]
    float jumpForce, gravity;

    [SerializeField]
    bool jumpEnabled;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = transform.GetChild(0).gameObject.GetComponent<Camera>();
    }

    void Update()
    {
        LookRotation();
        PlayerMovement();

        if (jumpEnabled)
        {
            Jump();
        }
    }

    void LookRotation()
    {
        Cursor.lockState = CursorLockMode.Locked;

        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
        cam.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void PlayerMovement()
    {
        float z = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        controller.Move(forward*z);
        controller.Move(right*x);
    }

    void Jump()
    {
        //Jumping
        if (!controller.isGrounded)
        {
            playerDir.y -= gravity * Time.deltaTime;
            controller.Move(playerDir);
        }

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            playerDir.y = jumpForce;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 5);
    }
}
