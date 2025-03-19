using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 3f;
    public float crouchHeight = 1.5f;
    public float crouchSpeed = 3f;
    public float slide_Time = 0f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool IsSlideing = false;
    public bool canslide = true;
    private bool canMove = true;
    private bool crouched = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        Debug.Log(characterController.height);
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded && canslide == true)
        {

            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (characterController.isGrounded)
        {
            moveDirection.y = Input.GetButton("Jump") ? jumpPower : -0.1f;
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (IsSlideing == false && crouched == false && characterController.isGrounded)
        {
            AdjustCollider(defaultHeight);
            walkSpeed = 6f;
            runSpeed = 12f;
            canslide = true;
        }
        else if (Input.GetKey(KeyCode.C) && !(Input.GetKey(KeyCode.LeftShift)) && canMove && characterController.isGrounded)
        {
            AdjustCollider(crouchHeight);
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
            crouched = true;
            Debug.Log("Is Crouching");

        }
        if (slide_Time > 0)
        {
            slide_Time -= Time.deltaTime;
            if (!(Input.GetKey(KeyCode.C)))
            {
                slide_Time = 0;
            }
        }
        else if (!(Input.GetKey(KeyCode.C)))
        {
            IsSlideing = false;
        }

        if (IsSlideing)
        {
            moveDirection *= Mathf.Lerp(1.25f, 0, (2 - slide_Time) / 2);
        }
        if (!(Input.GetKey(KeyCode.LeftShift)))
        {
            canslide = true;
            IsSlideing = false;
        }

        if (!(Input.GetKey(KeyCode.C)))
        {
            crouched = false;
        }

       


        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.C) && canMove && canslide == true && characterController.isGrounded && !crouched == true)
        {
            AdjustCollider(crouchHeight);
            Debug.Log("Is slideing");
            IsSlideing = true;
            canslide = false;

            slide_Time = 2f;
        }
    }

    private void AdjustCollider(float newHeight)
    {
        characterController.height = newHeight;
        characterController.center = new Vector3(0, newHeight / 12, 0);
    }
}