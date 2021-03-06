﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGeneratorNS;

public class InstantiateDungeon : MonoBehaviour
{
    [Serializable]
    public struct PrefabsSelect
    {
        public GameObject player;
        public GameObject ceiling;
        public GameObject floor;
        public GameObject floorOnMap;
        public GameObject wall;
        public GameObject portal;
        public GameObject key;
        public GameObject keyPad;
        public GameObject foundKey;
    }
    [SerializeField]
    private PrefabsSelect prefabs = new PrefabsSelect();
    private Dictionary<Block, GameObject> blockPrefabs;
    private Dictionary<string, GameObject> itemPrefabs;
    private List<Material> wallMaterials;

    [HideInInspector]
    public GameObject BlocksParent { get; private set; }
    [HideInInspector]
    public GameObject UnreachableBlocksParent { get; private set; }

    [HideInInspector]
    public GameObject ItemsParent { get; private set; }

    private DungeonGenerator dungeonGenerator;
    private Dungeon dungeon;
    private Textures textures;

    [SerializeField]
    [ColorUsageAttribute(true, true)]
    private Color ambientLight;
    [SerializeField]
    private Color32 ceilingColorLowGraphics;
    [SerializeField]
    private Color32 ceilingColorNormalGraphics;

    public void CreateDungeon()
    {
        dungeonGenerator = new DungeonGenerator(GM.Instance.DungeonParameters,
                                                GM.Instance.Random);
        dungeon = dungeonGenerator.Dungeon;
        textures = GetComponent<Textures>();
        textures.ApplyGraphicsSettings();
        LoadPrefabs();
        InitializeMaterials();
        InstantiateFloor();
        InstantiateCeiling();
        InstantiateDungeonBlocks();
        // InstantiateNewItems() or InstantiateLoadedItems() to be called via GM
        ApplyQualitySettings();
    }

    private void LoadPrefabs()
    {
        blockPrefabs = new Dictionary<Block, GameObject>
        {
            { Block.Granite, prefabs.wall },
            { Block.Rock,    prefabs.wall },
            { Block.Wall,    prefabs.wall },
        };

        itemPrefabs = new Dictionary<string, GameObject>
        {
            { "Key",         prefabs.key    },
            { "KeyPad",      prefabs.keyPad },
            { "FoundKey",    prefabs.foundKey },
            { "Portal",      prefabs.portal },
            { "StartPortal", prefabs.portal },
        };
    }

    private void InitializeMaterials()
    {
        wallMaterials = textures.PopulateWallMaterials(dungeon.Rooms.Count);
    }

    public void InstantiateCeiling()
    {
        // On medium and high quality, the ceiling is near-black, the only color being
        // from the light reflected off it from the player's lantern.
        // The lantern light doesn't show up on low quality, however, so the same ceiling
        // would appear black. Give the ceiling a lighter color on low quality.
        if (QualitySettings.GetQualityLevel() == 0)
            prefabs.ceiling.GetComponent<Renderer>().sharedMaterial.color = ceilingColorLowGraphics;
        else
            prefabs.ceiling.GetComponent<Renderer>().sharedMaterial.color = ceilingColorNormalGraphics;

        InstantiateFloorOrCeiling(GM.Instance.BlockScale * 3 + 1, prefabs.ceiling);
    }
    public void InstantiateFloor()
    {
        // Create the floor the player sees in first person. It's a darker color.
        InstantiateFloorOrCeiling(GM.Instance.BlockScale / 2, prefabs.floor);

        // Create the floor seen on the map. It's a lighter color.
        // This goes beneath the other one so it isn't seen in first person.
        InstantiateFloorOrCeiling(GM.Instance.BlockScale / 2 - 0.01f, prefabs.floorOnMap);
    }

    public void InstantiateFloorOrCeiling(float yPosition, GameObject prefab)
    {
        Vector3 scale = new Vector3(GM.Instance.BlockScale * GM.Instance.DungeonParameters.Cols,
                                    GM.Instance.BlockScale,
                                    GM.Instance.BlockScale * GM.Instance.DungeonParameters.Rows);
        Vector3 position = new Vector3((scale.x - GM.Instance.BlockScale) / 2,
                                                  yPosition,
                                       (scale.z - GM.Instance.BlockScale) / 2);
        GameObject gameObj = Instantiate(prefab, position, prefab.transform.rotation);
        gameObj.transform.localScale = scale;
        Material material = gameObj.GetComponent<Renderer>().material;
        material.mainTextureScale = new Vector2(GM.Instance.DungeonParameters.Cols,
                                                GM.Instance.DungeonParameters.Rows);
    }

    private void InstantiateDungeonBlocks()
    {
        BlocksParent = new GameObject("Blocks");
        UnreachableBlocksParent = new GameObject("UnreachableBlocks");

        for (int row = 0; row < dungeon.Height; ++row)
        {
            for (int col = 0; col < dungeon.Width; ++col)
            {
                Tile tile = dungeon.GetTile(row, col);
                GameObject prefab;

                // Instantiate block, if it is not a ground block
                if (!Tile.IsWalkable(tile))
                {
                    prefab = blockPrefabs[tile.Block];
                    GameObject block = InstantiateObject(prefab, row, col, true);

                    // If a room wall, apply an assigned material featuring a color and pattern
                    if (blockPrefabs[tile.Block] == prefabs.wall && tile.Area != null)
                    {
                        block.GetComponent<Renderer>().material = wallMaterials[tile.Area.Id];
                    }

                    // If this block cannot be seen in first person, put it under a
                    // different parent GameObject. This parent (and consequently all
                    // its blocks) will only be active when viewing the overhead map.
                    if ((tile.Block == Block.Rock || tile.Block == Block.Granite)
                            && !dungeon.IsTileAdjacentTo(tile, Block.WALKABLE))
                        block.transform.SetParent(UnreachableBlocksParent.transform);
                    else
                        block.transform.SetParent(BlocksParent.transform);
                }
            }
        }
        UnreachableBlocksParent.SetActive(false);
    }

    public void InstantiateNewItems()
    {
        ItemsParent = new GameObject("Items");

        for (int row = 0; row < dungeon.Height; ++row)
        {
            for (int col = 0; col < dungeon.Width; ++col)
            {
                Tile tile = dungeon.GetTile(row, col);

                // Instantiate item (eg. key, start portal) if one exists
                if (tile.Item != null)
                {
                    GameObject prefab = itemPrefabs[tile.Item.Name];
                    InstantiateObject(prefab, row, col, true, ItemsParent);

                    // If key, instantiate a pad under it as well
                    if (tile.Item.Name == "Key")
                    {
                        InstantiateObject(itemPrefabs["KeyPad"], row, col, true, ItemsParent);
                    }
                }
            }
        }
    }

    public void InstantiateLoadedItems(List<ItemSave> items)
    {
        ItemsParent = new GameObject("Items");
        foreach (ItemSave item in items)
        {
            GameObject obj = Instantiate(
                itemPrefabs[item.Name], item.Position, item.Rotation, ItemsParent.transform);
            obj.transform.localScale = item.Scale;
        }
    }

    public GameObject InstantiateNewPlayer()
    {
        return InstantiateObject(prefabs.player, dungeon.StartTile.Row, dungeon.StartTile.Col, false);
    }

    public GameObject InstantiateLoadedPlayer(Vector3 position, Quaternion rotation)
    {
        return Instantiate(prefabs.player, position, rotation);
    }

    private GameObject InstantiateObject(GameObject prefab, int row, int col, bool scale, GameObject parent=null)
    {
        Vector3 position = GetPositionFromRowAndCol(row, col, prefab);
        GameObject obj = Instantiate(prefab, new Vector3(position.x, position.y, position.z), prefab.transform.rotation);
        if (parent != null)
        {
            obj.transform.SetParent(parent.transform);
        }
        if (scale)
        {
            obj.transform.localScale = new Vector3(prefab.transform.localScale.x * GM.Instance.BlockScale,
                                                   prefab.transform.localScale.y * GM.Instance.BlockScale,
                                                   prefab.transform.localScale.z * GM.Instance.BlockScale);
        }
        return obj;
    }

    public Vector3 GetPositionFromRowAndCol(int row, int col, GameObject prefab=null)
    {
        if (prefab == null)
        {
            return new Vector3(col * GM.Instance.BlockScale,
                               0,   // also consider GM.Instance.BlockScale
                               row * GM.Instance.BlockScale);
        }
        return new Vector3((col + prefab.transform.position.x) * GM.Instance.BlockScale,
                                  prefab.transform.position.y  * GM.Instance.BlockScale,
                           (row + prefab.transform.position.z) * GM.Instance.BlockScale);
    }

    private void ApplyQualitySettings()
    {
        int quality = QualitySettings.GetQualityLevel();

        // Brighten the dungeon just a bit if on low quality,
        // to make up for missing light from the player's lantern
        RenderSettings.ambientLight = quality == 0 ? ambientLight : Color.black;
    }
}
