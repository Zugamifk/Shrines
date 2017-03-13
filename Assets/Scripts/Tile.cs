using UnityEngine;
using System.Collections;

[System.Serializable]
public class Tile {
    public enum ENeighbour
    {
        BOTTOMLEFT,BOTTOM, BOTTOMRIGHT,
        RIGHT,
        TOPRIGHT,TOP,TOPLEFT,
        LEFT
    }

    public enum EdgeType {
        ENCLOSED,
        SIDE, CORNER, POINT,
        OPEN
    }

    public Vector2i position;
    public TileData data;
    public string status = "";

    public Tile[] neighbours = new Tile[8];
    public Tile left {
        get {return neighbours[7];}
        set { setNeighbour(7, value); }
    }
    public Tile right
    {
        get { return neighbours[3]; }
        set { setNeighbour(3, value); }
    }
    public Tile bottom
    {
        get { return neighbours[1]; }
        set { setNeighbour(1, value); }
    }
    public Tile top
    {
        get { return neighbours[5]; }
        set { setNeighbour(5, value); }
    }
    public Tile topleft
    {
        get { return neighbours[6]; }
        set { setNeighbour(6, value); }
    }
    public Tile topright
    {
        get { return neighbours[4]; }
        set { setNeighbour(4, value); }
    }
    public Tile bottomleft
    {
        get { return neighbours[0]; }
        set { setNeighbour(0, value); }
    }
    public Tile bottomright
    {
        get { return neighbours[2]; }
        set { setNeighbour(2, value); }
    }

    void setNeighbour(int i, Tile value)
    {
        if (neighbours[i] != null ^ value != null)
        {
            if (value != null)
            {
                neighboursCount++;
            }
            else
            {
                neighboursCount--;
            }
            if (neighboursCount < 0 || neighboursCount > 8)
            {
                Debug.Log("Bad count for tile neighbours! " + neighboursCount);
                neighboursCount = Mathf.Clamp(neighboursCount, 0, 8);
            }
        }
        neighbours[i] = value;
    }

    public bool IsMatchingLine(Tile with)
    {
        return position.x == with.position.x || position.y == with.position.y;
    }

    public int neighboursCount { get; private set; }

    public bool IsDiagonal(Tile t)
    {
        return bottomright == t || bottomleft == t || topright == t || topleft == t;
    }

    public void SetNeighbour(int horz, int vert, Tile t)
    {
        int i = -1;
        if (vert == -1)
        {
            if (horz == -1)
            {
                i = 0;
            }
            else if (horz == 0)
            {
                i = 1;
            }
            else if (horz == 1)
            {
                i = 2;
            }
        }
        else if (vert == 0)
        {
            if (horz == -1)
            {
                i = 7;
            }
            else if (horz == 0)
            {
                i = -1;
            }
            else if (horz == 1)
            {
                i = 3;
            }
        }
        else if (vert == 1)
        {
            if (horz == -1)
            {
                i = 6;
            }
            else if (horz == 0)
            {
                i = 5;
            }
            else if (horz == 1)
            {
                i = 4;
            }
        }
        if (i >= 0)
        {
            setNeighbour(i, t);
        }
    }

    public EdgeType GetEdge()
    {
        int i = 0;
        if (left != null && !left.data.collides) i++;
        if (right != null && !right.data.collides) i++;
        if (top != null && !top.data.collides) i++;
        if (bottom != null && !bottom.data.collides) i++;
        return (EdgeType)i;
    }

    public bool HasNeighbour(System.Func<Tile, bool> test, bool testDiagonals)
    {
        return test(left) || test(right) || test(top) || test(bottom) ||
            (testDiagonals && (test(topleft) || test(topright) || test(bottomleft) || test(bottomright)));
    }
}
