using UnityEngine;
using System.Collections;

public class PatchView : MonoBehaviour {

    public Patch patch;
    public MeshFilter meshFilter;
    public PolygonCollider2D collider;

    public void SetPatch(Patch patch)
    {
        this.patch = patch;
    }

    public void Enable()
    {
        if (meshFilter != null)
        {
            patch.SetDrawMesh(meshFilter);
        }

        if (collider != null)
        {
            var sw = new System.Diagnostics.Stopwatch();
            var time = System.TimeSpan.Zero;
            collider.pathCount = 0;
            for (int i = 0; i < patch.colliders.Count; i++)
            {
                sw.Reset();
                sw.Start();
                if (patch.colliders[i].points != null && patch.colliders[i].points.Length > 0)
                {
                    collider.pathCount++;
                    collider.SetPath(collider.pathCount-1, patch.colliders[i].points);
                }
                if (sw.Elapsed > time)
                {
                    time = sw.Elapsed;
                }
            }
            Debug.Log("Max collider creation time: " + time);
        }
    }
}
