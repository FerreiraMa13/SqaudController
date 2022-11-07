using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressConsole : MonoBehaviour
{
    private Condition condition;
    private bool condition_state = false;
    public int part_id = 0;

    public ConsoleP2C console;

    private void Awake()
    {
        condition = GetComponent<Condition>();
        condition_state = condition.CheckCondition();
    }
    private void Update()
    {
        if (condition)
        {
            if (condition.CheckCondition() && !condition_state)
            {
                condition_state = true;
                console.Progress(part_id);
                this.enabled = false;
            }
        }
    }
    public int GetRequirements()
    {
        return condition.GetRequirements();
    }
}
