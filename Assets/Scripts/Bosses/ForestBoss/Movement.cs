using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    private int moveTime = 0;
    private int bossDirection = 1;

    public int setTime;
    public float bossSpeed;

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
            bossDirection *= -1;
            moveTime = 0;
        }
        transform.Translate(bossSpeed * bossDirection, 0, 0);
    }
}
