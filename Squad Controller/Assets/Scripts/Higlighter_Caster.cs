using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Higlighter_Caster : MonoBehaviour
{
    public string internal_string;
    public GameObject hit_object;
    public RaycastHit hit;
    public List<string> recognisable_tags;
    [System.NonSerialized] public string seleceted_object;

    private Player_Controller player;
    private Camera cam;

    private void Start()
    {
        if(player == null)
        {
            player = gameObject.GetComponent<Player_Controller>();
        }

        cam = player.main_camera;
    }

    private void Update()
    {
        UpdateCamera();
        Vector3 mouse_pos = Mouse.current.position.ReadValue();
        /*mouse_pos.z = cam.nearClipPlane;
        Vector3 world_pos = cam.ScreenToWorldPoint(mouse_pos);*/

        Ray ray = cam.ScreenPointToRay(mouse_pos);
        if (Physics.Raycast(ray, out hit) && hit.collider)
        {
            if(recognisable_tags.Contains(hit.transform.tag))
            {
                hit_object = hit.transform.gameObject;
                seleceted_object = hit_object.name;
                internal_string = seleceted_object;
            }
        }
    }
    private bool UpdateCamera()
    {
        if(cam != player.main_camera)
        {
            cam = player.main_camera;
            return true;
        }
        return false;
    }
}
