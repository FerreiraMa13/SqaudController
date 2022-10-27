using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadBrain : MonoBehaviour
{
    protected List<MinionController> tethered_minions = new List<MinionController>();
    protected MinionController minion_brain;
    private CharacterController controller;
    public float speed = 1f;
    public float error_margin = 0.5f;
    public MinionController.STATUS current_state = MinionController.STATUS.IDLE;

   protected Vector3 destination = Vector3.zero;

    public float turn_smooth_time = 0.1f;
    private float turn_smooth_velocity;
    private Quaternion group_rotation;

    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        group_rotation = transform.rotation;
    }
    public virtual void OrderToMove(Vector3 new_destination)
    {
        if(tethered_minions.Count >0)
        {
                current_state = MinionController.STATUS.MOVING;
                destination = new Vector3(new_destination.x, transform.position.y, new_destination.z);
        }
    }
    public virtual bool AddMinion(MinionController new_minion)
    {
        if(tethered_minions.Contains(new_minion))
        {
            return false;
        }
        tethered_minions.Add(new_minion);
        if (tethered_minions.Count == 1)
        {
            Recenter();
        }
        new_minion.current_state = MinionController.STATUS.TETHERED;
        new_minion.transform.SetParent(transform);
        return true;
    }
    public bool RemoveMinion(MinionController new_minion)
    {
        if(current_state == MinionController.STATUS.IDLE)
        {
            if (!tethered_minions.Contains(new_minion))
            {
                return false;
            }
            new_minion.transform.parent = null;
            tethered_minions.Remove(new_minion);
            new_minion.current_state = MinionController.STATUS.IDLE;
            return true;
        }
        return false;
    }
    public void Recenter()
    {
        List<Transform> children = new List<Transform>();
        for(int i =0; i < transform.childCount; i++ )
        {
            children.Add(transform.GetChild(i));
        }
        transform.DetachChildren();
        if(tethered_minions.Count > 0)
        {
            Vector3 mid_point = tethered_minions[0].transform.position;
            mid_point.y = transform.position.y;
            Vector3 top_left = tethered_minions[0].transform.position;
            Vector3 bot_right = tethered_minions[0].transform.position;

            foreach(var minion in tethered_minions)
            {
                var minion_pos = minion.transform.position;
                if(minion_pos.x < top_left.x) { top_left.x = minion_pos.x; }
                if(minion_pos.z > top_left.z) { top_left.z = minion_pos.z; }
                if(minion_pos.x > bot_right.x) { bot_right.x = minion_pos.x; }
                if(minion_pos.z < bot_right.z) { bot_right.z = minion_pos.z; }
            }

            var distance_x = bot_right.x - top_left.x;
            var distance_z = top_left.z - bot_right.z;
            distance_x *= 0.5f;
            distance_z *= 0.5f;
            mid_point.x = top_left.x + distance_x;
            mid_point.z = bot_right.z + distance_z;
            transform.position = mid_point;

            foreach( var child in children)
            {
                child.SetParent(transform);
            }
        }
    }
    public void FixedUpdate()
    {
        if (current_state == MinionController.STATUS.MOVING)
        {
            current_state = HandleMovement();
        }
    }
    protected virtual MinionController.STATUS HandleMovement()
    {
        Vector3 new_direction = destination - transform.position;
        Vector3 input_direction = new Vector3(new_direction.x, 0.0f, new_direction.y);
        Vector3 rotate = RotateCalc(new_direction, destination.y);

        
        foreach (var minion in  tethered_minions)
        {
            minion.RotateCalc(new_direction, destination.y);
           /* minion.transform.rotation = Quaternion.Euler(rotate);*/
        }
        controller.Move(new_direction.normalized * speed * Time.deltaTime);

        float distance_remain = (destination - transform.position).magnitude;
        if (distance_remain <= error_margin)
        {
            return MinionController.STATUS.IDLE;
        }
        return MinionController.STATUS.MOVING;
    }
    private Vector3 RotateCalc(Vector3 input_direction, float anchor_rotation)
    {
        input_direction.Normalize();
        float rotateAngle = Mathf.Atan2(input_direction.x, input_direction.z) * Mathf.Rad2Deg + anchor_rotation;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotateAngle, ref turn_smooth_velocity, turn_smooth_time);
        group_rotation = Quaternion.Euler(0.0f, smoothAngle, 0.0f);
        return new Vector3(0.0f, smoothAngle, 0.0f);
    }
}
