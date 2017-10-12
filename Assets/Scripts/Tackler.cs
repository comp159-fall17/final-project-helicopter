using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tackler : EnemyController {
    protected override void Update() {
        base.Update();

        Follow();
    }

    bool ShouldAdvance {
        get { return TargetDistance > Range * 0.7; }
    }

    void Follow() {
        if (!WallInWay) {
            transform.LookAt(Target);
            if (ShouldAdvance) {
                Body.velocity = transform.forward * 30;
            } else {
                Body.velocity *= 0.95f;
            }
        }
    }
}
