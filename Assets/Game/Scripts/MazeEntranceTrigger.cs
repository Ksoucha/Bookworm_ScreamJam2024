using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeEntranceTrigger : MonoBehaviour
{
    public Enemy_WormAI[] WormAIToStart;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (Enemy_WormAI worm in WormAIToStart)
            {
                worm.StartMoving();
            }
        }
    }
}
