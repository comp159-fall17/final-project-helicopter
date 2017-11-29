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
        Vector3 randomDirection = Random.insideUnitSphere * distance + origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        return navHit.position;
    }

    NavMeshAgent Agent;

    protected override void Start() {
        Agent = GetComponent<NavMeshAgent>();
        Agent.speed = Speed;

        // default wandering
        StartCoroutine(Wander());
    }

    #pragma warning disable RECS0135 // Function does not reach its end or a 'return' statement by any of possible execution paths
    protected virtual IEnumerator Wander() {
#pragma warning restore RECS0135 // Function does not reach its end or a 'return' statement by any of possible execution paths
        while (true) {
            Agent.destination = RandomNavSphere(transform.position, 30, -1);

            yield return new WaitUntil(() => Agent.remainingDistance < 0.1f);
        }
    }
}
