using System;
using UnityEngine;

[Serializable]
public struct Point
{
    public int x;
    public int y;

    public Point(int xx, int yy)
    {
        x = xx;
        y = yy;
    }

    public static Point operator -(Point a, Point b)
    => new Point(a.x - b.x, a.y - b.y);

    public static Point operator +(Point a, Point b)
    => new Point(a.x + b.x, a.y + b.y);
}

public enum TileType
{
    BonBon,
    Brownie,
    Chocolate,
    Cookie,
    Special_Munchkin,
    Count
}

public enum moveType
{
    None,
    Move
}

[Serializable]
public enum ListInfo
{
    Tile,
    Empty
}


public class GlobalData
{

    public static Sprite[] blockSprites = Resources.LoadAll<Sprite>("BlockImage");
}