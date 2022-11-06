using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_Identifier : MonoBehaviour
{
    private Camera cam;
    private string identifier;
    private Player_Controller player;
    private Character_Controller character;
    private UI_Caster last_caster;
    private bool online = true;

    private void Start()
    {
        if (GetComponent<Player_Controller>())
        {
            identifier = "Player";
            player = GetComponent<Player_Controller>();
            cam = player.main_camera;
        }
        else if (GetComponent<Character_Controller>())
        {
            identifier = "Character";
            character = GetComponent<Character_Controller>();
            cam = character.char_camera;
        }
    }
    private void Update()
    {
        UpdateCamera();
        online = cam.isActiveAndEnabled;
        if(online)
        {
            Raycast();
        }
        else
        {
            if(last_caster)
            {
                last_caster.Deactivate();
                last_caster = null;
            }
        }
    }
    private bool UpdateCamera()
    {
        Camera check_camera;
        if(identifier == "Player")
        {
            check_camera = player.main_camera;
        }
        else if(identifier == "Character")
        {
            check_camera = character.char_camera;
        }
        else
        {
            return false;
        }

        if(cam != check_camera)
        {
            cam = check_camera;
            return true;
        }

        return false;
    }
    private void Raycast()
    {
        Vector3 mouse_pos = Mouse.current.position.ReadValue();
        RaycastHit hit;
        var camera_center = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, cam.nearClipPlane));
        Ray ray = cam.ScreenPointToRay(camera_center);
        if (Physics.Raycast(ray, out hit) && hit.collider)
        {
            if (last_caster)
            {
                last_caster.Deactivate();
                last_caster = null;
            }
            var caster = hit.transform.gameObject.GetComponent<UI_Caster>();
            if (caster && caster.valid)
            {
                caster.Activate();
                last_caster = caster;
            }
        }
    }
}
