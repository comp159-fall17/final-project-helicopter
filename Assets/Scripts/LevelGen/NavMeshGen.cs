using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(-180)]
public class NavMeshGen : MonoBehaviour {
    public static NavMeshGen Instance;

    static readonly List<RoomSourceTag> Rooms = new List<RoomSourceTag>();

    /// <summary>
    /// Subscribe a certain room, e.g. <code>NavMeshGen.Subscribe(this)</code>
    /// </summary>
    /// <param name="room">The room to subscribe.</param>
    public static void Subscribe(RoomSourceTag room) {
        if (!Rooms.Exists(i => i.Equals(room))) {
            Rooms.Add(room);
        }
    }

    /// <summary>
    /// Unsubscribe a certain room, e.g. <code>NavMeshGen.Unsubscribe(this)</code>
    /// </summary>
    /// <param name="room">The room to subscribe.</param>
    public static void Unsubscribe(RoomSourceTag room) {
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

    void OnEnable() {
        data = new NavMeshData();
        dataInstance = NavMesh.AddNavMeshData(data);

        GenerateNavMesh();
    }

    void OnDisable() {
        dataInstance.Remove();
    }

    public void GenerateNavMesh() {
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByID(0);
        Bounds localBounds = new Bounds(NavMeshPosition, NavMeshSize);

        foreach (RoomSourceTag room in Rooms) {
            room.Collect(ref sources);
        }

        NavMeshBuilder.UpdateNavMeshData(data, buildSettings, sources, localBounds);
    }
}
