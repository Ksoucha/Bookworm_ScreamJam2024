using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookTag : MonoBehaviour, IInteractable
{
    public TextMeshProUGUI note;
    public GameObject mazeEntrance;
    private float timeToAppear = 1f;
    private float timeToDisappear;
    private bool seenNote = false;

    public void Interact()
    {
        note.enabled = true;
        timeToDisappear = Time.time + timeToAppear;
        seenNote = true;
    }

    void Update()
    {
        if (note.enabled && (Time.time >= timeToDisappear))
        {
            note.enabled = false;
        }

        if (seenNote)
        {
            Destroy(mazeEntrance);
        }
    }
}
