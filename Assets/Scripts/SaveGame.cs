using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveGame
{
    // An object instantiated from a prefab has "(Clone)" appended to its name.
    // Remove the suffix when saving object names to a file.
    public static readonly string CloneSuffix = "(Clone)";

    public DungeonParameters DungeonParameters;
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;

    // Store dungeon-generated items (start portal, keys, keypads, etc.)
    // in one gameobject. Store breadcrumbs in a separate gameobject.
    // This keeps all objects organized in the editor.
    public List<ItemSave> Items;
    public List<GameObjectSave> Breadcrumbs;

    public SaveGame(DungeonParameters dungeonParameters, GameObject player, GameObject itemsParent, GameObject breadcrumbsParent)
    {
        DungeonParameters = dungeonParameters;
        PlayerPosition = player.transform.position;
        PlayerRotation = player.transform.rotation;
        Items = new List<ItemSave>();
        Breadcrumbs = new List<GameObjectSave>();
        foreach (Transform child in itemsParent.transform)
        {
            string name = child.name;
            if (name.EndsWith(SaveGame.CloneSuffix))
            {
                name = name.Remove(name.Length - SaveGame.CloneSuffix.Length);
            }
            Items.Add(new ItemSave(name, child.position, child.rotation));
        }
        foreach (Transform child in breadcrumbsParent.transform)
        {
            Breadcrumbs.Add(new GameObjectSave(child.position, child.rotation));
        }
    }
}

/// <summary>
/// A version of GameObjectSave that includes an assignable name string.
/// This identifies what type of item is being saved or loaded.
/// </summary>
[Serializable]
public class ItemSave : GameObjectSave
{
    public string Name;

    public ItemSave(string name, Vector3 position, Quaternion rotation)
        : base(position, rotation)
    {
        Name = name;
    }
}

/// <summary>
/// For serialization in saving and loading.
/// This basic version with only transform parameters is used for breadcrumbs.
/// </summary>
[Serializable]
public class GameObjectSave
{
    public Vector3 Position;
    public Quaternion Rotation;

    public GameObjectSave(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}
