using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
public class StageManager : MonoBehaviour
{
    public List<GameObject> mapPrefabs;
    private GameObject currentMap;
    public NavMeshSurface navMeshSurface;
    [SerializeField] private int startingMapIndex = 0;
    private void Start()
    {
        // ตรวจสอบว่ามี NavMeshSurface ถูกกำหนดไว้หรือไม่
        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface is not assigned to StageManager!");
            return;
        }

        // สร้างด่านแรกทันทีที่เกมเริ่ม
        ChangeMap(startingMapIndex);
    }

    public void ChangeMap(int mapIndex)
    {
        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        if (mapIndex >= 0 && mapIndex < mapPrefabs.Count)
        {
            currentMap = Instantiate(mapPrefabs[mapIndex], Vector3.zero, Quaternion.identity);
         //   StartCoroutine(BakeNavMeshCoroutine());
            //  BakeNavMesh();
        }
        else
        {
            Debug.LogError("Invalid map index: " + mapIndex);
        }
    }

   /* private void BakeNavMesh()
    {
        if (navMeshSurface != null)
        {
            // Bake NavMesh
            navMeshSurface.BuildNavMesh();
        }
        else
        {
            Debug.LogError("NavMeshSurface is not assigned!");
        }
    }*/
   
   private IEnumerator BakeNavMeshCoroutine()
   {
       yield return new WaitForEndOfFrame(); // รอให้เฟรมปัจจุบันเสร็จก่อน
       if (navMeshSurface != null)
       {
           navMeshSurface.BuildNavMesh();
       }
   }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
          ChangeMap(1);
        }
    }
}
