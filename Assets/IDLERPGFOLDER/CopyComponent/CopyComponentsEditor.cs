using UnityEngine;
using UnityEditor;
using System.Reflection;

public class CopyComponentsEditor : EditorWindow
{
    GameObject sourceObject;
    GameObject targetObject;

    [MenuItem("Tools/Copy Components")]
    static void OpenWindow()
    {
        GetWindow<CopyComponentsEditor>("Copy Components");
    }

    void OnGUI()
    {
        sourceObject = (GameObject)EditorGUILayout.ObjectField("Source Object", sourceObject, typeof(GameObject), true);
        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);

        if (GUILayout.Button("Copy Components"))
        {
            if (sourceObject != null && targetObject != null)
                CopyAllComponents(sourceObject, targetObject);
            else
                Debug.LogError("Please assign both Source and Target Objects!");
        }
    }

    void CopyAllComponents(GameObject from, GameObject to)
    {
        foreach (Component comp in from.GetComponents<Component>())
        {
            if (comp is Transform) continue;

            Component newComp = to.AddComponent(comp.GetType());

            foreach (FieldInfo field in comp.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                field.SetValue(newComp, field.GetValue(comp));
            }
        }
        Debug.Log("Components copied successfully!");
    }
}