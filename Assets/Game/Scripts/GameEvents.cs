using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public delegate void GameEvent();

    public static event GameEvent onPlayClicked;
    public static event GameEvent onExitClicked;
    public static event GameEvent onHomeClicked;

    public static void TriggerOnPlayClicked() => onPlayClicked?.Invoke();
    public static void TriggerOnExitClicked() => onExitClicked?.Invoke();
    public static void TriggerOnHomeClicked() => onHomeClicked?.Invoke();
}
