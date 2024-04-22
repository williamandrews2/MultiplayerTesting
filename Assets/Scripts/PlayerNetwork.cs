using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    public float movementSpeed = 3;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
 
    void Update()
    {
        if(!IsOwner)
        {
            return;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        rb.velocity = new Vector3(horizontalInput * movementSpeed, rb.velocity.y, verticalInput * movementSpeed);

    }
}
