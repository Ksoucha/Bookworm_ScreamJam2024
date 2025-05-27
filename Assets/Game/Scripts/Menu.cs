using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public void OnPlayClicked()
    {
        GameEvents.TriggerOnPlayClicked();
    }

    public void OnExitClicked()
    {
        GameEvents.TriggerOnExitClicked();
    }
}
