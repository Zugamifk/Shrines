using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tiling {

    List<Tile> worker = new List<Tile>();
    public void Generate(Grid grid, Dictionary<string, TileData> tileLookup, int x, int y, int w, int h)
    {
        var count = grid.GetTiles(x, y, w, h, worker);
        for (int i = 0; i < count; i++)
        {
            TileData result = null;
            if(Random.value > 0.5f ) {
                tileLookup.TryGetValue("ground", out result);
            }  else {
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
