using System;
using System.Collections.Generic;

namespace Battleship
{
    public class Ship
    {
        public string Name;
        public int Size;
        public List<Tuple<int, int>> Cells = new List<Tuple<int, int>>();
        public HashSet<Tuple<int, int>> Hits = new HashSet<Tuple<int, int>>();
        public string Code;

        public Ship(string name, int size)
        {
            Name = name;
            Size = size;
            if (name.StartsWith("Αε")) Code = "ΑΕ";
            else if (name.StartsWith("Αν")) Code = "ΑΝ";
            else if (name.StartsWith("Πολ")) Code = "ΠΟ";
            else if (name.StartsWith("Υπ")) Code = "ΥΠ";
            else Code = "Π?";
        }

        public bool IsSunk() => Hits.Count == Size;
    }
}
