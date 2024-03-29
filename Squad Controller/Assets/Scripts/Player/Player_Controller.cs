using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    public enum ORDERS
    {
        NONE = -1,
        MOVE = 1,
        SOLO = 2, 
        OBJECTIVE = 3,
    }

    public CharacterController controller;
    private Player_Controller_Actions controls;
    private SquadBrain squad;
    [System.NonSerialized] public Camera main_camera;
    private readonly List<SmartMinionController> controlled_minions = new();
    private readonly List<Camera> cameras = new();
    private GameObject objective;
    public ConsoleP2C console_p2c;

    private Vector2 movement_inputs = Vector2.zero;
    private Vector2 zoom_inputs = Vector2.zero;
    private int camera_id = 0;

    private Vector3 anchor;
    /*private bool rotate_anchor = false;*/
    
    public float speed = 1f;
    public float scroll_speed = 1f;
    public float max_zoom = 8f;
    public float min_zoom = 16f;
    public bool inverted = true;
    public bool manual_zoom = false;
    public bool extra_utility = false;

    public float turn_smooth_time = 0.1f;
    private float turn_smooth_velocity;
    public float deadzone = 0.1F;

    void Awake()
    { 
        foreach(var cam in GetComponentsInChildren<Camera>())
        {
            cameras.Add(cam);
            cam.gameObject.SetActive(false);
            if(cam.transform.tag == "MainCamera")
            {
                main_camera = cam;
            }
        }
        /*main_camera.gameObject.SetActive(true);*/
        controller = GetComponent<CharacterController>();
        squad = GameObject.FindGameObjectWithTag("Squad").GetComponent<SquadBrain>();

        SetUpControls();
        Disable();
    }
    private void FixedUpdate()
    {
        HandleMovement();
        /*FreeHandleMovement();*/
    }
    private void Update()
    {
    }
    private void GetClickCoordenates(InputAction.CallbackContext ctx)
    {
        RaycastHit hit;
        Ray ray = main_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if(Physics.Raycast(ray, out hit) && hit.collider)
        {
            /*Debug.Log(hit.transform.tag);*/
            if (hit.transform.tag == "Minion")
            {
                var minion = hit.transform.gameObject.GetComponent<SmartMinionController>();
                if (!controlled_minions.Contains(minion))
                {
                    /*squad.AddMinion(minion);*/
                    controlled_minions.Add(minion);
                    minion.goOnline();
                }
                else
                {
                    /*squad.RemoveMinion(minion);*/
                    if( minion.goOffline() )
                    {
                        controlled_minions.Remove(minion);
                    }
                }
            }
            else if (hit.transform.tag == "Interactible")
            {
                objective = hit.transform.gameObject;
                HandleCommands(hit.point, ORDERS.OBJECTIVE);
            }
            else
            {
                if(extra_utility)
                {
                    HandleCommands(hit.point, ORDERS.SOLO);
                }
                else
                {
                    HandleCommands(hit.point, ORDERS.MOVE);
                }
            }
            
        }
    }
    private void HandleCommands(Vector3 position, ORDERS order)
    {
        switch (order)
        {
            case ORDERS.MOVE:
                if (controlled_minions.Count != 0)
                {
                    foreach (var minion in controlled_minions)
                    {
                        minion.OrderToMove(position);
                    }
                }
                break;
            case ORDERS.SOLO:
                if (controlled_minions.Count != 0)
                {
                    if(controlled_minions[0].goSolo())
                    {
                        controlled_minions[0].OrderToSolo(position);
                        controlled_minions.RemoveAt(0);
                    }
                }
                break;
            case ORDERS.OBJECTIVE:
                    if (controlled_minions.Count != 0)
                    {
                        if (controlled_minions[0].goSolo())
                        {
                            controlled_minions[0].OrderToSolo(position);
                            controlled_minions.RemoveAt(0);
                        }
                    }
                break;
            default:
                break;
        }
        
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
        /*controls.Player.Enable();*/
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
    public bool isInList(MinionController search_target)
    {
        foreach(MinionController current in controlled_minions)
        {
            if(current == search_target)
            {
                return true;
            }
        }
        return false;
    }
    public void SwapCameras()
    {
        int max_id = cameras.Count;
        camera_id++;
        Debug.Log(camera_id);
        Debug.Log(cameras.Count);
        if(camera_id >= max_id)
        {
            camera_id = 0;
        }
        main_camera.gameObject.SetActive(false);
        main_camera = cameras[camera_id];
        main_camera.gameObject.SetActive(true);
    }
    private void SetUpControls()
    {
        controls = new Player_Controller_Actions();
        controls.Player.Click.performed += ctx => GetClickCoordenates(ctx);
        controls.Player.Move.performed += ctx => movement_inputs = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => movement_inputs = Vector2.zero;
        controls.Player.CycleCamera.performed += ctx => SwapCameras();
        controls.Player.ExtraUtility.performed += ctx => extra_utility = true;
        controls.Player.ExtraUtility.canceled += ctx => extra_utility = false;
        controls.Player.CameraUtility.performed += ctx => MouseAnchor();
        if (!manual_zoom)
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
    public void RemoveMinion(SmartMinionController minion)
    {
        if(controlled_minions.Contains(minion))
        {
            controlled_minions.Remove(minion);
        }
    }
    private void FreeHandleMovement()
    {

        Vector3 input_direction = new Vector3(movement_inputs.x, 0.0f, movement_inputs.y);
        Vector3 rotate = RotateCalc(input_direction, main_camera.transform.eulerAngles.y);
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
        Vector3 movement = forward * speed;

        if ((!Compare2Deadzone(movement_inputs.x) && !Compare2Deadzone(movement_inputs.y)))
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
    private void MouseAnchor()
    {
        /*rotate_anchor = true;
        anchor = Mouse.current.position.ReadValue();*/
    }
    public void Activate()
    {
        main_camera.gameObject.SetActive(true);
        controls.Player.Enable();
    }
    public void Disable()
    {
        main_camera.gameObject.SetActive(false);
        controls.Player.Disable();
    }
}
