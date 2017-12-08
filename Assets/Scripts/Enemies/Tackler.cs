using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Tackler : Wanderer {
    protected override bool ShouldShoot { get { return false; } }

    public AudioClip tackleSound;
    public float minimumDistanceToTarget = 5f;

    bool attacking;
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
                Body.velocity *= 0;

                Shooter player = GameManager.Instance.Player.GetComponent<Shooter>();

                // snare
                attacking = true;
                Anim.Play("attack01");
                player.Body.isKinematic = true;
                yield return new WaitForSeconds(1f);

                // hurt
                GameManager.Instance.Player.GetComponent<Shooter>().Hit(10);
                GameManager.Instance.playerSound.PlayOneShot(tackleSound, 1);
                player.Body.isKinematic = false; // free
                attacking = false;
                yield return new WaitForSeconds(1f);
            }

            if (base.ShouldShoot) {
                Agent.destination = Target;
                Agent.speed = runSpeed;
            } else {
                Agent.destination = SearchNextDestination();
                Agent.speed = walkSpeed;
            }

            yield return new WaitUntil(() => Agent.isActiveAndEnabled && ShouldContinue);
        }
    }

    protected override bool ShouldContinue {
        get { return base.ShouldContinue || base.ShouldShoot; }
    }

    bool hasBeenHit;
    protected virtual void OnCollisionEnter(Collision collision) {
        GameObject player = GameManager.Instance.Player;

        if (collision.gameObject.Equals(player) && !attacking) {
            hasBeenHit = true;
        }
    }

    bool startedDeath;
    protected override void Die() {
        if (!startedDeath) {
            StartCoroutine(DeathProcess());
            startedDeath = true;
        }
    }

    IEnumerator DeathProcess() {
        Body.velocity *= 0;
        Agent.speed = 0;

        GameManager.Instance.Player.GetComponent<Rigidbody>().isKinematic &= !attacking;

        StopCoroutine(WanderCoroutine);

        Anim.Play("dead");

        yield return new WaitForSeconds(2);

        GameManager.Instance.EnemyHasDied(transform);
        GameManager.Instance.PlayDeathSound();
        Destroy(gameObject);
    }
}
