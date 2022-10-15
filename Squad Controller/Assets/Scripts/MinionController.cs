using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    public enum STATUS
    {
        IDLE = 0,
        MOVING = 1,
        TETHERED = 2
    }
    private SquadBrain squad;
    private CharacterController controller;
    public float speed = 1f;
    public float error_margin = 0.5f;
    public STATUS current_state = STATUS.IDLE;

    private Vector3 destination = Vector3.zero;

    public float turn_smooth_time = 0.1f;
    private float turn_smooth_velocity;

    public Material online_color;
    public Material offline_color;
    private MeshRenderer mesh_rend;
    private bool online = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        mesh_rend = GetComponent<MeshRenderer>();
        squad = GameObject.FindGameObjectWithTag("Squad").GetComponent<SquadBrain>();
    }
    public void OrderToMove(Vector3 new_destination)
    {
        if (current_state == STATUS.IDLE)
        {
            current_state = STATUS.MOVING;
            destination = new Vector3(new_destination.x, transform.position.y, new_destination.z);
        }
        else if(current_state == STATUS.TETHERED)
        {
            squad.OrderToMove(new_destination);
        }
    }
    public void FixedUpdate()
    {
        if (current_state == STATUS.MOVING)
        {
            current_state = HandleMovement();
        }
    }
    private STATUS HandleMovement()
    {

        Vector3 new_direction = destination - transform.position;
        Vector3 input_direction = new Vector3(new_direction.x, 0.0f, new_direction.y);
        Vector3 rotate = RotateCalc(new_direction, destination.y);
        controller.Move(new_direction.normalized * speed * Time.deltaTime);

        float distance_remain = (destination - transform.position).magnitude;
        if (distance_remain <= error_margin)
        {
            return STATUS.IDLE;
        }
        return STATUS.MOVING;
    }
    public Vector3 RotateCalc(Vector3 input_direction, float anchor_rotation)
    {
        input_direction.Normalize();
        float rotateAngle = Mathf.Atan2(input_direction.x, input_direction.z) * Mathf.Rad2Deg + anchor_rotation;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotateAngle, ref turn_smooth_velocity, turn_smooth_time);
        transform.rotation = Quaternion.Euler(0.0f, smoothAngle, 0.0f);
        return new Vector3(0.0f, rotateAngle, 0.0f);
    }
    public void goOnline()
    {
        online = true;
        mesh_rend.material = online_color;
        var meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (var mesh in meshes )
        {
            mesh.material = online_color;
        }
    }
    public void goOffline()
    {
        online = false;
        if(current_state == STATUS.TETHERED)
        {
            squad.RemoveMinion(this);
        }
        mesh_rend.material = offline_color;
        var meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (var mesh in meshes)
        {
            mesh.material = offline_color;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Minion")
        {
            if(other.GetComponent<MinionController>() != null && other.GetComponent<MinionController>() != this)
            {
                Debug.Log("flag");
                squad.AddMinion(this);
            }
        }
    }
}
