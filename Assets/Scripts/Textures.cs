using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGeneratorNS;

public class Textures : MonoBehaviour
{
    public Material wallMaterial;
    public Texture wallTexture;

    public static readonly byte minColor = 18;
    public static readonly byte maxColor = 58;

    public Material RandomWallMaterial()
    {
        byte GetAndRemove(List<byte> list, int index)
        {
            byte val = list[index];
            list.RemoveAt(index);
            return val;
        }

        List<byte> rgb = new List<byte>();
        rgb.Add(minColor);
        rgb.Add(maxColor);
        rgb.Add((byte)GM.Rng.Next(minColor, maxColor));

        byte red = GetAndRemove(rgb, Random.Range(0, 3));
        byte green = GetAndRemove(rgb, Random.Range(0, 2));
        byte blue = rgb[0];

        Material material = new Material(wallMaterial);
        material.color = new Color32(red, green, blue, 255);
        material.mainTexture = wallTexture;
        return material;
    }
}
