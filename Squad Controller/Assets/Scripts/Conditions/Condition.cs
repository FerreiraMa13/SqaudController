using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition : MonoBehaviour
{
    public bool active = true;
    public bool state = false;

    public virtual bool CheckCondition()
    {
        return false;
    }
    public virtual int GetRequirements()
    {
        return 0;
    }
}
