using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundTiling : Tiling
{
    public System.Func<float, float> depthLimit; 
    public virtual void Generate(Grid grid, TileSet tileSet, int x, int y, int w, int h)
    {
        if (depthLimit == null)
        {
            Debug.LogError("depthLimit is null! Can't produce GroundTiling!");
            return;
        }

        var count = grid.GetTiles(x, y, w, h, worker);
        for (int i = 0; i < count; i++)
        {
            TileData result = null;
            var inGround = depthLimit((float)worker[i].position.x/(float)w) > (float)worker[i].position.y/(float)h;
            if (inGround)
            {
                result = tileSet["ground"];
            }
            else
            {
                result = tileSet["air"];
            }

            if (result != null)
            {
                worker[i].data = result;
            }
            else
            {
                Debug.LogError("tileLookup up missing \'ground\' or \'air\' tiles!");
            }
        }

        for (int i = 30; i < 50; i++)
        {
            var j = (int)(h*depthLimit((float)i/(float)w))+4;
            j = Mathf.Clamp(j, 0, h-1);
            var tile = grid.GetTile(x + i, j);
            TileData result = null;
            result = tileSet["platform"];
            if (result != null)
            {
                tile.data = result;
            }
            else
            {
                Debug.LogError("tileLookup up missing \'platform\' tiles!");
            }
        }
    }
}
