using System;
using UnityEditor;
using UnityEngine;

public class GridNodeCreator : EditorWindow
{
    private GameObject _node;

    private int xNodes;
    private int yNodes;

    private Vector3 startPosition = new Vector3(0,0,0);

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
        GameObject parent = GameObject.Find("ContenedorDeNodos");
        for (int i = 0; i < xNodes; i++)
        {
            for (int j = 0; j < yNodes; j++)
            {
                //Debug.Log(startPosition + new Vector3(i, 0f, j));
                Vector3 pos = startPosition + new Vector3(i, 0f, j);
                Instantiate(_node, pos, Quaternion.identity, parent.transform);
                //grid[i, j] = Instantiate(tile, start + new Vector3(i, j, 0f), Quaternion.identity);
            }
        }
    }
}
