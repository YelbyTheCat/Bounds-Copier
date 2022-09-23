using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoundsCopier : EditorWindow
{
    [MenuItem("Yelby/Bounds Copier")]
    public static void ShowWindow() { GetWindow<BoundsCopier>("Bounds Copier 1.0.0"); }

    GameObject source;
    SkinnedMeshRenderer skinnedMeshRenderer;
    List<GameObject> targets = new List<GameObject>();
    Transform sourceTransform;

    private void OnGUI()
    {
        GUILayout.Label("Version: 1.0.0");

        source = EditorGUILayout.ObjectField("Source: ", source, typeof(GameObject), true) as GameObject;
        if (source == null) return;
        skinnedMeshRenderer = null;
        if(source.GetComponent<SkinnedMeshRenderer>() == null)
        {
            source = null;
            GUILayout.Label("Missing Skinned Mesh Renderer");
            return;
        }
        skinnedMeshRenderer = source.GetComponent<SkinnedMeshRenderer>();

        sourceTransform = null;
        if(skinnedMeshRenderer.rootBone.transform == null)
        {
            sourceTransform = null;
            GUILayout.Label("Missing Root bone transform");
            return;
        }
        sourceTransform = skinnedMeshRenderer.rootBone.transform;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Selected"))
        {
            GameObject[] holdingArray = Selection.gameObjects;
            Debug.Log(holdingArray.Length);
            if (holdingArray.Length == 0) return;

            foreach (GameObject gameObject in holdingArray)
                targets.Add(gameObject);
        }

        if (GUILayout.Button("Clear List"))
        {
            targets = new List<GameObject>();
        }

        EditorGUILayout.EndHorizontal();

        if(targets.Count > 0)
            if(GUILayout.Button("Copy Bounds"))
            {
                for (int i = 0; i < targets.Count - 1; i++)
                {
                    SkinnedMeshRenderer targetMesh = targets[i].GetComponent<SkinnedMeshRenderer>();
                    if(targetMesh == null) continue;
                    targetMesh.localBounds = skinnedMeshRenderer.localBounds;
                    targetMesh.rootBone = sourceTransform;
                    targetMesh.probeAnchor = skinnedMeshRenderer.probeAnchor;
                }
            }


        // Adds blanks to the bottom
        if (targets.Count == 0 || targets[targets.Count - 1] != null)
            targets.Add(default);

        // Removes null elements
        for (int i = 0; i < targets.Count - 1; i++)
        {
            if (targets[i] == null)
            {
                targets.Remove(targets[i]);
                i = 0;
            }
        }

        // Removes duplicates and non-childs
        for (int i = 0; i < targets.Count; i++)
        {
            // Duplicate
            if (hasDouble(targets, targets[i], out int index))
            {
                targets.RemoveAt(index);
                i = 0;
            }
        }

        for (int i = 0; i < targets.Count; i++)
        {
            targets[i] = EditorGUILayout.ObjectField(targets[i], typeof(GameObject), true) as GameObject;
        }
    }

    public static bool checkList(List<GameObject> objectsList, GameObject gameObject)
    {
        for (int i = 0; i < objectsList.Count; i++)
            if (objectsList[i] == gameObject)
                return true;
        return false;
    }

    public static bool hasDouble(List<GameObject> objectsList, GameObject gameObject, out int index)
    {
        index = -1;
        int count = 0;
        for (int i = 0; i < objectsList.Count; i++)
        {
            if (objectsList[i] == gameObject)
            {
                count++;
                if (count > 1)
                {
                    index = i;
                    break;
                }
            }
        }
        return count > 1;
    }
}
