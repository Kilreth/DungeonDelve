using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonParameters
{
    public int Rows;
    public int Cols;
    public int TotalKeys;

    // Smaller dungeon sizes (eg. 30x40) should have smaller max room sizes
    public int MinRoomHeight;
    public int MinRoomWidth;
    public int MaxRoomHeight;
    public int MaxRoomWidth;

    // The ratio of occupied room space (walls + walkable space)
    // against all space in the dungeon.
    //
    // The larger the rooms, the more wasted space between them,
    // and the less efficiently the dungeon will fill up.
    // With rooms of 9x9 max size, expect no difference past 0.5.
    //
    // Use a low ratio to create sparse dungeons.

    public double TargetRoomToDungeonRatio;


    // The ratio of door tiles to all wall tiles of a room.
    // A random -1, +0, or +1 doors is applied to each room later.
    //
    // If the ratio is set to 0, the generator will create doors
    // when making the connected graph anyway.
    // Also note that doors will not spawn next to each other, so
    // in practice the ratio will not exceed 0.5.
    //
    // Fewer doors means fewer choices for the player to make.
    // Too many doors make it too easy to get around the dungeon.
    //
    // A value around 0.1 strikes a fair balance.

    public double DoorsToWallRatio;


    // When generating corridors, the chance at each tile that
    // the corridor will turn.
    //
    // With a value of 0, corridors only turn upon hitting a wall.
    // With a value of 1, corridors constantly wind as they get anywhere.
    //
    // A low value of 0.2 works well.

    public double CorridorTurnChance;
}
