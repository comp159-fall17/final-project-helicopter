using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour {
    public static Spawner Instance;

    public float minSpawnDistance;
    public float maxSpawnDistance;
    public float[] spawnProbabilities;

    // Pickups
    public GameObject[] Pickups;

    public float pickupDestroyTime = 5.0f;

    // Enemies
    public GameObject[] Enemies;

    void Start() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Generates valid position using plain radius.
    /// </summary>
    /// <returns>The position.</returns>
    /// <param name="center">Center to generate from.</param>
    /// <param name="radius">Maximum distance from center.</param>
    /// <param name="overlaps">Overlap validation function.</param>
    Vector3 GeneratePosition(Vector3 center, float radius,
                             System.Func<Vector3, Collider[]> overlaps) {
        Vector3 candidate;

        bool valid;
        do {
            Vector3 offset = Random.insideUnitCircle * radius;
            offset.y = 2f;
            candidate = offset + center;

            //NavMeshHit hit;
            //NavMesh.SamplePosition(candidate, out hit, maxSpawnDistance, -1);
            //valid = hit.position == candidate;

            // if far from player and nothing collides with it, and on ground.
            Collider[] overlapHits = overlaps(candidate);

            valid = Vector3.Distance(candidate, center) > minSpawnDistance;
            valid &= !overlapHits.Any(i => i.gameObject.tag != "Floor");
        } while (!valid);

        print(center);
        return candidate;
    }

    GameObject Spawn(GameObject[] array, string arrayName, int type, Vector3 center,
                     System.Func<Vector3, Collider[]> overlaps) {
        return Spawn(array, arrayName, type,
                     GeneratePosition(center, maxSpawnDistance, overlaps));
    }

    GameObject Spawn(GameObject[] array, string arrayName, int type, Vector3 inputPos) {
        if (array.Length == 0) {
            Debug.LogWarning("There are no " + arrayName + " prefabs listed.");
            return null;
        }

        if (type >= array.Length) {
            Debug.LogError(arrayName + " type is not defined.");
            return null;
        }

        GameObject item = array[type];

        return Instantiate(item, inputPos, item.transform.rotation) as GameObject;
    }

    /// <summary>
    /// Spawns a pickup based on type.
    /// </summary>
    /// <param name="type">Index location of pickup in list.</param>
    public void SpawnPickup(int type, Transform inputPos = null) {
        System.Func<Vector3, Collider[]> overlaps = delegate (Vector3 position) {
            float prefabRadius = Pickups[0].GetComponent<SphereCollider>().radius;
            return Physics.OverlapSphere(position, prefabRadius);
        };

        if (inputPos != null) {
            Destroy(Spawn(Pickups, "Pickups", type, inputPos.position), pickupDestroyTime);
        } else {
            Destroy(Spawn(Pickups, "Pickups", type, inputPos.position, overlaps), pickupDestroyTime);
        }

    }

    /// <summary>
    /// Spawns a random pickup.
    /// </summary>
    public void SpawnPickup() {
        SpawnPickup(Random.Range(0, Pickups.Length));
    }

    /// <summary>
    /// Spawns a random pickup
    /// </summary>
    /// <param name="location">Transform (location) to spawn the pickup at.</param>
    public void SpawnPickupAtLocation(Transform location) {
        int pickupType = Random.Range(0, Pickups.Length);
        float val = Random.value + GameManager.Instance.playerLuck;

        if (val <= spawnProbabilities[pickupType]) {
            SpawnPickup(pickupType, location);
        }
    }

    /// <summary>
    /// Spawns a enemy based on type.
    /// </summary>
    /// <param name="type">Index location of enemy in list.</param>
    public void SpawnEnemy(int type, Vector3 center) {
        System.Func<Vector3, Collider[]> overlaps = delegate (Vector3 position) {
            return Physics.OverlapBox(position, new Vector3(.75f, 0f, .75f));
        };

        Spawn(Enemies, "Enemies", type, center, overlaps);
    }

    /// <summary>
    /// Spawns a random enemy.
    /// </summary>
    public void SpawnEnemy(Vector3 center) {
        SpawnEnemy(Random.Range(0, Enemies.Length), center);
    }
}
