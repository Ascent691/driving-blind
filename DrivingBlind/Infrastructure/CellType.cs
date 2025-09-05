using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public enum CellKind
    {
        Empty = 0,
        Wall = 1,
        Hazard = 2,
        InterestingStart = 3,
        InterestingFinish = 4,
    }

    public struct CellType : IEquatable<CellType>
    {
        public static CellType Empty = new CellType(CellKind.Empty, '.');
        public static CellType Wall = new CellType(CellKind.Wall, '#');
        public static CellType Hazard = new CellType(CellKind.Hazard, '*');

        public static CellType InterestingStart(char identifier)
        {
            return new CellType(CellKind.InterestingStart, identifier);
        }

        public static CellType InterestingFinish(char identifier)
        {
            return new CellType(CellKind.InterestingFinish, identifier);
        }

        public char Identifier { get; init; }
        public CellKind Kind { get; init; }

        public CellType(CellKind kind, char identifier) : this()
        {
            Kind = kind;
            Identifier = identifier;
        }

        

        public bool IsInteresting => Kind == CellKind.InterestingStart || Kind == CellKind.InterestingFinish;

        public override bool Equals(object? obj)
        {
            return obj is CellType type && Equals(type);
        }

        public bool Equals(CellType other)
        {
            return Kind == other.Kind && (!IsInteresting || Identifier == other.Identifier);
        }

        public override int GetHashCode()
        {
            return IsInteresting ? ((Identifier >> 8) | (int)Kind) : (int)Kind;
        }

        public static bool operator ==(CellType l, CellType r)
        {
            return l.Kind == r.Kind && (!l.IsInteresting || l.Identifier == r.Identifier);
        }

        public static bool operator !=(CellType l, CellType r)
        {
            return !(l.Kind == r.Kind && (!l.IsInteresting || l.Identifier == r.Identifier));
        }

        public static bool operator ==(CellType l, CellKind r)
        {
            return l.Kind == r;
        }

        public static bool operator !=(CellType l, CellKind r)
        {
            return l.Kind != r;
        }

        public static bool operator ==(CellKind l, CellType r)
        {
            return l == r.Kind;
        }

        public static bool operator !=(CellKind l, CellType r)
        {
            return l != r.Kind;
        }
    }
}
