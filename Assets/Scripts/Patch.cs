using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Patch
{
    public class ColliderInfo
    {
        public TileData.CollisionType collisionType;
        public Graph<Tile> cornerGraph;
        public Vector2[] points;
        public Vector2i position;
        public ColliderInfo(Graph<Tile> colliderGraph)
        {
            collisionType = colliderGraph.nodes[0].value.data.collisions;
            cornerGraph = colliderGraph;
        }
    }

    public class MeshInfo
    {
        public Mesh mesh;
        public List<Tile> tiles = new List<Tile>();
    }
    public int width;
    public int height;

    public List<Tile> currentTiles;
    public Dictionary<string, MeshInfo> meshes;
    public Graph<Tile> edgeGraph;
    public List<ColliderInfo> colliders;

    public Patch(int w, int h)
    {
        width = w;
        height = h;
    }

    public void SetDrawMesh(MeshFilter filter, string key)
    {
        MeshInfo drawMesh = null;
        meshes.TryGetValue(key, out drawMesh);
        filter.mesh = drawMesh.mesh;
    }

    public void Generate(List<Tile> tileList, TileSet tileSet)
    {
        currentTiles = new List<Tile>(tileList);
        meshes = new Dictionary<string, MeshInfo>();
        meshes["ground"] = new MeshInfo();
        meshes["platform"] = new MeshInfo();
        foreach (var t in tileList)
        {
            string type = "";
            switch (t.data.collisions)
            {
                case TileData.CollisionType.Platform:
                    {
                        type = "platform";
                    } break;
                case TileData.CollisionType.Solid:
                    {
                        type = "ground";
                    } break;
            }

            if (!string.IsNullOrEmpty(type))
            {
                meshes[type].tiles.Add(t);
            }
        }
        GenerateDrawMesh(meshes["ground"].tiles, "ground");
        GenerateDrawMesh(meshes["platform"].tiles, "platform");
        BuildEdgeGraph();
        GenerateColliderPaths();
    }

    public void PaintAll(TilePainting painting)
    {
        Paint(painting, "ground");
        Paint(painting, "platform");
    }

    public void Paint(TilePainting painting, string key)
    {
        MeshInfo drawMesh = null;
        meshes.TryGetValue(key, out drawMesh);
        if (drawMesh != null)
        {
            
            var uv = drawMesh.mesh.uv;
            painting.Paint(drawMesh.tiles, uv);
            drawMesh.mesh.uv = uv;
        }
    }

    void GenerateColliderPaths()
    {
        var genned = 0;
        var sw = new System.Diagnostics.Stopwatch();
        var time = System.TimeSpan.Zero;
        foreach (var ci in colliders)
        {
            sw.Reset();
            sw.Start();
            switch (ci.collisionType)
            {
                case TileData.CollisionType.Platform:
                    {
                        if (GetPolygonCollider(ci)) genned++;
                    }; break;
                case TileData.CollisionType.Solid:
                    {
                        if (GetPolygonCollider(ci)) genned++;
                    }; break;
                default: break;
            }
            if (sw.Elapsed > time)
            {
                time = sw.Elapsed;
            }
        }
        Debug.Log("Edge Colliders: " + genned + " Max time: " + time);
    }

    bool GetPolygonCollider(ColliderInfo ci)
    {
        var g = ci.cornerGraph;
        var n = g.nodes[0];
        if (g.nodes.Count == 1)
        {
            var pos = (Vector2)n.value.position;
            var pts = new Vector2[] {
                    pos,
                    pos+Vector2.up,
                    pos + Vector2.one,
                    pos+Vector2.right
                };
            ci.points = pts;
            ci.position = pos;
            return true;
        }
        else
        {
            int dir = 1;
            bool found = false;
            for (; dir < 8; dir += 2)
            {
                var nbr = n.value.neighbours[dir];
                if (nbr == null || nbr.IsEmpty)
                {
                    found = true;
                }
                else if (found && nbr != null && !nbr.IsEmpty)
                {
                    break;
                }
            }
            int start = dir % 8;
            n = GetNext(g, n, start);
            var n0 = n;
            var pts = new List<Vector2>();
            do
            {
                var next = dir + 6;
                var end = next + 8;
                var pos = (Vector2)n.value.position;
                for (; next < end; next = next + 2)
                {
                    var nbr = n.value.neighbours[next % 8];
                    if (nbr == null || nbr.IsEmpty)
                    {
                        if (next > dir + 6)
                        {
                            pts.Add(pos + GetQuadCorner((next + 6) % 8));
                        }
                    }
                    else if (nbr != null && !nbr.IsEmpty)
                    {
                        if (next == dir + 6)
                        {
                            pts.Add(pos + GetQuadCorner((dir + 4) % 8));
                        }
                        break;
                    }
                }
                if (next != end)
                {
                    dir = next % 8;
                    var nn = GetNext(g, n, dir);
                    if (nn == null)
                    {
                        n.value.status = string.Format("Tile missing neighbour! {0}", dir);
                        Debug.LogErrorFormat("Tile missing neighbour! {0}", dir);
                        break;
                    }
                    n = nn;
                }
            } while (n0 != n || start != dir);
            if (n != null && pts.Count > 0)
            {
                var pos = (Vector2)n.value.position;
                var path = new Vector2[pts.Count];
                for (int i = 0; i < pts.Count; i++)
                {
                    path[i] = pts[i];
                }
                ci.points = path;
                ci.position = pos;
                return true;
            }

        }
        return false;
    }

    bool GetPlatformCollider(ColliderInfo ci)
    {
        var g = ci.cornerGraph;
        var n = g.nodes[0];
        if (g.nodes.Count == 1)
        {
            var pos = (Vector2)n.value.position;
            var pts = new Vector2[] {
                    pos+Vector2.up,
                    pos + Vector2.one
                };
            ci.points = pts;
            ci.position = pos;
            return true;
        }
        else if (g.nodes.Count == 2)
        {
            var n2 = g.nodes[1];
            bool swap = (n2.value.position.x < n.value.position.x);
            var a = swap ? n2.value.position : n.value.position;
            var b = swap ? n.value.position : n2.value.position;
            var pts = new Vector2[] {
                    a+Vector2i.up,
                    b + Vector2i.one
                };
            ci.points = pts;
            ci.position = a;
            return true;
        }
        return false;
    }

    Vector2 GetQuadCorner(int side)
    {
        switch (side)
        {
            case 1: return Vector2.right;
            case 3: return Vector2.one;
            case 5: return Vector2.up;
            default: return Vector2.zero;
        }
    }

    Graph<Tile>.Node GetNext(Graph<Tile> graph, Graph<Tile>.Node start, int direction)
    {
        var t0 = start.value;
        for (int i = 0; i < start.connected.Count; i++)
        {
            var n = start.connected[i];
            var t1 = n.value;
            switch (direction)
            {
                case 1:
                    {
                        if (t0.position.y > t1.position.y)
                        {
                            return n;
                        }
                    } break;
                case 3:
                    {
                        if (t0.position.x < t1.position.x)
                        {
                            return n;
                        }
                    } break;
                case 5:
                    {
                        if (t0.position.y < t1.position.y)
                        {
                            return n;
                        }
                    } break;
                case 7:
                    {
                        if (t0.position.x > t1.position.x)
                        {
                            return n;
                        }
                    } break;
            }
        }
        return null;
    }

    void BuildEdgeGraph()
    {
        edgeGraph = new Graph<Tile>(currentTiles);
        edgeGraph.Filter(t => !t.value.IsEmpty && t.value.HasNeighbour(n => n == null || n.IsEmpty, true));
        edgeGraph.Connect((t, l) =>
        {
            foreach (var n in t.neighbours)
            {
                // only connect nodes with the same collision type and which are not empty or diagonal to each other
                if (n != null && (!t.IsDiagonal(n)) && edgeGraph.Contains(n) && n.data.collisions == t.data.collisions)
                {
                    l.Add(n);
                }
            }
        });
        edgeGraph.BFS();
        colliders = new List<ColliderInfo>();
        Debug.Log(edgeGraph.forest.Count);
        int e0 = 0, e1 = 0;
        // build a graph representing the perimeter of each connected grid
        foreach (var g in edgeGraph.forest)
        {
            if (e0++ > 1000) break;
            e1 = 0;
            var nl = new Graph<Tile>();

            for (int i = 0; i < g.nodes.Count; i++)
            {
                var n = g.nodes[i];
                bool added = false;

                for (int ni = 1; ni < 8 && !added; ni += 2)
                {
                    var e = n.value.neighbours[ni];
                    if (e == null || e.IsEmpty)
                    {
                        // get left and right neighbours
                        var li = (ni + 2) % 8;
                        var ri = (ni - 2 + 8) % 8;
                        var l = n.value.neighbours[li];
                        var r = n.value.neighbours[ri];
                        // treat tiles off the edge as empty tiles
                        // if either both neighbours are empty or neither, this is an important corner
                        var rc = r == null || r.IsEmpty;
                        var lc = l == null || l.IsEmpty;
                        if (rc || lc)
                        {
                            nl.Add(n.value);
                            added = true;
                        }
                    }
                }
                if (!added)
                {
                    for (int ni = 0; ni < 8 && !added; ni += 2)
                    {
                        var e = n.value.neighbours[ni];
                        if (e == null || e.IsEmpty)
                        {
                            // get left and right neighbours
                            var li = (ni + 1) % 8;
                            var ri = (ni - 1 + 8) % 8;
                            var l = n.value.neighbours[li];
                            var r = n.value.neighbours[ri];
                            // treat tiles off the edge as empty tiles
                            // if either both neighbours are empty or neither, this is an important corner
                            var rc = r == null || r.IsEmpty;
                            var lc = l == null || l.IsEmpty;
                            if (rc == lc)
                            {
                                nl.Add(n.value);
                                added = true;
                            }
                        }
                    }
                }
                if (added)
                {
                    for (int ni = 0; ni < 4; ni++)
                    {
                        var t = n.value.neighbours[ni * 2 + 1];
                        for (int s = 0; s < 100; s++)
                        {
                            if (t == null || t.IsEmpty)
                            {
                                break;
                            }
                            else if (nl.Contains(t))
                            {
                                nl.Connect(n.value, t);
                                break;
                            }
                            else
                            {
                                t = t.neighbours[ni * 2 + 1];
                            }
                        }
                    }
                }
            }
            if (nl.nodes.Count > 0)
            {
                var ci = new ColliderInfo(nl);
                colliders.Add(ci);
            }

        }
        Debug.Log(colliders.Count);
    }

    void GenerateDrawMesh(List<Tile> tileList, string key)
    {
        if (tileList.Count < 1)
        {
            Debug.LogError("TileList is empty!");
            return;
        }
        Mesh mesh = new Mesh();
        var basePos = tileList[0].position;

        var tileCount = tileList.Count;
        var vertices = new Vector3[tileCount * 4];
        var uv = new Vector2[tileCount * 4];
        var tris = new int[tileCount * 6];
        int vi = 0, ti = 0;
        for (int i = 0; i < tileCount; i++)
        {
            var tile = tileList[i];
            var pos = (Vector3)(tile.position);
            vertices[vi++] = pos;
            vertices[vi++] = pos + Vector3.up;
            vertices[vi++] = pos + new Vector3(1, 1, 0);
            vertices[vi++] = pos + Vector3.right;
            tris[ti++] = vi - 4;
            tris[ti++] = vi - 3;
            tris[ti++] = vi - 2;
            tris[ti++] = vi - 4;
            tris[ti++] = vi - 2;
            tris[ti++] = vi - 1;
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = tris;

        meshes[key].mesh = mesh;
    }
}
