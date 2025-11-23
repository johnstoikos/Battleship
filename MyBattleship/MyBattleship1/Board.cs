using Battleship;
using System;
using System.Collections.Generic;

namespace MyBattleship;

public class Board
{
    public char[,] Map;
    public List<Ship> Ships = new List<Ship>();
    int rows, cols;
    Random rnd = new Random();

    public Board(int r, int c)
    {
        rows = r; cols = c;
        Map = new char[rows, cols];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                Map[i, j] = '.';
    }

    public bool PlaceShip(Ship s)
    {
        for (int tries = 0; tries < 400; tries++)
        {
            int dir = rnd.Next(2);
            int r = rnd.Next(rows);
            int c = rnd.Next(cols);
            bool ok = true;
            List<Tuple<int, int>> tmp = new List<Tuple<int, int>>();

            for (int k = 0; k < s.Size; k++)
            {
                int rr = r + (dir == 1 ? k : 0);
                int cc = c + (dir == 0 ? k : 0);
                if (rr >= rows || cc >= cols || Map[rr, cc] != '.')
                {
                    ok = false; break;
                }
                tmp.Add(Tuple.Create(rr, cc));
            }

            if (!ok) continue;

            foreach (var p in tmp) Map[p.Item1, p.Item2] = 'S';
            s.Cells.AddRange(tmp);
            Ships.Add(s);
            return true;
        }
        return false;
    }

    public bool Shoot(int r, int c, out Ship? hitShip)
    {
        foreach (var s in Ships)
        {
            foreach (var pos in s.Cells)
            {
                if (pos.Item1 == r && pos.Item2 == c)
                {
                    s.Hits.Add(pos);
                    Map[r, c] = 'X';
                    hitShip = s;
                    return true;
                }
            }
        }
        Map[r, c] = '-';
        hitShip = null;
        return false;
    }

    public bool AllSunk()
    {
        foreach (var s in Ships)
            if (!s.IsSunk()) return false;
        return true;
    }

    public Ship? GetShipAt(int r, int c)
    {
        foreach (var s in Ships)
            foreach (var pos in s.Cells)
                if (pos.Item1 == r && pos.Item2 == c) return s;
        return null;
    }
}
