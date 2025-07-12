using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCam : MonoBehaviour
{
    public float moveSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // moves the object twards x+ indefently 
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
    }
}
