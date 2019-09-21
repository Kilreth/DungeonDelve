using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Generator
{
    /// <summary>
    /// An environmental tile. A 2D array of Tiles forms a dungeon.
    /// </summary>
    public class Tile
    {
        // Debug flag draws the tile in a red border
        public bool Debug { get; set; }
        public string Text { get; set; }

        public int Row { get; }
        public int Col { get; }
        public Space Space { get; set; }
        public Direction Direction { get; set; }
        public Area Area { get; set; }

        public static bool IsWalkable(Tile tile)
        {
            return IsWalkable(tile.Space);
        }

        public static bool IsWalkable(Space space)
        {
            return space == Space.Path
                || space == Space.Room
                || space == Space.Door;
        }

        public static Direction Invert(Direction direction)
        {
            if (direction == Direction.None)
                throw new ArgumentNullException("direction", "Direction of tile not set");
            if (direction == Direction.Up)
                return Direction.Down;
            else if (direction == Direction.Down)
                return Direction.Up;
            else if (direction == Direction.Left)
                return Direction.Right;
            else if (direction == Direction.Right)
                return Direction.Left;
            throw new ArgumentNullException("direction", "Invert undefined for this direction");
        }

        /// <summary>
        /// Test for equality based on row and column coordinates.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Tile other = (Tile) obj;
                return (Row == other.Row) && (Col == other.Col);
            }
        }

        /// <summary>
        /// As with equality, only the row and column is considered for the hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (Row << 2) ^ Col;
        }

        public Tile(int row, int col, Space space)
        {
            Row = row;
            Col = col;
            Space = space;
        }
    }

    /// <summary>
    /// Up:    Decreasing rows
    /// Down:  Increasing rows
    /// Left:  Decreasing columns
    /// Right: Increasing columns
    /// </summary>
    public enum Direction { None, Up, Down, Left, Right }

    /// <summary>
    /// Granite: Impervious edge of the dungeon
    /// Rock:    Solid tile with which to fill the dungeon
    /// Room:    Vacant tiles in a room
    /// Wall:    The rock surrounding Room tiles
    /// Path:    Vacant tiles that make corridors connecting doors
    /// Door:    Vacant tiles connecting rooms and paths
    /// </summary>
    public enum Space { Granite, Rock, Room, Wall, Path, Door, StairsUp, StairsDown, Key, WALKABLE }
}
