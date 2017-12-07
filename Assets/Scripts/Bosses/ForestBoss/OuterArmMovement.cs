using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterArmMovement : MonoBehaviour {

    private int moveTime = 0;

    public int Direction = 1;
    public int setTime;
    public float Speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        move();
    }

    private void move()
    {
        moveTime++;
        if (moveTime > setTime)
        {
            Direction *= -1;
            moveTime = 0;
        }
        transform.Translate(0, 0, Speed * Direction);
    }
}