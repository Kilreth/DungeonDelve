using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Generator
{
    /// <summary>
    /// A CorridorTile wraps around a Tile and is used in iterative path finding.
    /// </summary>
    public class CorridorTile
    {
        public Tile Tile { get; }
        public CorridorTile From { get; }
        public Direction PreviousDirection { get; }
        public List<Direction> DirectionsToTry { get; }
        public Space Space { get { return Tile.Space; } }

        public CorridorTile ChooseNextTile(Dungeon dungeon, double chanceToTurn)
        {
            Direction nextDirection = ChooseNextDirection(chanceToTurn);
            Tile nextTile = dungeon.GetTileByDirection(Tile, nextDirection);
            return new CorridorTile(nextTile, this, nextDirection);
        }

        public Direction ChooseNextDirection(double chanceToTurn)
        {
            if (DirectionsToTry.Count == 0)
            {
                throw new InvalidOperationException("All directions already explored");
            }
            if (DirectionsToTry.Contains(PreviousDirection)
                && DungeonGenerator.Rng.NextDouble() > chanceToTurn)
            {
                DirectionsToTry.Remove(PreviousDirection);
                return PreviousDirection;
            }
            int index;
            do
            {
                index = DungeonGenerator.Rng.Next(0, DirectionsToTry.Count);
            } while (DirectionsToTry[index] == PreviousDirection && DirectionsToTry.Count > 1);
            Direction direction = DirectionsToTry[index];
            DirectionsToTry.RemoveAt(index);
            return direction;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                CorridorTile other = (CorridorTile) obj;
                return Tile == other.Tile;
            }
        }

        public override int GetHashCode()
        {
            return Tile.GetHashCode();
        }

        public CorridorTile(Tile tile, CorridorTile from, Direction dir)
        {
            Tile = tile;
            From = from;
            PreviousDirection = dir;

            DirectionsToTry = new List<Direction>();
            DirectionsToTry.Add(Direction.Up);
            DirectionsToTry.Add(Direction.Down);
            DirectionsToTry.Add(Direction.Left);
            DirectionsToTry.Add(Direction.Right);
            DirectionsToTry.Remove(Tile.Invert(dir));
        }
    }
}
