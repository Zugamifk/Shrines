using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Grid {

    Tile[][] grid;
    int width, height;

    public void Generate(int width, int height)
    {
        grid = new Tile[width][];
        this.width = width;
        this.height = height;
        for(int x =0;x<width;x++) {
            grid[x] = new Tile[height];
            for (int y = 0; y < height; y++)
            {
                var tile = new Tile();
                tile.position = new Vector2i(x, y);
                grid[x][y] = tile;
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tile = grid[x][y];
                for (int nx = 0; nx < 3; nx++)
                {
                    for (int ny = 0; ny < 3; ny++)
                    {
                        var n = GetTile(x - 1 + nx, y - 1 + ny);
                        if (n != null)
                        {
                            tile.SetNeighbour(nx - 1, ny - 1, n);
                        }
                    }
                }
            }
        }
    }

    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return null;

        return grid[x][y];
    }

    public int GetTiles(int x, int y, int w, int h, List<Tile> values)
    {
        if (x >= width || y >= height)
        {
            return 0;
        }

        int count = 0;

        w = Mathf.Min(w, width - x, x + w);
        h = Mathf.Min(h, height - y, y + h);
        if (x < 0) x = 0;
        if (y < 0) y = 0;

        for (int xi = x; xi < x+w; xi++)
        {
            var col = grid[xi];
            for (int yi = y; yi < y+h; yi++)
            {
                var tile = col[yi];
                if (tile != null)
                {
                    values.Add(tile);
                    count++;
                }
                else
                {
                    Debug.LogError("Tile at (" + x + ", " + y + ") is null!");
                }
            }
        }

        return count;
    }

    public void Check(bool logStatus)
    {
        if (logStatus)
        {
            Debug.Log("Grid " + width + ", " + height);
            if (width > 0)
            {
                Debug.Log("Width: " + grid.GetLength(0) + " == " + width);
            }
        }
        int total = 0;
        int nullTiles = 0;
        int nulltileData = 0;
        int badPosition = 0;
        for (int x = 0; x < width; x++)
        {
            var col = grid[x];
            if (col == null)
            {
                if (logStatus)
                {
                    Debug.Log("Grid has null at column " + x);
                }
            }
            else
            {
                if (col.Length != height)
                {
                    if (logStatus)
                    {
                        Debug.Log("Height at row " + x + " does not match grid height " + height);
                    }
                }
                else
                {
                    for (int y = 0; y < height; y++)
                    {
                        var tile = grid[x][y];
                        total++;
                        if (tile == null)
                        {
                            nullTiles++;
                        }
                        else if (tile.data == null)
                        {
                            nulltileData++;
                        }
                        else if (tile.position.x != x || tile.position.y != y)
                        {
                            badPosition++;
                        }
                    }
                }
            }
        }
        if (logStatus)
        {
            Debug.LogFormat("Results:\n\tTotal tiles: {0}\n\tNull tiles: {1}\n\tNull TileData: {2}\n\tBad Positions: {3}", total, nullTiles, nulltileData, badPosition);
        }
    }
}
