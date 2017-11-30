using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoomSourceTag : MonoBehaviour {
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

    public bool Active;

    void Start() {
        if (Active) {
            NavMeshGen.Subscribe(this);
        }
    }

    public void Collect(ref List<NavMeshBuildSource> sources) {
        foreach (MeshFilter filter in GetComponentsInChildren<MeshFilter>()) {
            sources.Add(CreateSourceFromMeshFilter(filter));
        }
    }
}
