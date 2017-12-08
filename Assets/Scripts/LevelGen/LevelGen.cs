using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-170)]
public class LevelGen : MonoBehaviour {
    public static LevelGen Instance;

    public static Vector3 ReallyFarAway { get { return new Vector3(500, 0, 500); } }

    public int roomsPerFloor;
    [Space(10)]
    [Header("Room rates")]
    public float overallRoomRate;
    public float chestRate;
    public float shopRate;
    [Space(10)]
    public Vector3 centralNode;
    public float roomSize;
    [Space(10)]
    public GameObject finalBoss;
    public Biome[] biomes;

    public GameObject Door { get { return CurrentBiome.Door; } }
    public int CurrentFloor { get; private set; }

    public GameObject shopKeep;

    private int roomType = 0;

    private List<GameObject> spawnedRooms = new List<GameObject>();
    private Vector3[] nodes;

    private Biome CurrentBiome { get { return biomes[roomType]; } }

    void Start() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        nodes = new Vector3[5];
        CurrentFloor = 0;

        NewFloor();
    }

    public void ReloadFloor(bool won) {
        if (won) {
            GameManager.Instance.points += GameManager.Instance.pointsIncrease*2;
            CurrentFloor++;
            CurrentFloor %= 5;
        } else {
            CurrentFloor = 0;
        }

        GameManager.Instance.RemoveTagged("Pickup");
        GameManager.Instance.UpdateFloorText();

        RemoveFloor();
        NewFloor();
    }

    void Update() {
        foreach (GameObject obj in spawnedRooms) { //add in-game shop back in case it was for some reason removed
            if (obj.name.Contains("Shop")) {
                if (obj.GetComponentsInChildren<InGameShop>().Length < 1) {
                    Instantiate(shopKeep, obj.transform, false);
                }
            }
        }
    }

    /// <summary>
    /// Generates new floor.
    /// </summary>
    public void NewFloor() {
        ResetFloor();
        SetNavMeshSize();

        // create new biome
        roomType = Random.Range(0, biomes.Length);

        // spawn room is guaranteed
        spawnedRooms.Add(Instantiate(CurrentBiome.Spawn, nodes[0], Quaternion.identity));

        // keep spawning until enough rooms have been spawned or broken
        for (int unused = 0; unused < 20000 * roomsPerFloor; unused++) {
            // give a pass-through to all nodes
            for (int i = 0; i < 4; i++) {
                if (CheckNodeEmpty(i) && Random.value < overallRoomRate) {
                    AddRoom(nodes[i]);
                }

                // short-circuit if enough rooms have been reached
                if (spawnedRooms.Count() == roomsPerFloor && CurrentFloor != 5)
                {
                    // Spawns a boss room off to the side
                    spawnedRooms.Add(Instantiate(CurrentBiome.Boss, ReallyFarAway,
                                                 Quaternion.identity));
                    Spawner.Instance.Enemies = CurrentBiome.Enemies;

                    return;
                }
                else if (spawnedRooms.Count() == roomsPerFloor && CurrentFloor == 5)
                {
                    spawnedRooms.Add(Instantiate(finalBoss, ReallyFarAway,
                                                Quaternion.identity));
                    Spawner.Instance.Enemies = CurrentBiome.Enemies;

                    return;
                }
                }

            // Moves nodes to last room generated.
            GenerateNodes(spawnedRooms.Last().transform.position);
        }

        // generate floor to go to next one
        Vector3 nextRoomPos = centralNode;
        nextRoomPos.z += roomSize;
        spawnedRooms.Add(Instantiate(CurrentBiome.Portal, nextRoomPos, Quaternion.identity));
        spawnedRooms.Add(Instantiate(CurrentBiome.Boss, ReallyFarAway, Quaternion.identity));
        Debug.LogError("Could not generate floor. Please try again.");
    }

    /// <summary>
    /// Reset all floor data.
    /// </summary>
    private void ResetFloor() {
        GenerateNodes(centralNode);
        spawnedRooms.Clear();
        spawnedChest = false;
        spawnedShop = false;
        GameManager.Instance.Player.transform.position = new Vector3(0, 1f, 0);
    }

    /// <summary>
    /// Add a room to a given position.
    /// </summary>
    /// <param name="position">Position to place room at.</param>
    private void AddRoom(Vector3 position) {
        // spawn portal on last iteration
        GameObject nextRoom =
            spawnedRooms.Count() + 1 == roomsPerFloor ? CurrentBiome.Portal : RandomRoom();
        spawnedRooms.Add(Instantiate(nextRoom, position, Quaternion.identity) as GameObject);
    }

    /// <summary>
    /// Check if node is occupied.
    /// </summary>
    /// <returns>If node was occupied</returns>
    /// <param name="i">The node to check.</param>
    private bool CheckNodeEmpty(int i) {
        return !GameObject.FindGameObjectsWithTag("Room")
                          .Any(room => room.transform.position == nodes[i]);
    }

    /// <summary>
    /// Generates nodes.
    /// </summary>
    /// <param name="center">Center position.</param>
    private void GenerateNodes(Vector3 center) {
        // generate other nodes
        for (int i = 0; i < 5; i++) {
            nodes[i] = center;
        }

        // space nodes out
        nodes[1].x += roomSize;
        nodes[2].x -= roomSize;
        nodes[3].z += roomSize;
        nodes[4].z -= roomSize;
    }

    private bool spawnedChest = false;
    private bool spawnedShop = false;

    private GameObject RandomRoom() {
        // 10% chance and once per floor
        if (!spawnedChest && Random.value <= chestRate) {
            spawnedChest = true;
            return CurrentBiome.Chest;
        }

        // 15% chance and once per floor
        if (!spawnedShop && Random.value <= shopRate) {
            spawnedShop = true;
            return CurrentBiome.Shop;
        }

        // 75%
        return CurrentBiome.RandomEnemy();
    }

    private void SetNavMeshSize() {
        float size = roomsPerFloor * roomSize;
        NavMeshGen.Instance.NavMeshSize = new Vector3(size, 5, size);
    }

    public void RemoveFloor() {
        NavMeshGen.Reset();
        foreach (var room in spawnedRooms) {
            Destroy(room);
        }
        spawnedRooms.Clear();
    }
}

[System.Serializable]
public struct Biome {
    public string Name;
    [Space(10)]
    public GameObject Spawn;
    public GameObject Chest;
    public GameObject Shop;
    public GameObject Portal;
    public GameObject Boss;
    public GameObject Door;
    [Space(10)]
    public GameObject[] EnemyRooms;
    [Space(10)]
    public GameObject[] Enemies;

    public GameObject RandomEnemy() {
        return EnemyRooms[Random.Range(0, EnemyRooms.Length)];
    }
}
