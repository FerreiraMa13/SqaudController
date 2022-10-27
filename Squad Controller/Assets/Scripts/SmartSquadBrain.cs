using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SmartSquadBrain : SquadBrain
{
    protected SmartMinionController smart_minion_brain;
    public NavMeshAgent agent;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }
    public override bool AddMinion(MinionController new_minion)
    {
        if (tethered_minions.Contains(new_minion))
        {
            return false;
        }
        tethered_minions.Add(new_minion);
        if (tethered_minions.Count >= 1)
        {
            Recenter();
        }
        new_minion.current_state = MinionController.STATUS.TETHERED;
        new_minion.transform.SetParent(transform);
        return true;
    }
    public override void OrderToMove(Vector3 new_destination)
    {
        if (tethered_minions.Count > 0)
        {
            current_state = MinionController.STATUS.MOVING;
            destination = new Vector3(new_destination.x, transform.position.y, new_destination.z);
            agent.SetDestination(destination);
            Debug.Log("Destin");
        }
    }

    protected override MinionController.STATUS HandleMovement()
    {
        Vector3 new_direction = destination - transform.position;
        Vector3 new_forward = transform.position + transform.forward;
        Vector3 input_direction = new Vector3(new_direction.x, 0.0f, new_direction.y);

       foreach (var minion in tethered_minions)
        {
            minion.transform.localRotation = Quaternion.identity;
            /*minion.RotateCalc(new_forward, destination.y);*/
        }

        float distance_remain = (destination - transform.position).magnitude;
        if (distance_remain <= error_margin)
        {
            return MinionController.STATUS.IDLE;
        }
        return MinionController.STATUS.MOVING;
    }
    public void OrderToSolo(Vector3 new_destination)
    {
        Debug.Log(tethered_minions.Count);
        if (tethered_minions.Count > 0)
        {
            if(destination != new_destination)
            {
                destination = new Vector3(new_destination.x, transform.position.y, new_destination.z);
                var minion = tethered_minions[0];
                if(minion.goSolo())
                {
                    minion.OrderToSolo(destination);
                }
            }
        }
    }
}
