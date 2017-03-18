using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridView : MonoBehaviour {

    public TileSet tileSet;
    public TilePaintingPalette graphics;
    public PatchView patchView;
    public AnimationCurve surfaceCurve;

    public Dictionary<Vector2i, Patch> patches = new Dictionary<Vector2i, Patch>();
    public Grid grid;
    World world;

    [System.NonSerialized]
    Vector2i patchSize = new Vector2i(100, 100);

    void OnEnable()
    {

    }

    public void Generate(int x, int y)
    {
        var patch = new Patch(patchSize.x, patchSize.y);

        var tiles = new List<Tile>();
        grid.GetTiles(x, y, patchSize.x, patchSize.y, tiles);
        patch.Generate(tiles, tileSet);

        patches[new Vector2i(x, y)] = patch;

        var painting = new TilePainting();
        painting.palette = graphics;
        patch.PaintAll(painting);

        patchView.SetPatch(patch);
        patchView.Enable();
    }

    public void TestWorld()
    {
        if (world == null)
        {
            world = new World("Aebeltoft");
        }
        world.Generate();
        grid = world.grid;
        var tiling = new GroundTiling();
        tiling.depthLimit = surfaceCurve.Evaluate;
        tiling.Generate(grid, tileSet, 0, 0, 100, 100);
        Debug.Log(world.ToString());
        world.grid.Check(true);
    }
}
