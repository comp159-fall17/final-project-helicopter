using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(-183)]
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

    public bool ActiveOnStart;
    public MeshFilter[] SourceObjects;

    public bool IsActive { get; private set; }

    void Start() {
        IsActive = ActiveOnStart;

        if (ActiveOnStart) {
            NavMeshGen.Subscribe(this);
        }
    }

    public void Toggle(bool status) {
        IsActive = status;

        if (IsActive) {
            NavMeshGen.Subscribe(this);
        } else {
            NavMeshGen.Unsubscribe(this);
        }
    }

    public void Collect(ref List<NavMeshBuildSource> sources) {
        foreach (MeshFilter filter in SourceObjects) {
            sources.Add(CreateSourceFromMeshFilter(filter));
        }
    }
}
