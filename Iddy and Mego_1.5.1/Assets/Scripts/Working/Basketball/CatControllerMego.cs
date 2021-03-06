using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class CatControllerMego : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    LevelManager gameLevelManager;

    //for momentum movement
    [SerializeField] float maxSpeed = 8f;
    [SerializeField] float accel = 20f;

    //jumping
    [SerializeField] float jumpSpeed = 14f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool grounded;

    private Vector2 movementInput = Vector2.zero;
    float horizontalValue;

    bool facingRight = true;

    private void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        horizontalValue = movementInput.x;
    }

    private void FixedUpdate()
    {
        IsGrounded();
        float dir = horizontalValue;
        Move(dir);

        //animator
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    public void OnMove(InputValue input) {
        Vector2 inputVec = input.Get<Vector2>();
        movementInput = new Vector2(inputVec.x, inputVec.y);
    }

    public void OnJump() {
        Jump();
    }

    //player position checks

    //checks for ground
    bool IsGrounded()
    {
        Vector2 positionLeft = new Vector2(transform.position.x - 0.52f, transform.position.y);
        Vector2 positionRight = new Vector2(transform.position.x + 0.52f, transform.position.y);
        Vector2 positionMid = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = Vector2.down;
        float distance = 0.76f;

        Debug.DrawRay(positionLeft, direction, Color.green);
        Debug.DrawRay(positionRight, direction, Color.green);
        Debug.DrawRay(positionMid, direction, Color.green);
        RaycastHit2D hitLeft = Physics2D.Raycast(positionLeft, direction, distance, groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(positionRight, direction, distance, groundLayer);
        RaycastHit2D hitMid = Physics2D.Raycast(positionMid, direction, distance, groundLayer);

        if (hitLeft.collider != null || hitRight.collider != null || hitMid.collider != null) {
            grounded = true;
            return true;
        }

        grounded = false;
        return false;
    }
    
    public void Jump()
    {
        if (IsGrounded()) {
            //rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            rb.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
        }
    }

    void Land()
    {

    }
    
    public void Move(float dir)
    {
        //current horizontal speed
        float xSpeed = rb.velocity.x * Time.fixedDeltaTime * 100;

        //turning
        Vector3 currentScale = transform.localScale;
        if (facingRight && dir < 0) {
            currentScale.x *= -1;
            facingRight = false;
        }
        else if (!facingRight && dir > 0) {
            currentScale.x = Math.Abs(currentScale.x);
            facingRight = true;
        }
        transform.localScale = currentScale;

        //animator
        if (dir != 0) {
            animator.SetBool("LRHeld", true);
        } else {
            animator.SetBool("LRHeld", false);
        }
        
        //movement with AddForce
        //if holding "right"
        if (dir > 0f) 
        {
            //if current speed is above max speed
            if (xSpeed > maxSpeed) {
                //set current speed to max speed
                rb.velocity = new Vector2(maxSpeed * dir, rb.velocity.y);
            }
            else {
                //accelerate player by accel constant
                rb.AddForce(new Vector2(accel * dir, 0), ForceMode2D.Force);
            }
        }
        //if holding "left"
        else if (dir < 0f) 
        {
            //if current speed is above max speed
            if (xSpeed < -maxSpeed) {
                //set current speed to max speed
                rb.velocity = new Vector2(maxSpeed * dir, rb.velocity.y);
            }
            else {
                //accelerate player by accel constant
                rb.AddForce(new Vector2(accel * dir, 0), ForceMode2D.Force);
            }
        }
        //if holding neither "left" nor "right"
        else if (dir == 0) {
            //if current velocity is greater than 0.01
            if (xSpeed > 0.5f) {
                //decelerate player by accel constant
                rb.AddForce(new Vector2(-(accel), 0));
            }
            //if current velocity is less than -0.01
            else if (xSpeed < -0.5f) {
                rb.AddForce(new Vector2(accel, 0));
            }
            else {
                //if the current velocity is between 0.01 and -0.01, then set the instant
                //velocity to 0
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        animator.SetFloat("xVelocity", Math.Abs(xSpeed));
    }
}