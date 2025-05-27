using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

public class Enemy_WormAI : MonoBehaviour
{
    [SerializeField]
    private GameObject[] waypoints;

    [SerializeField]
    private bool repeatPathAfterFinalPoint = false;

    [SerializeField]
    private float distanceBeforeSwitchTarget = 1.0f;

    [SerializeField]
    private string[] itemTagToAvoidDestroying;

    public float speed = 2.0f;

    public float rotationSpeed = 1.0f;

    public GameObject exitItem;

    private UnityEngine.Vector3 positionToMoveTo;

    private int waypointIndex = 0;

    [SerializeField]
    float encirclingRadius = 5f;

    [SerializeField]
    float encirclingSpeed = 1f;
    
    private bool startCircularMovement = false;

    private bool startMoving = false;

    void Start()
    {
        if (waypoints.Length > 0)
        {
            positionToMoveTo = waypoints[waypointIndex].transform.position;
        }
        else if (exitItem)
        {
            // an area around exit item
            positionToMoveTo = exitItem.transform.position;
            startCircularMovement = true;
        }
    }

    public void StartMoving()
    {
        startMoving = true;
    }

    void Update()
    {
        if (startMoving)
        {
            // do a check whether we are near the next position 
            if (UnityEngine.Vector3.Distance(this.transform.position, positionToMoveTo) < distanceBeforeSwitchTarget)
            {
                SwitchTarget();
            }

            if (startCircularMovement)
            {
                CircularMovement();
            }
            else
            {
                this.transform.position = UnityEngine.Vector3.MoveTowards(this.transform.position, positionToMoveTo, speed * Time.deltaTime);
                // this.transform.LookAt(positionToMoveTo);
                UnityEngine.Vector3 targetDirection = positionToMoveTo - this.transform.position;
                UnityEngine.Vector3 newDirection = UnityEngine.Vector3.RotateTowards(this.transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
                this.transform.rotation = UnityEngine.Quaternion.LookRotation(newDirection);
            }

        }
    }

    private void SwitchTarget()
    {
        bool waypointChanged = false;
        if (waypoints.Length > 0)
        {
            waypointIndex++;
            if (waypointIndex < waypoints.Length)
            {
                waypointChanged = true;
            }
            else if (repeatPathAfterFinalPoint)
            {
                waypointIndex = 0;
                waypointChanged = true;
            }
        }

        if (waypointChanged)
        {
            positionToMoveTo = waypoints[waypointIndex].transform.position;
        }
        else if (exitItem)
        {
            // an area around exit item
            positionToMoveTo = exitItem.transform.position;
            startCircularMovement = true;
        }
    }

    public void OnCollisionEnterChild(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        Debug.Log("BRUH");

        if (collision.gameObject.CompareTag("Player"))
        {
            //trigger lose condition
            return;
        }

        foreach (string tagToAvoid in itemTagToAvoidDestroying)
        {
            //Do nothing if it is quest item
            return;
        }

        //We can trigger destruction sequence
        Destroy(collision.gameObject);
    }

    public void OnTriggerEnterChild(Collider other)
    {
        Debug.Log(other.tag);

        if (other.CompareTag("Player"))
        {
            //trigger lose condition
            return;
        }

        foreach (string tagToAvoid in itemTagToAvoidDestroying)
        {
            //Do nothing if it is quest item/item that we don't want to destroy
            if (other.CompareTag(tagToAvoid))
            {
                return;
            }
        }

        //We can trigger destruction sequence
        Destroy(other.gameObject);
    }

    private void CircularMovement()
    {
        float distance = UnityEngine.Vector3.Distance(positionToMoveTo, this.transform.position);
        if (distance > encirclingRadius)
        {
            UnityEngine.Vector3 newPosition = (distance- (encirclingSpeed * Time.deltaTime)) * UnityEngine.Vector3.Normalize(this.transform.position - positionToMoveTo) + positionToMoveTo;
        }

        transform.RotateAround(positionToMoveTo, UnityEngine.Vector3.up, rotationSpeed*2 * Time.deltaTime);
    }
}
