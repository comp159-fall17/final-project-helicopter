using System.Linq;
using System.Collections;
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
    public GameObject SpawnPoints;

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
        StartCoroutine(Spawning());
    }

    private IEnumerator Spawning() {
        NavMeshGen.Instance.NavMeshPosition = transform.position;
        NavMeshGen.Instance.NavMeshSize = new Vector3(41, 5, 41);

        yield return new WaitForSeconds(0.1f);

        List<Vector3> spawnPoints = GetSpawnPoints();

        for (int i = 0; i < EnemiesPerWave + LevelGen.Instance.CurrentFloor; i++) {
            // Uncomment if we figure this out
            //Spawner.Instance.SpawnEnemy(transform.position);

            Vector3 randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count())];
            Spawner.Instance.SpawnEnemyAtLocation(randomPoint);
            spawnPoints.Remove(randomPoint);
        }
        DoorScript.CanCross = false;
        GameManager.Instance.enemyRoom = true;
    }

    List<Vector3> GetSpawnPoints() {
        List<Vector3> list = new List<Vector3>();
        for (int i = 0; i < SpawnPoints.transform.childCount; i++) {
            list.Add(SpawnPoints.transform.GetChild(i).transform.position);
        }
        return list;
    }
}
