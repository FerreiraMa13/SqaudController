using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{

    private CharacterController controller;
    private Player_Controller_Actions controls;
    private SquadBrain squad;
    private Camera main_camera;
    private readonly List<MinionController> controlled_minions = new();
    private readonly List<Camera> cameras = new();

    private Vector2 movement_inputs = Vector2.zero;
    private Vector2 zoom_inputs = Vector2.zero;
    private int camera_id = 0;
    
    public float speed = 1f;
    public float scroll_speed = 1f;
    public float max_zoom = 8f;
    public float min_zoom = 16f;
    public bool inverted = true;
    public bool manual_zoom = false;

    void Awake()
    { 
        foreach(var cam in GetComponentsInChildren<Camera>())
        {
            cameras.Add(cam);
            cam.gameObject.SetActive(false);
        }
        main_camera = cameras[0];
        main_camera.gameObject.SetActive(true);
        controller = GetComponent<CharacterController>();
        squad = GameObject.FindGameObjectWithTag("Squad").GetComponent<SquadBrain>();
        SetUpControls();
    }
    private void FixedUpdate()
    {
        HandleMovement();
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
            if (hit.transform.tag == "Minion")
            {
                var minion = hit.transform.gameObject.GetComponent<MinionController>();
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
            else
            {
                HandleCommands(hit.point);
            }
            
        }
    }
    private void HandleCommands(Vector3 position)
    {
        Debug.Log(position);
        /*squad.OrderToMove(position);*/
        if(controlled_minions.Count != 0)
        {
            foreach(var minion in controlled_minions)
            {
                minion.OrderToMove(position);
            }
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
}
