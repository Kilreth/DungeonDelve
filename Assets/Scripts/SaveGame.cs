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
    public int KeysCollected;
    public float TimeElapsed;

    // Store dungeon-generated items (start portal, keys, keypads, etc.)
    // in one gameobject. Store breadcrumbs in a separate gameobject.
    // This keeps all objects organized in the editor.
    public List<ItemSave> Items;
    public List<ItemSave> Breadcrumbs;

    public Dictionary<string, string> BreadcrumbNameToChar = new Dictionary<string, string>()
    {
        { "BreadcrumbRed(Clone)"  , "r" },
        { "BreadcrumbGreen(Clone)", "g" },
        { "BreadcrumbBlue(Clone)" , "b" },
    };

    public SaveGame(DungeonParameters dungeonParameters, GameObject player,
                    GameObject itemsParent, GameObject breadcrumbsParent,
                    int keysCollected, float timeElapsed)
    {
        DungeonParameters = dungeonParameters;
        PlayerPosition = player.transform.position;
        PlayerRotation = player.transform.rotation;
        KeysCollected = keysCollected;
        TimeElapsed = timeElapsed;
        Items = new List<ItemSave>();
        Breadcrumbs = new List<ItemSave>();
        foreach (Transform child in itemsParent.transform)
        {
            string name = child.name;
            if (name.EndsWith(SaveGame.CloneSuffix))
            {
                name = name.Remove(name.Length - SaveGame.CloneSuffix.Length);
            }
            Items.Add(new ItemSave(name, child.position, child.rotation, child.localScale));
        }
        foreach (Transform child in breadcrumbsParent.transform)
        {
            Breadcrumbs.Add(new ItemSave(
                BreadcrumbNameToChar[child.name], child.position, child.rotation, child.localScale));
        }
    }
}

/// <summary>
/// A representation of GameObjects that can be serialized.
/// </summary>
[Serializable]
public class ItemSave
{
    public string Name;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;

    public ItemSave(string name, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Name = name;
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
}
