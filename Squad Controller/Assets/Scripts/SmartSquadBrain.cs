using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SmartSquadBrain : SquadBrain
{
    protected new List<MinionController> tethered_minions = new();
    protected new SmartMinionController minion_brain;
    public NavMeshAgent agent;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }
    public override void OrderToMove(Vector3 new_destination)
    {
        if (tethered_minions.Count > 0)
        {
            current_state = MinionController.STATUS.MOVING;
            destination = new Vector3(new_destination.x, transform.position.y, new_destination.z);
            agent.SetDestination(destination);
        }
    }

    protected override MinionController.STATUS HandleMovement()
    {
        Vector3 new_direction = destination - transform.position;
        Vector3 input_direction = new Vector3(new_direction.x, 0.0f, new_direction.y);

        foreach (var minion in tethered_minions)
        {
            minion.RotateCalc(new_direction, destination.y);
        }

        float distance_remain = (destination - transform.position).magnitude;
        if (distance_remain <= error_margin)
        {
            return MinionController.STATUS.IDLE;
        }
        return MinionController.STATUS.MOVING;
    }
}
