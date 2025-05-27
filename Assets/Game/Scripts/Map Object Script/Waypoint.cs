using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // Make it invisible on start
    void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }
}
