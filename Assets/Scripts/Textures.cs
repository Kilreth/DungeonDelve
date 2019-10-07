using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGeneratorNS;

public class Textures : MonoBehaviour
{
    [Serializable]
    public struct WallTextures
    {
        public Texture wall1;
        public Texture wall2;
        public Texture wall3;
        public Texture wall4;
        public Texture wall5;
        public Texture wall6;
        public Texture wall7;
        public Texture wall8;
    }
    public WallTextures WallTexturesInEditor;
    private readonly int totalWallTextures = 8;

    public Material WallMaterial;

    public static readonly byte MinColor = 18;
    public static readonly byte MaxColor = 58;

    public List<Material> PopulateWallMaterials(int repetitionsPerTexture)
    {
        List<Texture> wallTextures = new List<Texture>
        {
            WallTexturesInEditor.wall1,
            WallTexturesInEditor.wall2,
            WallTexturesInEditor.wall3,
            WallTexturesInEditor.wall4,
            WallTexturesInEditor.wall5,
            WallTexturesInEditor.wall6,
            WallTexturesInEditor.wall7,
            WallTexturesInEditor.wall8
        };

        int total = repetitionsPerTexture * totalWallTextures;
        List<Material> materials = new List<Material>();
        for (int i = 0; i < total; ++i)
        {
            materials.Add(RandomWallMaterial());
            materials[i].mainTexture = wallTextures[i % totalWallTextures];
        }
        return materials;
    }

    public Material RandomWallMaterial()
    {
        byte GetAndRemove(List<byte> list, int index)
        {
            byte val = list[index];
            list.RemoveAt(index);
            return val;
        }

        List<byte> rgb = new List<byte>();
        rgb.Add(MinColor);
        rgb.Add(MaxColor);
        rgb.Add((byte)GM.Instance.Random.Next(MinColor, MaxColor));

        byte red = GetAndRemove(rgb, GM.Instance.Random.Next(0, 3));
        byte green = GetAndRemove(rgb, GM.Instance.Random.Next(0, 2));
        byte blue = rgb[0];

        Material material = new Material(WallMaterial);
        material.color = new Color32(red, green, blue, 255);
        return material;
    }
}
