using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {
    public float walkSpeed = 0.1f;
    float horizontal, vertical;

    Rigidbody2D body;
    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        transform.Translate(horizontal * walkSpeed, vertical * walkSpeed, 0);
    }

    private void FixedUpdate()
    {
        
    }
}
