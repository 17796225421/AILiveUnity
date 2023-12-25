using UnityEngine;
using UnityEditor;

public class HierarchyPrinter : MonoBehaviour
{
    [MenuItem("Tools/Print Hierarchy")]
    private static void PrintHierarchy()
    {
        if (Selection.activeTransform != null)
        {
            string hierarchyText = GetTransformHierarchy(Selection.activeTransform, "");
            Debug.Log(hierarchyText);
        }
    }

    private static string GetTransformHierarchy(Transform trans, string indent)
    {
        string text = indent + "- " + trans.name + "\n";
        foreach (Transform child in trans)
        {
            text += GetTransformHierarchy(child, indent + "  ");
        }
        return text;
    }
}
