using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GridView))]
public class GridViewInspector : Editor {

	    void OnEnable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            var obj = (GridView)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Generate Patch"))
            {
                obj.Generate(0, 0);
            }
            if (GUILayout.Button("Test"))
            {
                obj.TestWorld();
            }
        }

        public void OnSceneGUI()
        {
            var obj = (GridView)target;
            var normal = obj.transform.forward;
            var radius = 0.25f;
            var offset = new Vector3(0.5f, 0.5f, 0);
            Patch patch;

            if (obj.patches!=null && obj.patches.TryGetValue(Vector2i.zero, out patch))
            {
                foreach (var ci in patch.colliders)
                {
                    var cg = ci.cornerGraph;
                    foreach (var t in cg.nodes)
                    {
                        Handles.DrawSolidDisc((Vector3)t.value.position + offset, normal, radius);
                    }
                    foreach (var e in cg.edges)
                    {
                        Handles.DrawLine((Vector3)e.a.value.position + offset, (Vector3)e.b.value.position + offset);
                    }
                }
            }
        } 
}
