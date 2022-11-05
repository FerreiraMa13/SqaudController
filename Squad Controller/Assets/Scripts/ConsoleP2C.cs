using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleP2C : MonoBehaviour
{
    public string char_tag = "Character";
    public Player_Controller player_claw;
    public Character_Controller character;

    private void Awake()
    {
        character = GameObject.FindGameObjectWithTag(char_tag).GetComponent<Character_Controller>();
    }
    private void OnValidate()
    {
        if (player_claw != null)
        {
            player_claw.console_p2c = this;
        }
    }
    public void Character_interact()
    {
        player_claw.Activate();
        character.Disable();
    }
    public void Claw_Interact()
    {
        character.Activate();
        player_claw.Disable();
    }
}
