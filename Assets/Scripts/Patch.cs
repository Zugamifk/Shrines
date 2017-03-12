using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Patch
{

    public int width;
    public int height;

    public List<Tile> currentTiles;
    public List<Tile> drawTiles;
    Mesh drawMesh;
    public Graph<Tile> edgeGraph;
    public List<Graph<Tile>> colliderGraphs;

    public Patch(int w, int h)
    {
        width = w;
        height = h;
    }

    public void SetDrawMesh(MeshFilter filter)
    {
        filter.mesh = drawMesh;
    }

    public void Generate(List<Tile> tileList, TileSet tileSet)
    {
        currentTiles = new List<Tile>(tileList);
        drawTiles = new List<Tile>(tileList);
        GenerateDrawMesh(tileList);
        BuildEdgeGraph();
    }

    public void Paint(TilePainting painting)
    {
        var uv = drawMesh.uv;
        painting.Paint(currentTiles, uv);
        drawMesh.uv = uv;
    }

    void BuildEdgeGraph()
    {
        edgeGraph = new Graph<Tile>(drawTiles);
        edgeGraph.Filter(t => t.value.data.collides && t.value.HasNeighbour(n => n != null && !n.data.collides, true));
        edgeGraph.Connect((t, l) =>
        {
            foreach (var n in t.neighbours)
            {
                if (n != null && (!t.IsDiagonal(n)) && edgeGraph.Contains(n))
                {
                    l.Add(n);
                }
            }
        });
        edgeGraph.BFS();
        colliderGraphs = new List<Graph<Tile>>();
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
                    if (e == null || !e.data.collides)
                    {
                        // get left and right neighbours
                        var li = (ni + 2) % 8;
                        var ri = (ni - 2 + 8) % 8;
                        var l = n.value.neighbours[li];
                        var r = n.value.neighbours[ri];
                        // treat tiles off the edge as empty tiles
                        // if either both neighbours are empty or neither, this is an important corner
                        var rc = r == null || !r.data.collides;
                        var lc = l == null || !l.data.collides;
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
                        if (e == null || !e.data.collides)
                        {
                            // get left and right neighbours
                            var li = (ni + 1) % 8;
                            var ri = (ni - 1 + 8) % 8;
                            var l = n.value.neighbours[li];
                            var r = n.value.neighbours[ri];
                            // treat tiles off the edge as empty tiles
                            // if either both neighbours are empty or neither, this is an important corner
                            var rc = r == null || !r.data.collides;
                            var lc = l == null || !l.data.collides;
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
                            if (t == null || !t.data.collides)
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
                colliderGraphs.Add(nl);
            }

        }
        Debug.Log(colliderGraphs.Count);
    }

    void GenerateDrawMesh(List<Tile> tileList)
    {
        if (tileList.Count < 1)
        {
            Debug.LogError("TileList is empty!");
            return;
        }
        Mesh mesh = new Mesh();
        var basePos = tileList[0].position;
        drawTiles.RemoveAll(t => t.data == null || !t.data.collides);


        if (drawTiles.Count < 1)
        {
            Debug.LogError("Empty patch!");
            return;
        }

        var tileCount = drawTiles.Count;
        var vertices = new Vector3[tileCount * 4];
        var uv = new Vector2[tileCount * 4];
        var tris = new int[tileCount * 6];
        int vi = 0, ti = 0;
        for (int i = 0; i < tileCount; i++)
        {
            var tile = drawTiles[i];
            var pos = (Vector3)(tile.position - basePos);
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

        drawMesh = mesh;
    }

    void GenerateColliderMeshes()
    {

    }

}
