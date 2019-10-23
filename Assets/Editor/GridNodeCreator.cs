using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridNodeCreator : EditorWindow
{
    private GameObject _node;

    private int xNodes;
    private int yNodes;

    private Vector3 startPosition = new Vector3(0,0,0);

    private List<GameObject> nodes;

    [MenuItem("Pathfinding Editor/Create Nodes")]
    static void Init()
    {
        GridNodeCreator window =
            (GridNodeCreator)EditorWindow.GetWindow(typeof(GridNodeCreator));
    }

    void OnGUI()
    {
        ShowPrefab();
        ShowPositions();

        if (GUILayout.Button("Create Grid"))
        {
            Debug.Log("Crea la grilla");
            CreateGrid();
            Close();
        }
    }

    private void ShowPositions()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        xNodes = EditorGUILayout.IntField("Nodes in X", xNodes);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        yNodes = EditorGUILayout.IntField("Nodes in Y", yNodes);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void ShowPrefab()
    {
        GUILayout.BeginVertical();
        _node = (GameObject)EditorGUILayout.ObjectField("Node Prefab ", _node, typeof(GameObject), false);
        GUILayout.EndVertical();
    }

    private void CreateGrid()
    {
        nodes = new List<GameObject>();
        GameObject parent = GameObject.Find("Navigation");
        parent.transform.rotation = Quaternion.Euler(0, 0, 0);
        for (int i = 0; i < xNodes; i++)
        {
            for (int j = 0; j < yNodes; j++)
            {
                Vector3 pos = startPosition + new Vector3(i, 0f, j);
                GameObject goNode = Instantiate(_node, pos, Quaternion.identity, parent.transform);
                nodes.Add(goNode);
            }
        }

        parent.transform.rotation = Quaternion.Euler(0, 45, 0);

        for (int i = 0; i < nodes.Count; i++)
        {
            Collider[] shooteables = Physics.OverlapSphere(nodes[i].transform.position, 1);

            if (shooteables.Length > 0)
            {
                for (int r = 0; r < shooteables.Length; r++)
                {
                    if (shooteables[r].gameObject.layer == LayerMask.NameToLayer(StringTagManager.maskShootable))
                    {
                        DestroyImmediate(nodes[i].gameObject);
                        break;
                    }
                }
            }
        }
    }
}
