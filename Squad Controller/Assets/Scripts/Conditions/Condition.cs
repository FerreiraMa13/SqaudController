using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition : MonoBehaviour
{
    public bool active = true;

    public virtual bool CheckCondition()
    {
        return false;
    }

}
