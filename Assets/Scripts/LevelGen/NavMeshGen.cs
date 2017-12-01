using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(-180)]
public class NavMeshGen : MonoBehaviour {
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

    NavMeshData data;
    NavMeshDataInstance instance;

    void Update() {
        GenerateNavMesh();
    }

    void OnEnable() {
        data = new NavMeshData();
        instance = NavMesh.AddNavMeshData(data);

        GenerateNavMesh();
    }

    void OnDisable() {
        instance.Remove();
    }

    void GenerateNavMesh() {
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByID(0);
        Bounds localBounds = new Bounds(transform.position, new Vector3(60, 5, 60));

        foreach (RoomSourceTag room in Rooms) {
            room.Collect(ref sources);
        }

        NavMeshBuilder.UpdateNavMeshData(data, buildSettings, sources, localBounds);
    }
}
