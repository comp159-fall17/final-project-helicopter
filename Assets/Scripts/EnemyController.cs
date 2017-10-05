using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Shooter {
    protected override bool StartShooting {
        get { return true; }
    }

    protected override bool StopShooting {
        get { return false; }
    }

    protected override void Start() {
        base.Start();
        StartCoroutine(ShootBullets());
    }

    protected override Vector3 GetTarget() {
        return GameObject.FindGameObjectWithTag("Player").transform.position;
    }
}
