using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SmartMinionController : MinionController
{
    public Material solo_color;
    public NavMeshAgent agent;
    public Player_Controller player;
    protected override void AdditionalAwake()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Controller>();
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
            destination = new_destination;
        }
    }
    protected override STATUS HandleMovement()
    {
        float distance_remain = (destination - transform.position).magnitude;
        if (distance_remain <= error_margin)
        {
            Debug.Log(current_state);
            if(current_state == STATUS.SOLO)
            {
                goOffline();
            }
            return STATUS.IDLE;
        }
        return current_state;
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Minion")
        {
            if (other.GetComponent<MinionController>() != null && other.GetComponent<MinionController>() != this)
            {
                if(current_state != STATUS.SOLO && other.GetComponent<MinionController>().current_state != STATUS.SOLO)
                {
                    squad.AddMinion(this);
                    agent.enabled = false;
                }
            }
        }
    }
    public override bool goOffline()
    {
        online = false;
        if (current_state == STATUS.TETHERED)
        {
            if (!squad.RemoveMinion(this))
            {
                return false;
            }
        }
        ChangeMaterial(offline_color);
        agent.enabled = false;
        agent.enabled = true;
        return true;
    }
    public override bool goSolo()
    {
        if (current_state == STATUS.TETHERED)
        {
            if (!squad.RemoveMinion(this))
            {
                return false;
            }
            current_state = STATUS.IDLE;
            if (!agent.isActiveAndEnabled)
            {
                agent.enabled = true;
            }
        }
        ChangeMaterial(solo_color);
        return true;
    }
    public override void OrderToSolo(Vector3 new_destination)
    {
        if (current_state == STATUS.TETHERED)
        {
            squad.OrderToSolo(new_destination);
        }
        else if (current_state != STATUS.SOLO)
        {
            current_state = STATUS.SOLO;
            agent.SetDestination(new_destination);
            destination = new (new_destination.x, transform.position.y, new_destination.z);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 6 /*Tiles*/|| collision.gameObject.tag == "Minion")
        {

        }
        else
        {
            goOffline();
            agent.isStopped = true;
            player.RemoveMinion(this);
        }
    }
}
