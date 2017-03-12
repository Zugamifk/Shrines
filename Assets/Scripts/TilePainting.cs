using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TilePainting {

    [SerializeField]
    public TileGraphicData tileGraphicData;

    List<TileGraphicData.SpriteShape> shapes = new List<TileGraphicData.SpriteShape>();

    public void Init()
    {
        shapes.AddRange(tileGraphicData.shapes);
        shapes.Sort((a, b) => a.depth - b.depth);
    }

    public void Paint(List<Tile> tiles, Vector2[] uvs)
    {
        int ui = 0;
        foreach (var tile in tiles)
        {
            if (!tile.data.collides) continue;
            var seek = tile;
            bool found = false;
            for (int d = 0; d < shapes.Count; d++)
            {
                seek = seek.top;
                if (seek == null || !seek.data.collides)
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
                var sprite = shapes[shapes.Count-1].sprites[0];
                var rect = sprite.uv;
                uvs[ui++] = rect[2];
                uvs[ui++] = rect[0];
                uvs[ui++] = rect[1];
                uvs[ui++] = rect[3];
            }
        }
        Debug.Log(ui);
    }
}
