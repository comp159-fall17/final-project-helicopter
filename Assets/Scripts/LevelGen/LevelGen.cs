using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LevelGen : MonoBehaviour {
    public static LevelGen Instance;

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
    public Biome[] biomes;

    public GameObject Door { get { return CurrentBiome.Door; } }
    public int CurrentFloor { get; private set; }

    private int roomType = 0;

    private List<GameObject> spawnedRooms = new List<GameObject>();
    private Vector3[] nodes;
    private Vector3 originalNode;

    private Biome CurrentBiome { get { return biomes[roomType]; } }

    void Start() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        originalNode = centralNode;
        nodes = new Vector3[5];
        GenerateNodes(centralNode);
        CurrentFloor = 0;

        NewFloor();
    }

    public void ReloadFloor(bool won) {
        if (won) {
            CurrentFloor++;
            CurrentFloor %= 5;
        } else {
            CurrentFloor = 0;
        }

        RemoveFloor();
        NewFloor();
    }

    /// <summary>
    /// Generates new floor.
    /// </summary>
    public void NewFloor() {
        ResetFloor();
        SetNavMeshSize();
        centralNode = originalNode;

        // create new biome
        roomType = Random.Range(0, biomes.Length);

        // spawn room is guaranteed
        spawnedRooms.Add(Instantiate(CurrentBiome.Spawn, nodes[0], Quaternion.identity));

        // Spawns the boss room at 500, 510 so the player isnt spawned in the middle of the room
        spawnedRooms.Add(Instantiate(CurrentBiome.Boss, new Vector3(500, 0, 510), Quaternion.identity)); 

        // keep spawning until enough rooms have been spawned
        while (true) {
            // give a pass-through to all nodes
            for (int i = 0; i < 4; i++) {
                if (Random.value < overallRoomRate && CheckNodeEmpty(i)) {
                    AddRoom(nodes[i]);
                }

                // short-circuit if enough rooms have been reached
                if (spawnedRooms.Count() == roomsPerFloor) {
                    return;
                }
            }

            // Moves nodes to last room generated.
            GenerateNodes(spawnedRooms.Last().transform.position);
        }
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
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
        foreach (GameObject room in rooms) {
            if (room.transform.position == nodes[i]) {
                return false;
            }
        }
        return true;
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
        foreach (var item in GameObject.FindGameObjectsWithTag("Room")) {
            Destroy(item);
        }
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
    public GameObject[] Enemy;

    public GameObject RandomEnemy() {
        return Enemy[Random.Range(0, Enemy.Length)];
    }
}
