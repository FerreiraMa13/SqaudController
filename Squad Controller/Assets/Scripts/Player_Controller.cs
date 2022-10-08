using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{

    CharacterController controller;
    Player_Controller_Actions controls;
    Camera main_camera;
    Vector3 coordenates = Vector3.zero;
    Vector2 movement_inputs = Vector2.zero;
    Vector2 zoom_inputs = Vector2.zero;

    public float speed = 1f;
    public float scroll_speed = 1f;
    public float max_zoom = 8f;
    public float min_zoom = 16f;

    public bool inverted = true;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new Player_Controller_Actions();
        controls.Player.Click.performed += ctx => GetClickCoordenates();
        controls.Player.Move.performed += ctx => movement_inputs = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => movement_inputs = Vector2.zero;
        controls.Player.Zoom.performed += ctx => zoom_inputs = ctx.ReadValue<Vector2>();
        controls.Player.Zoom.canceled += ctx => zoom_inputs = Vector2.zero;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }
    void GetClickCoordenates()
    {

    }
    void HandleMovement()
    {
        Vector3 input_direction = new Vector3(movement_inputs.x, 0.0f, movement_inputs.y);
        float zoom = Calc3rule(zoom_inputs.y)*scroll_speed;
        /*if(zoom < max_zoom)
        {
            zoom = max_zoom;
        }
        else if(zoom > min_zoom)
        {
            zoom = min_zoom;
        }*/
        if(inverted)
        {
            zoom = -zoom;
        }
        input_direction.y = zoom; 
        controller.Move(input_direction * speed * Time.deltaTime);
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

    /// <summary>
    /// a ----- A
    /// b ----- x
    /// </summary>
    private float Calc3rule(float a, float b, float A)
    {
        return (b * A) / a;
    }
    private float Calc3rule(float b)
    {
        return (b * 100) / 240;
    }
}
