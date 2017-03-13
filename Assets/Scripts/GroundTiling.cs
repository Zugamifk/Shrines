using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundTiling : Tiling
{
    public System.Func<int, int, bool> depthLimit; 
    public virtual void Generate(Grid grid, Dictionary<string, TileData> tileLookup, int x, int y, int w, int h)
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
            var inGround = depthLimit.Invoke(worker[i].position.x, worker[i].position.y);                 
            if (inGround)
            {
                tileLookup.TryGetValue("ground", out result);
            }
            else
            {
                tileLookup.TryGetValue("air", out result);
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
    }
}
