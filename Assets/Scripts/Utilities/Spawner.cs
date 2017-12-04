using System.Linq;
using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public static Spawner Instance;

    public float minSpawnDistance;

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
    /// Generates valid position.
    /// </summary>
    /// <returns>The position.</returns>
    /// <param name="overlaps">Overlap validation function.</param>
    Vector3 GeneratePosition(System.Func<Vector3, Collider[]> overlaps) {
        Vector3 candidate;

        Vector3 playerPostion = GameObject.FindGameObjectWithTag("Player")
                                          .transform.position;

        do {
            candidate = new Vector3(Random.Range(-38.0f, 38.0f), 0.5f,
                                    Random.Range(-38.0f, 38.0f));
            // if far from player and nothing collides with it
        } while (Vector3.Distance(candidate, playerPostion) < minSpawnDistance
                 || overlaps(candidate).Any(i => i.gameObject.name != "Ground"));

        return candidate;
    }

    GameObject Spawn(GameObject[] array, string arrayName, int type,
               System.Func<Vector3, Collider[]> overlaps, Transform inputPos = null) {
        if (array.Length == 0) {
            Debug.LogWarning("There are no " + arrayName + " prefabs listed.");
            return null;
        }

        if (type >= array.Length) {
            Debug.LogError(arrayName + " type is not defined.");
            return null;
        }

        GameObject item = array[type];

        Vector3 pos;
        if (inputPos == null) {
            pos = GeneratePosition(overlaps);
        } else {
            pos = new Vector3(inputPos.position.x, 0.5f, inputPos.position.z);
        }

        return Instantiate(item, pos, item.transform.rotation) as GameObject;
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

        Destroy(Spawn(Pickups, "Pickups", type, overlaps, inputPos), pickupDestroyTime);
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
    public void SpawnEnemy(int type) {
        System.Func<Vector3, Collider[]> overlaps = delegate (Vector3 position) {
            return Physics.OverlapBox(position, new Vector3(.75f, 0f, .75f));
        };

        Spawn(Enemies, "Enemies", type, overlaps);
    }

    /// <summary>
    /// Spawns a random enemy.
    /// </summary>
    public void SpawnEnemy() {
        SpawnEnemy(Random.Range(0, Enemies.Length));
    }
}
