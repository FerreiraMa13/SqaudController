using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    public enum STATUS
    {
        IDLE = 0,
        MOVING = 1,
    }
    private CharacterController controller;
    public float speed = 1f;
    public float error_margin = 1.5f;
    public STATUS current_state = STATUS.IDLE;

    private Vector3 destination = Vector3.zero;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    public void OrderToMove(Vector3 new_destination)
    {
        if(current_state == STATUS.IDLE)
        {
            current_state = STATUS.MOVING;
            destination = new Vector3(new_destination.x, transform.position.y, new_destination.z);
        }
    }

    public void FixedUpdate()
    {
        if(current_state == STATUS.MOVING)
        {
            current_state = HandleMovement();
        }
    }
    private STATUS HandleMovement()
    {
        Vector3 new_direction = destination - transform.position;
        controller.Move(new_direction.normalized * speed * Time.deltaTime);

        float distance_remain = (destination - transform.position).magnitude;
        if(distance_remain <= error_margin)
        {
            return STATUS.IDLE;
        }
        return STATUS.MOVING;
    }
}
