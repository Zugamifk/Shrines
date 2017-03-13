using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridView : MonoBehaviour {

    public TileSet tileSet;
    public TileGraphicData graphics;
    public PatchView patchView;
    public AnimationCurve surfaceCurve;

    public Dictionary<Vector2i, Patch> patches = new Dictionary<Vector2i, Patch>();
    public Grid grid;
    World world;

    [System.NonSerialized]
    Vector2i patchSize = new Vector2i(100, 100);

    public void Generate(int x, int y)
    {
        var patch = new Patch(patchSize.x, patchSize.y);

        var tiles = new List<Tile>();
        grid.GetTiles(x, y, patchSize.x, patchSize.y, tiles);
        patch.Generate(tiles, tileSet);

        patches[new Vector2i(x, y)] = patch;

        var painting = new TilePainting();
        painting.tileGraphicData = graphics;
        painting.Init();
        patch.Paint(painting);

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
        tiling.depthLimit = (x,y) => surfaceCurve.Evaluate((float)x/100) > (float)y/100f;
        tiling.Generate(grid, new Dictionary<string, TileData>() { {"ground", tileSet.tiles[0].tile}, {"air", tileSet.tiles[1].tile} }, 0, 0, 100, 100);
        Debug.Log(world.ToString());
        world.grid.Check(true);
    }
}
