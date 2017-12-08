using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossKilledSnow : MonoBehaviour {

    public GameObject UnlockKey;

    // Use this for initialization
    void Start () {
        UnlockKey.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if(GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            UnlockKey.SetActive(true);
            GameManager.Instance.PlayDeathSound();
            Destroy(GameObject.FindGameObjectWithTag("Boss"));
        }
    }
}
