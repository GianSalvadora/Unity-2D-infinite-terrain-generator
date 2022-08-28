using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerControls : MonoBehaviour
{


    new Rigidbody2D rigidbody2D;
    public float movementSpeed = 5;
    Vector2 moveInput = new Vector2(0, 0);

    Transform cameraTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        rigidbody2D = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        cameraTransform.position = new Vector3(transform.position.x, transform.position.y, cameraTransform.position.z);
    }

   void FixedUpdate()
   {
    rigidbody2D.velocity = moveInput.normalized * movementSpeed;
   }
}
