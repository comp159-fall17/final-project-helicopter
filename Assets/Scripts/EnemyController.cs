using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Shooter {
    protected float range = 10;

    protected override bool ShouldShoot {
        get {
            Vector3 target = GetTarget();
            float targetDistance = Vector3.Distance(target, transform.position);
            bool inRange = targetDistance < range;

            if (inRange) {
                transform.LookAt(target);
            }

            return inRange;
        }
    }

    protected override void Start() {
        base.Start();
    }

    protected override Vector3 GetTarget() {
        return GameObject.FindGameObjectWithTag("Player")
                         .transform.position;
    }
}
