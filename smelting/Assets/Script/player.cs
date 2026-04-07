using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    Rigidbody player1;
    // Start is called before the first frame update
    void Start()
    {
        player1 = GetComponent<Rigidbody>();
        player1.freezeRotation = true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
