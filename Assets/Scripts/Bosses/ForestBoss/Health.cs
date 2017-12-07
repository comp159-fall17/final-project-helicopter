using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public float health;
    private bool notDead = true;
    private float damage;

	// Use this for initialization
	void Start () {
        damage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControls>().bulletDamage;
	}
	
	// Update is called once per frame
	void Update () {
		if(health <= 0 && notDead)
        {
            Debug.Log("ok");
            notDead = false;
            die();
        }
	}

    private void die()
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "PlayerBullet")
        {
            health -= damage;
        }
        if(other.gameObject.tag == "Player")
        {
            //Damage player
        }
    }
}
