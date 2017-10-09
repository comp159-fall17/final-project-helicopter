using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {

    public float hitPoints = 100;
    public Image healthBar;

    private float fillAmount;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        handleBar();
    }

    void OnTriggerEnter(Collider other) {
        

        if (other.CompareTag("Bullet"))
        {
            Debug.Log(hitPoints);
           
            hitPoints -= 1;
           
            if (hitPoints == 0)
                Destroy(this.gameObject);
        }
    }

    private void handleBar()
    {
        healthBar.fillAmount = Map(hitPoints, 0, 100, 0, 1);
    }

    private float Map(float value,float inMin,float inMax,float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
