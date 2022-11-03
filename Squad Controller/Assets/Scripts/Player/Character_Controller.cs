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

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Controller>();
        SetUpControls();
    }
    private void FixedUpdate()
    {
        HandleMovement();
    }
    public void Interact()
    {

    }
    private void HandleMovement()
    {
        Vector3 input_direction = new Vector3(movement_inputs.x, 0.0f, movement_inputs.y);
        Vector3 rotate = RotateCalc(input_direction, char_camera.transform.eulerAngles.y);
        Vector3 movement = XZMoveCalc(rotate, input_direction);
        controller.Move(movement * Time.deltaTime);
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
        controls.Player.Move.performed += ctx => movement_inputs = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => movement_inputs = Vector2.zero;
        controls.Player.Click.performed += ctx => Interact();
    }
    private void OnEnable()
    {
        controls.Player.Enable();
    }
    public void EnableInput()
    {
        controls.Player.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
    }
    public void DisableInput()
    {
        controls.Player.Disable();
    }
    public void Activate()
    {
        char_camera.enabled = true;
        controls.Player.Enable();
    }
    public void Disable()
    {
        char_camera.enabled = false;
        controls.Player.Disable();

    }
}
