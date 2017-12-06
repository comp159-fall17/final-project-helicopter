using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRoom : MonoBehaviour {
    /// <summary>
    /// Creates the source from a mesh filter.
    /// 
    /// Adapted from https://github.com/Unity-Technologies/NavMeshComponents/blob/8d95e981f6575b4c63908451a770c2f43341a0f4/Assets/Examples/Scripts/NavMeshSourceTag.cs
    /// </summary>
    /// <returns>The source from mesh filter.</returns>
    /// <param name="filter">Filter.</param>
    public static NavMeshBuildSource CreateSourceFromMeshFilter(MeshFilter filter) {
        return new NavMeshBuildSource {
            area = 0,
            shape = NavMeshBuildSourceShape.Mesh,
            sourceObject = filter.sharedMesh,
            transform = filter.transform.localToWorldMatrix
        };
    }

    [Header("NavMesh Building")]
    public bool ActiveOnStart;
    public MeshFilter[] SourceObjects;
    [Space(10)]
    [Header("Enemy spawning")]
    public int EnemiesPerWave;

    public bool IsActive { get; private set; }

    void Start() {
        IsActive = ActiveOnStart;

        if (ActiveOnStart) {
            NavMeshGen.Subscribe(this);
        }
    }

    /// <summary>
    /// Switches participation in the navmesh drawing process. 
    /// </summary>
    /// <param name="status">Should participate in NavMesh building process.</param>
    public void Toggle(bool status) {
        IsActive = status;

        if (IsActive) {
            NavMeshGen.Subscribe(this);
        } else {
            NavMeshGen.Unsubscribe(this);
        }

        NavMeshGen.Instance.GenerateNavMesh();
    }

    /// <summary>
    /// Collects all source meshes.
    /// </summary>
    /// <param name="sources">Sources list (for performance).</param>
    public void Collect(ref List<NavMeshBuildSource> sources) {
        foreach (MeshFilter filter in SourceObjects) {
            sources.Add(CreateSourceFromMeshFilter(filter));
        }
    }

    public void Spawn() {
        for (int i = 0; i < EnemiesPerWave; i++) {
            Spawner.Instance.SpawnEnemy(transform.position);
        }
        DoorScript.CanCross = false;
    }
}
