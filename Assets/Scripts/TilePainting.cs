using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TilePainting {

    [SerializeField]
    public TilePaintingPalette palette;

    public void Paint(List<Tile> tiles, Vector2[] uvs)
    {
        int ui = 0;
        foreach (var tile in tiles)
        {
            if (tile.IsEmpty) continue;
            var seek = tile;
            bool found = false;
            TileGraphicData tg = null;
            if (tile.data.collisions == TileData.CollisionType.Platform)
            {
                tg = palette["platform"];
            }
            else
            {
                tg = palette["ground"];
            }

            if (tg != null)
            {
                var shapes = tg.shapes;
                for (int d = 0; d < shapes.Count; d++)
                {
                    seek = seek.top;
                    if (seek == null || seek.IsEmpty)
                    {
                        found = true;
                        var sprite = shapes[d].sprites[0];
                        var rect = sprite.uv;
                        uvs[ui++] = rect[2];
                        uvs[ui++] = rect[0];
                        uvs[ui++] = rect[1];
                        uvs[ui++] = rect[3];
                        break;
                    }
                }
                if (!found)
                {
                    var sprite = shapes[shapes.Count - 1].sprites[0];
                    var rect = sprite.uv;
                    uvs[ui++] = rect[2];
                    uvs[ui++] = rect[0];
                    uvs[ui++] = rect[1];
                    uvs[ui++] = rect[3];
                }
            }
            else
            {
                Debug.LogError("TileGraphicData is null at " + tile.position);
            }
        }
    }
}
