using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Controller : MonoBehaviour
{

    private Player_Controller player;
    private Player_Controller_Actions controls;
    private CharacterController controller;
    private Vector2 movement_inputs = Vector2.zero;

    public Camera char_camera;
    public float move_speed = 1.0f;

    public float deadzone = 0.1f;
    public float turn_smooth_time = 0.1f;
    private float turn_smooth_velocity;

    public int number_jumps = 1;
    [System.NonSerialized]
    public int jump_attempts = 0;
    
    public float jump_force = 10.0f;
    private float jump_velocity = 0.0f;

    public float gravity = 9.8f;
    public float additional_decay = 0.0f;
    public float decay_multiplier = 0.2f;

    public bool jumping = false;
    public bool landing = false;
    private bool falling = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Controller>();
        char_camera = GetComponentInChildren<Camera>();
        SetUpControls();
    }
    private void FixedUpdate()
    {
        HandleJump();
        HandleMovement();
    }
    public void Interact()
    {

    }
    private void HandleMovement()
    {
        Vector3 input_direction = new(movement_inputs.x, 0.0f, movement_inputs.y);
        Vector3 rotate = RotateCalc(input_direction, char_camera.transform.eulerAngles.y);
        Vector3 movement = XZMoveCalc(rotate, input_direction);

        movement.y = jump_velocity;

        controller.Move(move_speed * Time.deltaTime * movement);
    }
    private void HandleJump()
    {
        /*if (controller.isGrounded)
        {
            if (jumping && jump_velocity <= -0.5f)
            {
                jumping = false;
                additional_decay = 0.0f;
                jump_attempts = 0;
            }
            else if (falling)
            {
                falling = false;
                additional_decay = 0.0f;
            }
            jump_velocity = -gravity * Time.deltaTime;
        }
        else
        {
            if (!falling && !jumping)
            {
                falling = true;
            }
            jump_velocity -= (gravity * Time.deltaTime) + additional_decay;
            additional_decay += (0.2f * move_speed * Time.deltaTime);
        }*/
        if(controller.isGrounded)
        {
            if(jumping && additional_decay >= decay_multiplier)
            {
                Debug.Log("Landed");
                jumping = false;
                additional_decay = 0.0f;
                jump_attempts = 0;
            }
        }
        if(jumping)
        {
            jump_velocity -= (gravity * Time.deltaTime) + additional_decay;
            additional_decay += (Time.deltaTime * move_speed * decay_multiplier);
        }

    }
    private void Jump()
    {
        Debug.Log("Jump");
        if (jump_attempts < number_jumps && !landing)
        {
            jumping = true;
            jump_velocity = jump_force;
            additional_decay = 0.0f;
            jump_attempts += 1;
        }
    }
    private Vector3 RotateCalc(Vector3 input_direction, float anchor_rotation)
    {
        input_direction.Normalize();
        float rotateAngle = Mathf.Atan2(input_direction.x, input_direction.z) * Mathf.Rad2Deg + anchor_rotation;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotateAngle, ref turn_smooth_velocity, turn_smooth_time);
        transform.rotation = Quaternion.Euler(0.0f, smoothAngle, 0.0f);

        return new Vector3(0.0f, rotateAngle, 0.0f);
    }
    private Vector3 XZMoveCalc(Vector3 direction, Vector3 input)
    {
        Vector3 forward = Quaternion.Euler(direction).normalized * Vector3.forward;
        Vector3 movement = forward * move_speed;
        if (!Compare2Deadzone(movement_inputs.x) && !Compare2Deadzone(movement_inputs.y))
        {
            movement = Vector3.zero;
        }
        return movement;
    }
    private bool Compare2Deadzone(float value)
    {
        if (value < deadzone)
        {
            if (value > -deadzone)
            {
                return false;
            }
        }
        return true;
    }
    private void SetUpControls()
    {
        controls = new Player_Controller_Actions();
        controls.Character.Move.performed += ctx => movement_inputs = ctx.ReadValue<Vector2>();
        controls.Character.Move.canceled += ctx => movement_inputs = Vector2.zero;
        controls.Character.Click.performed += ctx => Interact();
        controls.Character.Interact.performed += ctx => Interact();
        controls.Character.Jump.performed += ctx => Jump();
    }
    private void OnEnable()
    {
        Activate();
    }
    public void Activate()
    {
        char_camera.enabled = true;
        EnableInput();
    }
    public void EnableInput()
    {
        controls.Character.Enable();
    }
    private void OnDisable()
    {
        Disable();
    }
    public void Disable()
    {
        char_camera.enabled = false;
        DisableInput();
    }
    public void DisableInput()
    {
        controls.Character.Disable();
    }
    
}
