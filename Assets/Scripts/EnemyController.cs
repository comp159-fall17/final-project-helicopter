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
                !Physics.Raycast(transform.position, TargetDirection,
                                 TargetDistance,
                                 ~((1 << LayerMask.NameToLayer("Player")) |
                                 (1<< LayerMask.NameToLayer("Shield"))));

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

    float EffectiveRange {
        get {
            float thisRange = Range;
            if (Mathf.Abs(RelativeTargetAngle) < VisionCone) {
                thisRange *= FacingDistanceScale;
            }
            return thisRange;
        }
    }

    bool WithinRange {
        get { return TargetDistance < EffectiveRange; }
    }

    void OnDrawGizmosSelected() {
        // basic range
        GizmoDraw.Circle(transform.position, range);

        GizmoDraw.Wedge(transform.position, range * FacingDistanceScale,
                        visionCone * 2, transform.forward, Color.magenta);

        // target system
        Debug.DrawRay(transform.position, TargetDirection); // all
        Debug.DrawRay(transform.position,
                      TargetDirection.normalized * EffectiveRange,
                      Color.red); // viewing portion

        // forward facing direction
        Debug.DrawRay(transform.position,
                      transform.forward * range * FacingDistanceScale,
                      Color.green);
    }

    public float hitPoints = 10; //Enemy HP

    protected override void Die() {
        GameManager.Instance.EnemyHasDied();
        Destroy(gameObject);
    }
}
