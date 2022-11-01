using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion_Proximity : Condition
{
    public int required_minions = 1;
    public bool debug = false;
    [System.NonSerialized] public List<SmartMinionController> current_minions = new();

    public override bool CheckCondition()
    {
        return required_minions == current_minions.Count;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == Globals.minion_tag)
        {
            
            SmartMinionController minion;
            if (minion = other.transform.gameObject.GetComponent<SmartMinionController>())
            {
                current_minions.Add(minion);
                Debug.Log("Entered");
                debug = CheckCondition();
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == Globals.minion_tag)
        {
            SmartMinionController minion;
            if (minion = other.transform.gameObject.GetComponent<SmartMinionController>())
            {
                if(current_minions.Contains(minion))
                {
                    current_minions.Remove(minion);
                    Debug.Log("Left");
                    debug = CheckCondition();
                }
            }
        }
    }

    public override int GetRequirements()
    {
        return required_minions;
    }
}
