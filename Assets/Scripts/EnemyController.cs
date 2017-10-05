using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Shooter {
    /// <summary>
    /// Range of action.
    /// </summary>
    protected float range = 10;

    /// <summary>
    /// Cone of vision (theta = visionCone * 2, centered on transform.forward).
    /// </summary>
    protected float visionCone = 45;

    /// <summary>
    /// How much farther the enemy can see if they're facing the target.
    /// </summary>
    protected float facingDistanceScale = 1.5f;

    protected override bool ShouldShoot {
        get {
            Vector3 target = GetTarget();
            Vector3 direction = target - transform.position;
            float targetDistance = direction.magnitude;

            float angle = Vector3.Angle(direction, transform.forward);

            if (Mathf.Abs(angle) < visionCone) {
                targetDistance /= facingDistanceScale;
            }

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
