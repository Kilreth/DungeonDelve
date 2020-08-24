using System.Collections.Generic;
using UnityEngine;

public class Textures : MonoBehaviour
{
    [SerializeField]
    private Texture[] wallTextures = null;

    [SerializeField]
    private Material wallMaterial;
    [SerializeField]
    private Material[] materialsWithNormalMaps = null;

    public static readonly byte MinColor = 18;
    public static readonly byte MaxColor = 58;

    /// <summary>
    /// For every room in the dungeon, create a material with
    /// a random color and one of several brick patterns.
    ///
    /// Rooms[i] is assigned materials[i].
    ///
    /// The brick patterns are used in turn,
    /// meaning each pattern will be used equally.
    /// </summary>
    /// <param name="total"></param>
    /// <returns></returns>
    public List<Material> PopulateWallMaterials(int total)
    {
        List<Material> materials = new List<Material>();
        for (int i = 0; i < total; ++i)
        {
            Material m = RandomWallMaterial();
            m.mainTexture = wallTextures[i % wallTextures.Length];
            materials.Add(m);
        }
        return materials;
    }

    /// <summary>
    /// Returns a copy of the base wall material in a randomly-generated color.
    ///
    /// RGB values:
    ///   - one value is 18
    ///   - one value is 58
    ///   - one value is somewhere between 18 and 58
    ///
    /// This produces any color in a dark hue.
    /// </summary>
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

        Material material = new Material(wallMaterial);
        material.color = new Color32(red, green, blue, 255);

        return material;
    }

    public void ApplyGraphicsSettings()
    {
        if (PlayerPrefs.GetInt("Graphics", 1) == 1)
        {
            foreach (Material m in materialsWithNormalMaps)
                m.EnableKeyword("_NORMALMAP");
        }
        else
        {
            foreach (Material m in materialsWithNormalMaps)
                m.DisableKeyword("_NORMALMAP");
        }
    }
}
