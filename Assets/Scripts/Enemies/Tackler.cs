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
                Agent.speed = runSpeed * 0;
            } else {
                Agent.destination = SearchNextDestination();
                Agent.speed = walkSpeed * 0;
            }

            yield return new WaitUntil(() => Agent.isActiveAndEnabled && ShouldContinue);
        }
    }

    protected override void Update() {
        base.Update();

        if (base.ShouldShoot) {
            PlayClip("run");
        } else {
            PlayClip("walk");
        }
    }

    float lastPlayedTime;
    string lastPlayedClip = "";
    void PlayClip(string clipName) {
        if (Time.time - lastPlayedTime > 0.5f || lastPlayedClip != clipName) {
            print("played " + clipName);
            lastPlayedTime = Time.time;
            Anim.Play(clipName);
            lastPlayedClip = clipName;
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

    protected override void Die() {
        StartCoroutine(DeathProcess());
    }

    IEnumerator DeathProcess() {
        Body.velocity *= 0;
        Agent.speed = 0;
        yield return new WaitWhile(() => attacking);

        StopCoroutine(WanderCoroutine);

        Anim.Play("dead");

        yield return new WaitForSeconds(2);

        GameManager.Instance.EnemyHasDied(transform);
        GameManager.Instance.PlayDeathSound();
        Destroy(gameObject);
    }
}
