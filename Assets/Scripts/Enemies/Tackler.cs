using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Tackler : Wanderer {
    protected override float Speed { get { return base.ShouldShoot ? runSpeed : walkSpeed; } }

    protected override bool ShouldShoot { get { return false; } }

    public float minimumDistanceToTarget = 5f;

#pragma warning disable RECS0135 // Function does not reach its end or a 'return' statement by any of possible execution paths
    protected override IEnumerator Wander() {
#pragma warning restore RECS0135 // Function does not reach its end or a 'return' statement by any of possible execution paths
        yield return new WaitUntil(() => Agent.isActiveAndEnabled);

        while (true) {
            // Reset speed just in case was hit, etc
            Body.velocity *= 0;

            if (hasBeenHit) {
                hasBeenHit = false;
                Agent.speed = 0;
                yield return new WaitUntil(() => TargetDistance > minimumDistanceToTarget);
            }

            Agent.destination = base.ShouldShoot ? Target : SearchNextDestination();
            Agent.speed = Speed;

            yield return new WaitUntil(() => Agent.isActiveAndEnabled && ShouldContinue);
        }
    }

    protected override bool ShouldContinue {
        get { return base.ShouldContinue || base.ShouldShoot; }
    }

    bool hasBeenHit;
    protected virtual void OnCollisionEnter(Collision collision) {
        GameObject player = GameManager.Instance.Player;

        if (collision.gameObject.Equals(player)) {
            player.GetComponent<Shooter>().Hit(10);
            hasBeenHit = true;
        }
    }
}
