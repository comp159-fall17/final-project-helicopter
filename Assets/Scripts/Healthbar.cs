using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {

    public float hitPoints = 100;
    public Image healthBar;

    private float health;

    // Use this for initialization
    void Start () {
        health = hitPoints;
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    void OnTriggerEnter(Collider other) {
        

        if (other.CompareTag("Bullet"))
        {
            Debug.Log(hitPoints);
            Debug.Log(health);
            hitPoints -= 1;
            healthBar.fillAmount = health / hitPoints;
            if (hitPoints == 0)
                Destroy(this.gameObject);
        }
    }
}
