using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class SaveNavMesh : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    [ContextMenu("Save NavMesh")]
    void SavedNavMesh()
    {
        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface ยังไม่ได้ถูกกำหนด!");
            return;
        }

        // สร้าง NavMeshData ใหม่
        NavMeshData navMeshData = new NavMeshData();
        NavMesh.AddNavMeshData(navMeshData);

        // Bake NavMesh ลงไปใน NavMeshData
        navMeshSurface.BuildNavMesh();

        // บันทึกเป็นไฟล์ Asset
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(navMeshData, "Assets/SavedNavMesh.asset");
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        Debug.Log("✅ บันทึก NavMeshData เรียบร้อย!");
    }
}