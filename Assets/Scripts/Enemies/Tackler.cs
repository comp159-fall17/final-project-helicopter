using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Tackler : Wanderer {
    protected override bool ShouldShoot { get { return false; } }

    public float minimumDistanceToTarget = 5f;

    NavMeshAgent Agent;

    protected override void Update() {
        base.Update();

        UpdateSpeed();
    }

#pragma warning disable RECS0135 // Function does not reach its end or a 'return' statement by any of possible execution paths
    protected override IEnumerator Wander() {
#pragma warning restore RECS0135 // Function does not reach its end or a 'return' statement by any of possible execution paths
        while (true) {
            if (hasBeenHit) {
                hasBeenHit = false;
                Agent.isStopped = true;
                yield return new WaitUntil(() => TargetDistance > minimumDistanceToTarget);
                Agent.isStopped = false;
            } else if (base.ShouldShoot) {
                Agent.destination = Target;
            } else {
                Agent.destination = RandomNavSphere(transform.position, 60, -1);
            }

            yield return new WaitUntil(() => base.ShouldShoot || Agent.remainingDistance < 0.1f);
        }
    }

    void UpdateSpeed() {
        Agent.speed = base.ShouldShoot ? runSpeed : walkSpeed;
    }

    bool hasBeenHit;
    void OnCollisionEnter(Collision collision) {
        GameObject player = GameManager.Instance.Player;

        if (collision.gameObject.Equals(player)) {
            player.GetComponent<Shooter>().Hit(10);
            hasBeenHit = true;
        }
    }
}
