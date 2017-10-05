using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Shooter {
    float range = 10;
    float visionCone = 45;
    float facingDistanceScale = 1.5f;

    GameObject rangeMarker;

    /// <summary>
    /// Range of action.
    /// </summary>
    public float Range { get { return range; } }

    /// <summary>
    /// Cone of vision (theta = visionCone * 2, centered on transform.forward).
    /// </summary>
    public float VisionCone { get { return visionCone; } }

    /// <summary>
    /// How much farther the enemy can see if they're facing the target.
    /// </summary>
    public float FacingDistanceScale { get { return facingDistanceScale; } }

    protected override bool ShouldShoot {
        get {
            bool visible = WithinRange &&              // Not "Player" layer (8)
                !Physics.Raycast(TargetRay, EffectiveRange, ~(1 << 8));

            if (visible) {
                transform.LookAt(TargetDirection);
            }

            return visible;
        }
    }

    protected override Vector3 Target {
        get {
            return GameObject.FindGameObjectWithTag("Player")
                             .transform.position;
        }
    }

    Vector3 TargetDirection {
        get { return Target - transform.position; }
    }

    Ray TargetRay {
        get { return new Ray(transform.position, TargetDirection); }
    }

    float EffectiveRange {
        get {
            float thisRange = Range;
            if (Mathf.Abs(TargetAngle) < VisionCone) {
                thisRange *= FacingDistanceScale;
            }
            return thisRange;
        }
    }

    float TargetAngle {
        get { return Vector3.Angle(TargetDirection, transform.forward); }
    }

    bool WithinRange {
        get { return TargetDirection.magnitude < EffectiveRange; }
    }

    void OnDrawGizmos() {
        GizmoDraw.Circle(transform.position, range);
        GizmoDraw.Ray(TargetRay, EffectiveRange);
    }
}
