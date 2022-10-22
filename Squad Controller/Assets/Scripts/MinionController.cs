using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    public enum STATUS
    {
        IDLE = 0,
        MOVING = 1,
        TETHERED = 2,
        SOLO = 3
    }
    protected SquadBrain squad;
    private CharacterController controller;
    public float speed = 1f;
    public float error_margin = 0.5f;
    public STATUS current_state = STATUS.IDLE;

    protected Vector3 destination = Vector3.zero;

    public float turn_smooth_time = 0.1f;
    protected float turn_smooth_velocity;

    public Material online_color;
    public Material offline_color;
    protected MeshRenderer mesh_rend;
    protected bool online = false;

    protected void Awake()
    {
        controller = GetComponent<CharacterController>();
        mesh_rend = GetComponent<MeshRenderer>();
        squad = GameObject.FindGameObjectWithTag("Squad").GetComponent<SquadBrain>();
        AdditionalAwake();
    }
    public virtual  void OrderToMove(Vector3 new_destination)
    {
        if (current_state == STATUS.TETHERED)
        {
            squad.OrderToMove(new_destination);
        }
        else if (current_state != STATUS.SOLO)
        {
            current_state = STATUS.MOVING;
            destination = new Vector3(new_destination.x, transform.position.y, new_destination.z);
        }
    }
    public void FixedUpdate()
    {
        if (current_state == STATUS.MOVING || current_state == STATUS.SOLO)
        {
            current_state = HandleMovement();
        }
    }
    protected virtual STATUS HandleMovement()
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
        return current_state;
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
        ChangeMaterial(online_color);
    }
    public virtual bool goOffline()
    {
        if(current_state == STATUS.SOLO)
        {
            return false;
        }
        online = false;
        if(current_state == STATUS.TETHERED)
        {
            if (!squad.RemoveMinion(this))
            {
                return false;
            }
        }
        ChangeMaterial(offline_color);
        return true;
    }
    protected void ChangeMaterial (Material new_material)
    {
        mesh_rend.material = new_material;
        var meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (var mesh in meshes)
        {
            mesh.material = new_material;
        }
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Minion")
        {
            if(other.GetComponent<MinionController>() != null && other.GetComponent<MinionController>() != this)
            {
                squad.AddMinion(this);
            }
        }
    }
    protected virtual void AdditionalAwake()
    {

    }
    public virtual void OrderToSolo(Vector3 new_destination)
    {
        OrderToMove(new_destination);
    }
}
