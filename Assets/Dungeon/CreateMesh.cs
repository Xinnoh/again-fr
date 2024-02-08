using Edgar.Unity;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine;
using NavMeshPlus.Extensions;
using NavMeshPlus.Components;
using Unity.AI.Navigation;
using UnityEngine.UI;
using System.Collections;


public class CreateMesh : DungeonGeneratorPostProcessingComponentGrid2D
{
    public GameObject navMeshComponent;
    public float delay = 0.5f;


    public override void Run(DungeonGeneratorLevelGrid2D level)
    {
        StartCoroutine(ExecuteWithDelay());
    }

    private IEnumerator ExecuteWithDelay()
    {

        yield return new WaitForSeconds(delay);
        CompositeCollider2D floorFilter;
        Mesh floorMesh;

        GameObject floorTiles = GameObject.FindWithTag("FloorTiles");
        

        floorFilter = floorTiles.GetComponent<CompositeCollider2D>();
        floorMesh = floorFilter.CreateMesh(false, false);

        GameObject floorTemp = new GameObject("floorTemp");

        floorTemp.AddComponent<MeshFilter>().mesh = floorMesh;
        floorTemp.AddComponent<MeshRenderer>();

        floorTemp.transform.position = floorTiles.transform.parent.position;


        NavMeshPlus.Components.NavMeshModifier floorMesh2 = floorTemp.AddComponent<NavMeshPlus.Components.NavMeshModifier>();
        floorMesh2.overrideArea = true;


        NavMeshPlus.Components.NavMeshSurface navMeshScript = navMeshComponent.GetComponent<NavMeshPlus.Components.NavMeshSurface>();
        navMeshScript.BuildNavMesh();


        DestroyImmediate(floorTemp);

    }
}