using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(-180)]
public class NavMeshGen : MonoBehaviour {
    public static NavMeshGen Instance;

    static readonly List<EnemyRoom> Rooms = new List<EnemyRoom>();

    /// <summary>
    /// Subscribe a certain room, e.g. <code>NavMeshGen.Subscribe(this)</code>
    /// </summary>
    /// <param name="room">The room to subscribe.</param>
    public static void Subscribe(EnemyRoom room) {
        if (!Rooms.Exists(i => i.Equals(room))) {
            Rooms.Add(room);
        }
    }

    /// <summary>
    /// Unsubscribe a certain room, e.g. <code>NavMeshGen.Unsubscribe(this)</code>
    /// </summary>
    /// <param name="room">The room to subscribe.</param>
    public static void Unsubscribe(EnemyRoom room) {
        Rooms.Remove(room);
    }

    /// <summary>
    /// Unsubscribe all rooms.
    /// </summary>
    public static void Reset() {
        Rooms.Clear();
    }

    public Vector3 NavMeshPosition;
    public Vector3 NavMeshSize;

    NavMeshData data;
    NavMeshDataInstance dataInstance;

    void Start() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    private void Update() {
        GenerateNavMesh();
    }

    void OnEnable() {
        data = new NavMeshData();
        dataInstance = NavMesh.AddNavMeshData(data);

        GenerateNavMesh();
    }

    void OnDisable() {
        dataInstance.Remove();
    }

    public void GenerateNavMesh() {
        if (Rooms.Count == 0) {
            return;
        }

        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByID(0);
        Bounds localBounds = new Bounds(NavMeshPosition, NavMeshSize);

        foreach (EnemyRoom room in Rooms) {
            room.Collect(ref sources);
        }

        NavMeshBuilder.UpdateNavMeshData(data, buildSettings, sources, localBounds);
    }
}
