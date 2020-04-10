using System;
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
        public GameObject wall;
        public GameObject portal;
        public GameObject key;
        public GameObject keyPad;
    }
    [SerializeField]
    private PrefabsSelect prefabs = new PrefabsSelect();
    private Dictionary<Block, GameObject> blockPrefabs;
    private Dictionary<string, GameObject> itemPrefabs;
    private GameObject blocksParent;
    private List<Material> wallMaterials;

    [HideInInspector]
    public GameObject ItemsParent;

    private DungeonGenerator dungeonGenerator;
    private Dungeon dungeon;

    public void CreateDungeon()
    {
        dungeonGenerator = new DungeonGenerator(GM.Instance.DungeonParameters,
                                                GM.Instance.Random);
        dungeon = dungeonGenerator.Dungeon;
        LoadPrefabs();
        InitializeMaterials();
        InstantiateFloor();
        InstantiateCeiling();
        InstantiateDungeonBlocks();
        // InstantiateNewItems() or InstantiateLoadedItems() to be called via GM
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
            { "Portal",      prefabs.portal },
            { "StartPortal", prefabs.portal },
        };
    }

    private void InitializeMaterials()
    {
        wallMaterials = GetComponent<Textures>().PopulateWallMaterials(dungeon.Rooms.Count);
    }

    public void InstantiateCeiling()
    {
        InstantiateFloorOrCeiling(GM.Instance.BlockScale * 3 + 1, prefabs.ceiling);
    }
    public void InstantiateFloor()
    {
        InstantiateFloorOrCeiling(GM.Instance.BlockScale / 2, prefabs.floor);
    }

    public void InstantiateFloorOrCeiling(float yPosition, GameObject prefab)
    {
        Vector3 scale = new Vector3(GM.Instance.BlockScale * GM.Instance.DungeonParameters.Cols,
                                    GM.Instance.BlockScale,
                                    GM.Instance.BlockScale * GM.Instance.DungeonParameters.Rows);
        Vector3 position = new Vector3((scale.x - GM.Instance.BlockScale) / 2,
                                                  yPosition,
                                       (scale.z - GM.Instance.BlockScale) / 2);
        GameObject gameObj = Instantiate(prefab, position, Quaternion.identity);
        gameObj.transform.localScale = scale;
        Material material = gameObj.GetComponent<Renderer>().material;
        material.mainTextureScale = new Vector2(GM.Instance.DungeonParameters.Cols,
                                                GM.Instance.DungeonParameters.Rows);
    }

    private void InstantiateDungeonBlocks()
    {
        blocksParent = new GameObject("Blocks");

        for (int row = 0; row < dungeon.Height; ++row)
        {
            for (int col = 0; col < dungeon.Width; ++col)
            {
                Tile tile = dungeon.GetTile(row, col);
                GameObject prefab;

                // If the player will never see this block, don't instantiate it
                if ((tile.Block == Block.Rock || tile.Block == Block.Granite)
                    && !dungeon.IsTileAdjacentTo(tile, Block.WALKABLE))
                {
                    continue;
                }

                // Instantiate block, if it is not a ground block
                if (!Tile.IsWalkable(tile))
                {
                    prefab = blockPrefabs[tile.Block];
                    GameObject block = InstantiateObject(prefab, row, col, true, blocksParent);

                    // If a room wall, apply an assigned material featuring a color and pattern
                    if (blockPrefabs[tile.Block] == prefabs.wall && tile.Area != null)
                    {
                        block.GetComponent<Renderer>().material = wallMaterials[tile.Area.Id];
                    }
                }
            }
        }
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
            Instantiate(itemPrefabs[item.Name], item.Position, item.Rotation,
                        ItemsParent.transform);
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
}
