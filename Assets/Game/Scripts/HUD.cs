using UnityEngine;

public class HUD : MonoBehaviour
{
    public void OnHomeClicked()
    {
        GameEvents.TriggerOnHomeClicked();
    }
}
