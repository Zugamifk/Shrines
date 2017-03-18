using UnityEngine;
using System.Collections;

public class PatchView : MonoBehaviour
{

    public Patch patch;
    public MeshFilter worldMeshFilter;
    public MeshFilter platformMeshFilter;
    public PolygonCollider2D worldColliderTemplate;
    public PolygonCollider2D platformColliderTemplate;

    PolygonCollider2D worldCollider;
    PolygonCollider2D platformCollider;

    Transform collidersRoot;

    public void SetPatch(Patch patch)
    {
        this.patch = patch;
    }

    public void Enable()
    {
        if (worldMeshFilter != null)
        {
            patch.SetDrawMesh(worldMeshFilter, "ground");
        }

        if (platformMeshFilter != null)
        {
            patch.SetDrawMesh(platformMeshFilter, "platform");
        }

        if (collidersRoot != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(collidersRoot);
#else
            Destroy(collidersRoot);
#endif
        }

        collidersRoot = (new GameObject("colliders")).transform;
        collidersRoot.SetParent(transform, false);

        worldCollider = Instantiate<PolygonCollider2D>(worldColliderTemplate);
        worldCollider.transform.SetParent(collidersRoot, false);
        worldCollider.gameObject.SetActive(true);

        platformCollider = Instantiate<PolygonCollider2D>(platformColliderTemplate);
        platformCollider.transform.SetParent(collidersRoot, false);
        platformCollider.gameObject.SetActive(true);

        var sw = new System.Diagnostics.Stopwatch();
        var time = System.TimeSpan.Zero;
        for (int i = 0; i < patch.colliders.Count; i++)
        {
            sw.Reset();
            sw.Start();
            var c = patch.colliders[i];
            switch (c.collisionType)
            {
                case TileData.CollisionType.Solid:
                    {
                        if (c.points != null && c.points.Length > 0)
                        {
                            worldCollider.pathCount++;
                            worldCollider.SetPath(worldCollider.pathCount - 1, c.points);
                        }
                    } break;
                case TileData.CollisionType.Platform:
                    {
                        if (c.points != null && c.points.Length > 0)
                        {
                            platformCollider.pathCount++;
                            platformCollider.SetPath(platformCollider.pathCount - 1, c.points);
                        }
                    } break;
                default: break;
            }

            if (sw.Elapsed > time)
            {
                time = sw.Elapsed;
            }
        }
        Debug.Log("Max collider creation time: " + time);
    }
}
