using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public string notifications_parent = "Notifications";
    public List<TextMeshProUGUI> notifications = new();

    private void OnValidate()
    {
        notifications.Clear();
        for(int i  = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if(child.name == notifications_parent)
            {
                foreach (var text in transform.GetChild(i).GetComponentsInChildren<TextMeshProUGUI>())
                {
                    if(!CheckTextList(text, notifications))
                    {
                        notifications.Add(text);
                    }
                }
            }
        }
    }
    private void Awake()
    {
        foreach(var text in notifications)
        {
            text.enabled = false;
        }
    }
    public bool ShowNotification(TextMeshProUGUI notification)
    {
        var valid = CheckTextList(notification, notifications);
        if (valid)
        {

            notification.enabled = true;
        }
        return valid;
    }
    public bool HideNotification(TextMeshProUGUI notification)
    {
        var valid = CheckTextList(notification, notifications);
        if (valid)
        {

            notification.enabled = false;
        }
        return valid;
    }
    private bool CheckTextList(TextMeshProUGUI element, List<TextMeshProUGUI> list)
    {
        if(list.Contains(element))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
