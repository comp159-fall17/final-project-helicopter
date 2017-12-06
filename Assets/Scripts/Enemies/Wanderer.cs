using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Wanderer : EnemyController {
    /// <summary>
    /// Finds next random spot to go to.
    /// 
    /// Adapted from https://forum.unity.com/threads/solved-random-wander-ai-using-navmesh.327950/
    /// </summary>
    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask) {
        for (int i = 0; i < 30; i++) {
            Vector3 randomDirection = Random.insideUnitSphere * distance + origin;
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask)) {
                return navHit.position;
            }
        }

        return origin;
    }

    public float wanderSearchRadius = 5f;

    protected NavMeshAgent Agent;

    protected override void Start() {
        Agent = GetComponent<NavMeshAgent>();
        Agent.speed = Speed;

        // default wandering
        StartCoroutine(Wander());
    }

#pragma warning disable RECS0135 // Function does not reach its end or a 'return' statement by any of possible execution paths
    protected virtual IEnumerator Wander() {
#pragma warning restore RECS0135 // Function does not reach its end or a 'return' statement by any of possible execution paths
        yield return new WaitUntil(() => Agent.isActiveAndEnabled);

        while (true) {
            // Reset speed just in case was hit, etc
            Body.velocity *= 0;

            Agent.destination = SearchNextDestination();

            yield return new WaitUntil(() => Agent.isActiveAndEnabled && ShouldContinue);
        }
    }

    protected virtual bool ShouldContinue {
        get { return Agent.remainingDistance < 0.1f; }
    }

    protected Vector3 SearchNextDestination() {
        return RandomNavSphere(transform.position, wanderSearchRadius, -1);
    }
}
