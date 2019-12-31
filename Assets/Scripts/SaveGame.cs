using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveGame
{
    public DungeonParameters DungeonParameters;
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;
    //public List<BreadcrumbSave> Breadcrumbs;

    public SaveGame(DungeonParameters dungeonParameters, GameObject player)
    {
        DungeonParameters = dungeonParameters;
        PlayerPosition = player.transform.position;
        PlayerRotation = player.transform.rotation;
    }

    /*Breadcrumbs = new List<BreadcrumbSave>();
    foreach (Transform child in breadcrumbs.transform)
    {
        Breadcrumbs.Add(new BreadcrumbSave(child.position, child.rotation));
    }*/
}

/// <summary>
/// For serialization in saving and loading.
/// </summary>
[Serializable]
public class BreadcrumbSave
{
    public Vector3 Position;
    public Quaternion Rotation;

    public BreadcrumbSave(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}
