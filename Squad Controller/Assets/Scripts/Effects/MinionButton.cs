using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionButton : MonoBehaviour
{
    Condition condition;
    bool condition_state = false;
    Vector3 initial_position;
    float movement = 0;
    bool direction = false;
    CharacterController controller;
    bool past_direction = false;

    public float error_margin = 0.5f;
    public GameObject reactor;
/*    public float movement_magnitude;
    public Vector3 direction;*/
    public Vector3 final_position;
    public float speed = 1;

    private void Awake()
    {
        condition = GetComponent<Condition>();
        controller = reactor.GetComponent<CharacterController>();
        condition_state = condition.CheckCondition();
        initial_position = reactor.transform.position;
    }

    protected virtual void HandleMovement()
    {
        var goal = final_position;
        if(!direction)
        {
            goal = initial_position;
        }
        Vector3 new_direction = goal - reactor.transform.position;
        if (new_direction.magnitude <= error_margin)
        {
            movement = 0;
        }
        Vector3 input_direction = new Vector3(new_direction.x, 0.0f, new_direction.y);
        controller.Move(new_direction.normalized * speed * Time.deltaTime * movement);
    }
    private void FixedUpdate()
    {
        if(condition)
        {
            if(direction = condition.CheckCondition())
            {
                if(past_direction != direction)
                {
                    movement = 1;
                    past_direction = direction;
                }
            }
            else
            {
                if(past_direction != direction)
                {
                    movement = 1;
                    past_direction = direction;
                }
            }
        }

        HandleMovement();
    }

    public int GetRequirements()
    {
        return condition.GetRequirements();
    }

}
