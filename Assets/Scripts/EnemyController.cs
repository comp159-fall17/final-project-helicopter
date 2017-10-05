using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Shooter {
    protected float range = 10;

    Vector3 target;

    protected override bool ShouldShoot {
        get {
            return Vector3.Distance(GetTarget(), transform.position) < range;
        }
    }

    protected override void Start() {
        base.Start();
    }

    protected override Vector3 GetTarget() {
        return GameObject.FindGameObjectWithTag("Player")
                         .transform.position; ;
    }
}
