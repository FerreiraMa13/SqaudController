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
    Vector2 mouse_pos = Vector2.zero;
    Vector2 clicked_pos = Vector2.zero;
    bool clicked = false;
    MinionController minion;

    public float speed = 1f;
    public float scroll_speed = 1f;
    public float max_zoom = 8f;
    public float min_zoom = 16f;

    public bool inverted = true;
    public bool manual_zoom = false;
    


    void Awake()
    {
        main_camera = GetComponentInChildren<Camera>();
        controller = GetComponent<CharacterController>();
        controls = new Player_Controller_Actions();
        controls.Player.Click.performed += ctx => GetClickCoordenates();
        controls.Player.MousePosition.performed += ctx => mouse_pos = ctx.ReadValue<Vector2>();
        controls.Player.Move.performed += ctx => movement_inputs = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => movement_inputs = Vector2.zero;
        if(!manual_zoom)
        {
            controls.Player.Zoom.performed += ctx => zoom_inputs = ctx.ReadValue<Vector2>();
            controls.Player.Zoom.canceled += ctx => zoom_inputs = Vector2.zero;
        }
        else
        {
            controls.Player.ManualZoomPlus.performed += ctx => zoom_inputs.x = 1f;
            controls.Player.ManualZoomPlus.canceled += ctx => zoom_inputs.x = 0f;
            controls.Player.ManualZoomMinus.performed += ctx => zoom_inputs.y = -1f;
            controls.Player.ManualZoomMinus.canceled += ctx => zoom_inputs.y = 0f; 
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }
    private void Update()
    {
        if(clicked)
        {
            HandleCommands();
        }
    }
    private void GetClickCoordenates()
    {
        if(!clicked)
        {
            RaycastHit hit;
            var ray = main_camera.ScreenPointToRay(mouse_pos);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Minion")
                {
                    if (minion == null)
                    {
                        minion = hit.transform.gameObject.GetComponent<MinionController>();
                        Debug.Log("Hit Minion");
                    }
                }
                else
                {
                    clicked = true;
                    clicked_pos = hit.point;
                    Debug.Log(main_camera.ScreenToWorldPoint(mouse_pos));
                }
            }
            
        }
    }

    private void HandleCommands()
    {
        Debug.Log(main_camera.ScreenToWorldPoint(clicked_pos));
        if(minion != null)
        {
            minion.OrderToMove(clicked_pos);
        }
        clicked = false;
    }
    private void HandleMovement()
    {
        Vector3 input_direction = new Vector3(movement_inputs.x, 0.0f, movement_inputs.y);
        float zoom = 0f;
        if(manual_zoom)
        {
            if(zoom_inputs.x > 0)
            {
                zoom = Calc3rule(zoom_inputs.x) * scroll_speed;
            }
            else
            {
                zoom = Calc3rule(zoom_inputs.y) * scroll_speed;
            }
        }
        else
        {
            zoom = Calc3rule(zoom_inputs.y) * scroll_speed;
        }
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
