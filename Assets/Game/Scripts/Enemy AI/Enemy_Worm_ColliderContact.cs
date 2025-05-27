using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Since the parent can't direct access to child collission, gotta do this
public class Enemy_Worm_ColliderContact : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Enemy_WormAI mainAIScript = this.transform.parent.GetComponentInParent<Enemy_WormAI>();
        mainAIScript.OnCollisionEnterChild(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy_WormAI mainAIScript = this.transform.parent.GetComponentInParent<Enemy_WormAI>();
        mainAIScript.OnTriggerEnterChild(other);
    }
}
