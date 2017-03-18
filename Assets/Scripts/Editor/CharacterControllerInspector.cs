using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CharacterController))]
public class CharacterControllerInspector : Editor
{
    const string k_CharacterControllerKey = "CharacterController";
    bool showPlayerTiles;

    void OnEnable()
    {
        showPlayerTiles = PlayerPrefs.GetInt(string.Format("{0}.{1}", k_CharacterControllerKey, "showPlayerTiles")) > 0;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);
        GUILayout.Label("Debug");
        var spt = EditorGUILayout.Toggle("Show Player Tiles", showPlayerTiles);
        if (spt != showPlayerTiles)
        {
            showPlayerTiles = spt;
            SaveBool("showPlayerTiles", showPlayerTiles);
        }
    }

    public void OnSceneGUI()
    {
        var obj = (CharacterController)target;
        if (showPlayerTiles)
        {
            Handles.color = Color.green;
            var box = obj.rigidbody.GetComponent<Collider2D>().bounds;
            var tiles = Recti.GetBoundingRect(box.min.x, box.min.y, box.size.x, box.size.y);
            for (int x = tiles.xMin; x < tiles.xMax; x++)
            {
                for (int y = tiles.yMin; y < tiles.yMax; y++)
                {
                    Handles.DrawWireCube(new Vector3((float)x + 0.5f, (float)y + 0.5f, 0), Vector3.one);
                }
            }
        }
    }

    void SaveBool(string key, bool val)
    {
        PlayerPrefs.SetInt(string.Format("{0}.{1}", k_CharacterControllerKey, key), val ? 1 : 0);
    }
}

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerInspector : CharacterControllerInspector { }
