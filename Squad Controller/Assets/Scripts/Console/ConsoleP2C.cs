using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleP2C : MonoBehaviour
{
    /*public string char_tag = "Character";*/
    public Player_Controller player_claw;
    public Character_Controller character;
    public Material completed_material;
    private bool online = true;
    public MeshRenderer mesh_rend;
    public List<GameObject> spawnpoints = new();
    public int current_index = 0;

    private void Awake()
    {
       /* character = GameObject.FindGameObjectWithTag(char_tag).GetComponent<Character_Controller>();*/
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
        if(online)
        {
            player_claw.transform.position = spawnpoints[current_index].transform.position;
            player_claw.Activate();
            character.Disable();
        }
    }
    public void Claw_Interact()
    {
        character.Activate();
        player_claw.Disable();
    }
    public void GoOffline()
    {
        online = false;
        ChangeMaterial(completed_material);
        GetComponent<UI_Caster>().valid = false;
    }
    protected void ChangeMaterial(Material new_material)
    {
        mesh_rend.material = new_material;
    }
    public void Progress(int id)
    {
        if(spawnpoints.Count > 0 && id == current_index)
        {
            current_index++;
            if (current_index >= spawnpoints.Count)
            {
                Claw_Interact();
                GoOffline();
            }
            else
            {
                RepositionPlayer(current_index);
            }
        }
    }
    public void RepositionPlayer(int index)
    {
        player_claw.Disable();
        player_claw.controller.enabled = false;
        player_claw.transform.position = spawnpoints[index].transform.position;
        player_claw.controller.enabled = true;
        player_claw.Activate();
    }
}
