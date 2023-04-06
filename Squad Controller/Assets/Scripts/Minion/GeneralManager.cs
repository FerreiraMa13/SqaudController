using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralManager : MonoBehaviour
{
    public string minion_tag = "Minion";
    public string interactible_tag = "Interactible";
    private void Awake()
    {
        Globals.minion_tag = minion_tag;
        Globals.interactible_tag = interactible_tag;
    }
}
