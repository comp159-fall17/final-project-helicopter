using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossKilledSnow : EnemyRoom {

    public GameObject UnlockKey;

    // Use this for initialization
    protected override void Start () {
        base.Start();
        UnlockKey.SetActive(false);
    }

    public override void Spawn() {
        base.Spawn();
        StartCoroutine(WaitToDestroy());
    }
	
	// Update is called once per frame
    IEnumerator WaitToDestroy () {
        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

        UnlockKey.SetActive(true);
        GameManager.Instance.PlayDeathSound();
        Destroy(GameObject.FindGameObjectWithTag("Boss"));
    }
}
