using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeExit : MonoBehaviour, IInteractable
{
    public GameObject mazeExitPoint;
    private bool haveBookTag;

    public void Interact()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (haveBookTag)
        {
            Destroy(mazeExitPoint);
        }
    }
}
