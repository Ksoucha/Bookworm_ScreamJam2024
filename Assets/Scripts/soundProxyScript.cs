using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundProxyScript : MonoBehaviour
{
    public float jumpRad = 6f;
    public float sprintRad = 3f;
    public float walkRad = 1.5f;
    public float stillRad = 0f;
    [SerializeField] private SphereCollider detector;
    public FirstPersonController firstPersonController;
    float flagTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        detector = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (firstPersonController.isJumping)
        {
            flagTimer = 0f;
        }

         if(flagTimer>0 && flagTimer <1)
        detector.radius = jumpRad;
        else if (firstPersonController.isSprinting)
            detector.radius = sprintRad;
        else if (firstPersonController.move != Vector2.zero)
            detector.radius = walkRad;
        else
            detector.radius = stillRad;
        if (flagTimer < 1.1f) flagTimer += Time.deltaTime;
    }
    
}
