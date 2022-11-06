using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Caster : MonoBehaviour
{
    [System.NonSerialized] public bool active;
    public bool valid;
    public List<TextMeshProUGUI> activated_texts;
    private UI_Manager ui_manager;

    private void Awake()
    {
        ui_manager = GameObject.FindGameObjectWithTag("UI_Manager").GetComponent<UI_Manager>();
    }
    public void Activate()
    {
        foreach (var text in activated_texts)
        {
            ui_manager.ShowNotification(text);
        }
    }
    public void Deactivate()
    {
        foreach (var text in activated_texts)
        {
            ui_manager.HideNotification(text);
        }
    }
}
