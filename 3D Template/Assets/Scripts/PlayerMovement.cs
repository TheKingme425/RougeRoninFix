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
    public float Slide_cooldown = 0f;
    public float WallRunningSpeed;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool IsSlideing = false;
    public bool canslide = true;
    private bool canMove = true;
    private bool crouched = false;
    public bool WallRunning;
    public bool IsWallRunning;

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

        if (WallRunning)
        {
            moveDirection.y = WallRunningSpeed;
        }

        if (IsSlideing == false && crouched == false && characterController.isGrounded)
        {
            AdjustCollider(defaultHeight);
            characterController.center = new Vector3(0, 0, 0);
            walkSpeed = 6f;
            runSpeed = 12f;
            if (Slide_cooldown == 0)
            {
                canslide = true;
            }
            
        }
        if (Input.GetKey(KeyCode.C) && !(Input.GetKey(KeyCode.LeftShift)) && canMove && characterController.isGrounded)
        {
            AdjustCollider(crouchHeight);
            characterController.center = new Vector3(0, 0.2f, 0);
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
            crouched = true;

        }
        if (Slide_cooldown > 0)
        {
            Slide_cooldown -= Time.deltaTime;
            if(Slide_cooldown < 1)
            {
                Slide_cooldown = 0;
                canslide = true;
            }
        }
        if (slide_Time > 0)
        {
            AdjustCollider(crouchHeight);
            slide_Time -= Time.deltaTime;
            if (!(Input.GetKey(KeyCode.C)))
            {
               
                if (slide_Time <= 1.5f)
                {
                    Slide_cooldown = 3f;
                }
                slide_Time = 0;

            }
        }
        else if (!(Input.GetKey(KeyCode.C)))
        {
            IsSlideing = false;
        }

        if (IsSlideing)
        {
            AdjustCollider(crouchHeight);
            moveDirection.x *= Mathf.Lerp(1.25f, 0, (2 - slide_Time) / 2);
            moveDirection.z *= Mathf.Lerp(1.25f, 0, (2 - slide_Time) / 2);
        }
        if (!(Input.GetKey(KeyCode.LeftShift)))
        {
            if (Slide_cooldown == 0)
            {
                canslide = true;
            }
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
            characterController.center = new Vector3(0, 0.2f, 0);
            Debug.Log("Is slideing");
            IsSlideing = true;
            canslide = false;
            slide_Time = 2f;
        }
    }

    private void AdjustCollider(float newHeight)
    {
        Debug.Log(newHeight);
        characterController.height = newHeight;
        
    }
}