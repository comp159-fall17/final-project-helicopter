using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour {

    public int moveDistance;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(this.transform.rotation.y == 0)
            {
                other.transform.Translate(0, 0, moveDistance);
            }
            else if (this.transform.rotation.y == 1)
            {
                other.transform.Translate(0, 0, -moveDistance);
            }
            else if (this.transform.rotation.y > 0)
            {
                other.transform.Translate(moveDistance, 0, 0);
            }
            else if (this.transform.rotation.y < 0)
            {
                other.transform.Translate(-moveDistance, 0, 0);
            }
        }
    }
}
