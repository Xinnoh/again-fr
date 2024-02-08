using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderNavMesh : MonoBehaviour
{
    public void UpdateMesh()
    {
        NavMeshPlus.Components.NavMeshSurface navMeshScript = GetComponent<NavMeshPlus.Components.NavMeshSurface>();
        navMeshScript.BuildNavMesh();
    }

}
