using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Tackler : Wanderer {
    protected override bool ShouldShoot { get { return false; } }

    public float minimumDistanceToTarget = 5f;

#pragma warning disable RECS0135 // Function does not reach its end or a 'return' statement by any of possible execution paths
    protected override IEnumerator Wander() {
#pragma warning restore RECS0135 // Function does not reach its end or a 'return' statement by any of possible execution paths
        while (true) {
            yield return new WaitUntil(() => Agent.isActiveAndEnabled);

            // Reset speed just in case was hit, etc
            Body.velocity *= 0;

            float speed;

            if (hasBeenHit) {
                hasBeenHit = false;
                Agent.speed = 0;
                yield return new WaitUntil(() => TargetDistance > minimumDistanceToTarget);
            }

            if (base.ShouldShoot) {
                Agent.destination = Target;
                speed = runSpeed;
            } else {
                Agent.destination = SearchNextDestination();
                speed = walkSpeed;
            }

            Agent.speed = speed;
            yield return new WaitUntil(() => base.ShouldShoot || Agent.remainingDistance < 0.1f);
        }
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
