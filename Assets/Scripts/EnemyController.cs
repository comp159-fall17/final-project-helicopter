using UnityEngine;

/// <summary>
/// Enemy controller for enemy that targets player.
/// </summary>
public class EnemyController : Shooter {
    float range = 10;
    float visionCone = 45;
    float facingDistanceScale = 1.5f;

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
            bool visible = WithinRange &&
                !Physics.Raycast(TargetRay, TargetDistance,
                                 ~(1 << LayerMask.NameToLayer("Player")));

            if (visible) {
                transform.LookAt(Target);
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

    float TargetDistance {
        get { return Vector3.Distance(transform.position, Target); }
    }

    bool WithinRange {
        get { return TargetDirection.magnitude < EffectiveRange; }
    }

    void OnDrawGizmosSelected() {
        // basic range
        GizmoDraw.Circle(transform.position, range);

        GizmoDraw.Wedge(transform.position, range * FacingDistanceScale,
                        visionCone * 2, transform.forward, Color.magenta);

        // target system
        GizmoDraw.Ray(TargetRay, TargetDistance); // all
        GizmoDraw.Ray(TargetRay, EffectiveRange, Color.red); // viewing portion

        // forward facing direction
        GizmoDraw.Ray(new Ray(transform.position, transform.forward),
                      range * FacingDistanceScale, Color.green);
    }
}
