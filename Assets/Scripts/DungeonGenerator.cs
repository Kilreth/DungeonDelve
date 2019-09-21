using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Dungeon_Generator
{
    public class DungeonGenerator
    {
        public Dungeon Dungeon { get; }
        public static Random Rng { get; }

        /// <summary>
        /// Choose a random start room.
        /// Choose the farthest room from that as the goal room.
        /// Choose a room with medium distance to place the key in.
        /// 
        /// Rooms are ordered by BFS so the heuristic is number of rooms+paths crossed.
        /// </summary>
        /// <param name="dungeon"></param>
        public void GenerateStairsAndKey(Dungeon dungeon)
        {
            // Although very unlikely, it is possible not all rooms at this point are connected
            // Choose a random room that belongs to the connected dungeon
            Room startRoom;
            List<Room> roomsFromStart;
            do
            {
                startRoom = dungeon.GetRandomRoom();
                roomsFromStart = dungeon.FindConnectedRooms(startRoom);
            }
            while (roomsFromStart.Count * 2 < dungeon.Rooms.Count);
            int totalRooms = roomsFromStart.Count;

            // DEBUG: Draw BFS order on screen
            for (int i = 0; i < totalRooms; ++i)
            {
                roomsFromStart[i].GetRandomTile(dungeon).Text = i.ToString();
            }

            // The goal room is the farthest room encountered in the breadth-first search
            // The room with the key is somewhere in the middle
            Room goalRoom = roomsFromStart[totalRooms - 1];
            Room keyRoom  = roomsFromStart[Rng.Next((int)(totalRooms * 0.4), (int)(totalRooms * 0.6))];

            startRoom.GetRandomTile(dungeon).Space = Space.StairsUp;
            goalRoom.GetRandomTile(dungeon).Space = Space.StairsDown;
            keyRoom.GetRandomTile(dungeon).Space = Space.Key;
        }

        /// <summary>
        /// Prior corridor generation simply looks for other rooms and paths to connect to.
        /// There is no guarantee that every room can be reached from a given room.
        /// Here we attempt to find rooms unconnected to the largest graph, and connect
        /// them by *spawning a new door and attempting to generate a new corridor*.
        /// 
        /// Because there may be limited space for corridors, we must not waste it, so we discard
        /// any new doors+corridors that make a duplicate connection.
        /// 
        /// There may be unconnected rooms even after this, but they are very rare.
        /// Moreover, because stair and key generation use the connected graph,
        /// any isolated rooms are harmless.
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="chanceToTurn"></param>
        public void MakeDungeonACompleteGraph(Dungeon dungeon, double chanceToTurn)
        {
            List<Room> unconnectedRooms = dungeon.FindUnconnectedRooms(dungeon.GetRandomRoom());
            int tries = 0;
            while (unconnectedRooms.Count > 0 && tries < dungeon.Rooms.Count)
            {
                foreach (Room r in unconnectedRooms)
                {
                    r.FlagDebug();
                }
                Room room = unconnectedRooms[Rng.Next(0, unconnectedRooms.Count)];
                Tile door = room.GenerateDoor(dungeon);
                if (door != null)
                {
                    door.Debug = true;

                    // Avoid wasting doors on paths that don't connect anywhere new
                    bool allowConnectionToConnectedArea = false;
                    GenerateCorridor(dungeon, door, chanceToTurn, allowConnectionToConnectedArea);
                }
                else
                {
                    return;
                }

                unconnectedRooms = dungeon.FindUnconnectedRooms(dungeon.GetRandomRoom());
                ++tries;
            }
        }

        /// <summary>
        /// Link all adjacent walkable areas to the area a given tile belongs to.
        /// For example, link the end of a path to the room belonging to a door.
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="tile"></param>
        public void ConnectAreas(Dungeon dungeon, Tile tile)
        {
            foreach (Tile adj in dungeon.GetAdjacentTiles(tile))
            {
                if (Tile.IsWalkable(adj.Space))
                {
                    tile.Area.ConnectTo(adj.Area);
                }
            }
        }

        /// <summary>
        /// Commits a stack of wrapped corridor tiles to the dungeon.
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="path"></param>
        public void CarveCorridor(Dungeon dungeon, Stack<CorridorTile> path)
        {
            // Unwrap the tiles in the stack to populate a set

            HashSet<Tile> tiles = new HashSet<Tile>();
            foreach (CorridorTile wrappedTile in path)
            {
                tiles.Add(wrappedTile.Tile);
            }

            // Get an existing Path object if there is one; otherwise make a new object

            Area area;
            List<Tile> adjacentPaths = dungeon.GetAdjacentTilesOfType(path.Peek().Tile, Space.Path);
            if (adjacentPaths.Count > 0)
            {
                area = adjacentPaths[0].Area;
            }
            else
            {
                area = new Path();
                area.InitializeArea();
            }

            // Connect the *end* of the path to any adjacent Area objects

            Tile end = path.Peek().Tile;
            end.Area = area;
            ConnectAreas(dungeon, end);

            // Finally begin carving the corridor

            CorridorTile head = null;
            while (path.Count > 0)
            {
                head = path.Pop();
                tiles.Remove(head.Tile);
                List<Tile> adjacents = dungeon.GetAdjacentTiles(head.Tile, head.From.Tile);
                foreach (Tile adjacent in adjacents)
                {
                    if (tiles.Contains(adjacent))
                    {
                        // Loops exists, trim it

                        while (path.Peek().Tile != adjacent)
                        {
                            Tile remove = path.Pop().Tile;
                            tiles.Remove(remove);
                        }
                    }
                }
                head.Tile.Space = Space.Path;
                head.Tile.Area = area;
            }

            // Connect the *start* of the path to the room it came from

            ConnectAreas(dungeon, head.Tile);
        }

        /// <summary>
        /// Perform a flood-fill search from a door to another room's door or a path.
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="door"></param>
        /// <param name="chanceToTurn"></param>
        public void CorridorWalk(Dungeon dungeon, Tile door, double chanceToTurn, bool allowConnectionToConnectedArea)
        {
            bool DoorLeadsToOtherRoom(List<Tile> doors)
            {
                foreach (Tile d in doors)
                {
                    if (door.Area != d.Area)
                    {
                        return true;
                    }
                }
                return false;
            }

            HashSet<CorridorTile> visited = new HashSet<CorridorTile>();
            Stack<CorridorTile> path = new Stack<CorridorTile>();

            Tile firstTile = dungeon.GetTileByDirection(door);
            CorridorTile start = new CorridorTile(door, null, door.Direction);
            CorridorTile head = new CorridorTile(firstTile, start, door.Direction);
            path.Push(head);
            visited.Add(head);
            while (path.Count > 0)
            {
                head = path.Peek();

                // Can we carve this tile?

                // If a door is on the head of the stack, it belongs to the room we came from.
                // (If we had found a door to another room, we would already have exited the while loop)
                // Treat these doors as walls.

                if (head.Space == Space.Wall || head.Space == Space.Granite || head.Space == Space.Door)
                {
                    path.Pop();
                    continue;
                }

                // Have we found any doors?
                // If all adjacent doors lead to the room we came from, carry on

                if (dungeon.IsTileAdjacentTo(head.Tile, Space.Door, head.From.Tile)
                        && DoorLeadsToOtherRoom(dungeon.GetAdjacentTilesOfType(head.Tile, Space.Door, head.From.Tile)))
                {
                    CarveCorridor(dungeon, path);
                    return;
                }

                // Have we found an existing path? Connect to it and end
                // If we are not allowed to connect to it, then we must skirt around it

                if (dungeon.IsTileAdjacentTo(head.Tile, Space.Path, head.From.Tile))
                {
                    if (allowConnectionToConnectedArea || RoomUnconnectedToAdjacentArea(door.Area,
                        dungeon.GetAdjacentTilesOfType(head.Tile, Space.Path, head.From.Tile)))
                    {
                        CarveCorridor(dungeon, path);
                        return;
                    }
                    else
                    {
                        // Do not allow our path to touch existing paths. Treat this tile as non-carvable

                        path.Pop();
                        continue;
                    }
                }

                // Decide where to go next, or step back one tile if all paths have been explored

                if (head.DirectionsToTry.Count > 0)
                {
                    CorridorTile next;
                    do
                    {
                        next = head.ChooseNextTile(dungeon, chanceToTurn);
                    } while (visited.Contains(next) && head.DirectionsToTry.Count > 0);
                    if (visited.Contains(next))
                    {
                        path.Pop();
                    }
                    else
                    {
                        path.Push(next);
                        visited.Add(next);
                    }
                }
                else
                {
                    path.Pop();
                }
            }

            // There are no doors or paths to connect to, so erase this door
            EraseDoor(door);
        }

        /// <summary>
        /// If a door opens into the wall of another room, carving straight ahead is guaranteed
        /// to open into that room. Make a door at the other end.
        /// It's possible that if the door opens into the corner of another room, our corridor
        /// opens into another corridor first. This is fine, just don't make the last tile a door.
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="door"></param>
        /// <param name="startOfPath"></param>
        public void CorridorThroughRoomWall(Dungeon dungeon, Tile door, Tile startOfPath, bool allowConnectionToConnectedArea)
        {
            // Prepare a stack of path tiles so we can call our general method to carve the corridor

            Stack<CorridorTile> path = new Stack<CorridorTile>();
            CorridorTile wrappedDoor = new CorridorTile(door, null, door.Direction);
            CorridorTile head = new CorridorTile(startOfPath, wrappedDoor, door.Direction);
            path.Push(head);
            while (!dungeon.IsTileAdjacentTo(head.Tile, Space.WALKABLE, head.From.Tile))
            {
                Tile nextTile = dungeon.GetTileByDirection(head.Tile, door.Direction);
                head = new CorridorTile(nextTile, head, door.Direction);
                path.Push(head);
            }

            if (!allowConnectionToConnectedArea && !RoomUnconnectedToAdjacentArea(door.Area,
                dungeon.GetAdjacentTilesOfType(head.Tile, Space.WALKABLE, head.From.Tile)))
            {
                EraseDoor(door);
                return;
            }

            // If the last tile is directly touching another room's interior, make this tile a door
            // As a wall tile originally, the door tile is already facing out from its room

            Tile otherDoor = path.Peek().Tile;
            if (dungeon.IsTileAdjacentTo(otherDoor, Space.Room))
            {
                path.Pop();
                Room otherRoom = (Room)otherDoor.Area;
                otherRoom.SetTileAsDoor(otherDoor);
            }

            // If we didn't pop our only path tile off of the stack, carve the remaining corridor

            if (path.Count > 0)
            {
                CarveCorridor(dungeon, path);
            }
            else
            {
                ConnectAreas(dungeon, door);
            }
        }

        /// <summary>
        /// Account for all the cases a door may open into.
        ///   - Another room. Carve no corridor, just link areas
        ///   - A wall of another room. Carve a corridor straight ahead until a walkable space is found
        ///   - Solid rock. Call the flood-fill search to find a door or path for a corridor to connect to
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="door"></param>
        /// <param name="chanceToTurn"></param>
        /// <param name="allowConnectionToConnectedArea"></param>
        public void GenerateCorridor(Dungeon dungeon, Tile door, double chanceToTurn, bool allowConnectionToConnectedArea)
        {
            Tile startOfPath = dungeon.GetTileByDirection(door, door.Direction);

            // If door is already connected to a path or another door, there is nothing to do
            // (The initial set of doors touch no other doors, so if a door is adjacent,
            // it was spawned while carving a path straight ahead.)

            if (dungeon.IsTileAdjacentTo(door, Space.WALKABLE, dungeon.GetTileByDirection(door, Tile.Invert(door.Direction))))
            {
                if (allowConnectionToConnectedArea || !door.Area.To.Contains(startOfPath.Area))
                {
                    ConnectAreas(dungeon, door);
                }
                else
                {
                    EraseDoor(door);
                }
                return;
            }

            // If the door has opened into a wall, carve straight ahead until the other room can be entered
            // Then exit this method call

            if (startOfPath.Space == Space.Wall)
            {
                CorridorThroughRoomWall(dungeon, door, startOfPath, allowConnectionToConnectedArea);
                return;
            }

            // If there is solid stone ahead, start a path

            CorridorWalk(dungeon, door, chanceToTurn, allowConnectionToConnectedArea);
        }

        private bool RoomUnconnectedToAdjacentArea(Area area, List<Tile> adjacents)
        {
            foreach (Tile adj in adjacents)
            {
                if (!area.To.Contains(adj.Area))
                {
                    return true;
                }
            }
            return false;
        }

        private void EraseDoor(Tile door)
        {
            Room room = (Room)door.Area;
            room.Doors.Remove(door);
            door.Space = Space.Wall;
        }

        public void GenerateCorridors(Dungeon dungeon, double chanceToTurn)
        {
            foreach (Room room in dungeon.Rooms)
            {
                // If no corridor can be formed from a door, the door is removed
                // So a room's list of doors may shrink as we iterate over it

                bool allowConnectionToConnectedArea = true;
                for (int i = room.Doors.Count - 1; i >= 0; --i)
                {
                    GenerateCorridor(dungeon, room.Doors[i], chanceToTurn, allowConnectionToConnectedArea);
                }
            }
        }

        public void GenerateDoors(Dungeon dungeon, double doorToWallRatio)
        {
            foreach (Room room in dungeon.Rooms)
            {
                room.GenerateDoors(dungeon, doorToWallRatio);
            }
        }

        /// <summary>
        /// Randomly fill an empty dungeon with rooms until a space ratio is reached,
        /// or until adding another room is unsuccessful after a number of tries.
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="roomToDungeonRatio"></param>
        /// <param name="minRoomHeight"></param>
        /// <param name="minRoomWidth"></param>
        /// <param name="maxRoomHeight"></param>
        /// <param name="maxRoomWidth"></param>
        public void GenerateRooms(Dungeon dungeon, double roomToDungeonRatio,
                                     int minRoomHeight, int minRoomWidth, int maxRoomHeight, int maxRoomWidth)
        {
            // Calculate how many room tiles we have

            int totalTiles = dungeon.Height * dungeon.Width;
            int remainingRoomTiles = (int) (totalTiles * roomToDungeonRatio);
            int minRoomSize = minRoomHeight * minRoomWidth;

            while (remainingRoomTiles > minRoomSize * 3)
            {
                // Generate random dimensions for a room

                int roomHeight, roomWidth;
                do
                {
                    roomHeight = Rng.Next(minRoomHeight, maxRoomHeight + 1);
                    roomWidth  = Rng.Next(minRoomWidth,  maxRoomWidth  + 1);
                } while (roomHeight * roomWidth > remainingRoomTiles);

                // Create the room and put it in a random place

                Room room;
                int row, col;
                int dungeonEdge = 2;
                int attempts = 0;
                do
                {
                    row = Rng.Next(dungeonEdge, dungeon.Height - roomHeight - dungeonEdge + 1);
                    col = Rng.Next(dungeonEdge, dungeon.Width - roomWidth - dungeonEdge + 1);
                    room = new Room(row, col, roomHeight, roomWidth);
                    ++attempts;
                } while (!room.CanRoomFit(dungeon) && attempts != 100);
                if (attempts == 100)
                {
                    break;
                }

                room.InitializeArea();
                dungeon.CarveRoom(room);
                remainingRoomTiles -= roomHeight * roomWidth;
            }
        }

        public DungeonGenerator(int height, int width)
        {
            Dungeon = new Dungeon(height, width);
            GenerateRooms(Dungeon, 0.9, 3, 3, 9, 9);
            GenerateDoors(Dungeon, 0.1);
            GenerateCorridors(Dungeon, 0.2);
            //MakeDungeonACompleteGraph(Dungeon, 0.2);
            //GenerateStairsAndKey(Dungeon);
        }

        static DungeonGenerator()
        {
            Rng = new Random();
        }
    }
}
