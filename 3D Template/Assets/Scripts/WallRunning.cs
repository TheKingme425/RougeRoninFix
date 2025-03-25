using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float maxwallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]

    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftwallhit;
    private RaycastHit rightwallhit;
    private bool wallLeft;
    private bool wallRight;

    [Header("References")]
    public Transform orientation;
    private PlayerMovement pm;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if(pm.WallRunning)
            wallRunningMovement();
        
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightwallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftwallhit, wallCheckDistance, whatIsWall);

    }
    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }
    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {
           if(!pm.WallRunning)
           {
                StartWallRun();
           }
        }
        else
        {
            if (pm.WallRunning)
            {
                StopwallRun();
            }
        }
    }

    private void StartWallRun()
    {
        pm.WallRunning = true;
        pm.IsWallRunning = true;
    }
    private void wallRunningMovement()
    {
        Vector3 wallNormal = wallRight ? rightwallhit.normal : leftwallhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
    }
    private void StopwallRun()
    {
        
        pm.WallRunning = false;
        pm.IsWallRunning = false;
    }
}
