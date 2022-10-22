using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SmartMinionController : MinionController
{
    public NavMeshAgent agent;
    protected override void AdditionalAwake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    public override void OrderToMove(Vector3 new_destination)
    {
        if (current_state == STATUS.TETHERED)
        {
            squad.OrderToMove(new_destination);
        }
        else
        {
            current_state = STATUS.MOVING;
            agent.SetDestination(new_destination);
        }
    }

    protected override STATUS HandleMovement()
    {
        float distance_remain = (destination - transform.position).magnitude;
        if (distance_remain <= error_margin)
        {
            return STATUS.IDLE;
        }
        return STATUS.MOVING;
    }
}
