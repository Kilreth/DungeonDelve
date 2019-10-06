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
        public GameObject path;
        public GameObject wall;
        public GameObject stairs;
        public GameObject portal;
        public GameObject key;
    }
    public PrefabsSelect PrefabsInEditor;
    public Dictionary<Block, GameObject> BlockPrefabs;
    public Dictionary<string, GameObject> Prefabs;
    public GameObject BlocksParent { get; private set; }
    public GameObject ItemsParent { get; private set; }
    public GameObject Player { get; private set; }
    public DungeonGenerator DungeonGenerator { get; private set; }
    public Dungeon Dungeon { get; private set; }

    private Textures texturesScript;
    private List<Material> wallMaterials;

    public void CreateDungeon()
    {
        Initialize();

        BlockPrefabs = new Dictionary<Block, GameObject>
        {
            { Block.Granite, PrefabsInEditor.wall },
            { Block.Rock,    PrefabsInEditor.wall },
            { Block.Wall,    PrefabsInEditor.wall },

            { Block.Room,    PrefabsInEditor.path },
            { Block.Door,    PrefabsInEditor.path },
            { Block.Path,    PrefabsInEditor.path },

            { Block.StairsUp,   PrefabsInEditor.stairs },
            { Block.StairsDown, PrefabsInEditor.stairs },
        };

        Prefabs = new Dictionary<string, GameObject>
        {
            { "Key",         PrefabsInEditor.key },
            { "StartPortal", PrefabsInEditor.portal },
            { "EndPortal",   PrefabsInEditor.portal },

            { "Player",      PrefabsInEditor.player },
        };

        DungeonGenerator = new DungeonGenerator(60, 80);
        Dungeon = DungeonGenerator.Dungeon;
        CreateDungeonObjects();
    }

    private void CreateDungeonObjects()
    {
        BlocksParent = new GameObject("Blocks");
        ItemsParent = new GameObject("Items");

        for (int row = 0; row < Dungeon.Height; ++row)
        {
            for (int col = 0; col < Dungeon.Width; ++col)
            {
                Tile tile = Dungeon.GetTile(row, col);

                // Instantiate block
                GameObject prefab = BlockPrefabs[tile.Block];
                GameObject block = InstantiateObject(prefab, row, col, true, BlocksParent);

                // If a room wall, apply an assigned material featuring a color and pattern
                if (BlockPrefabs[tile.Block] == PrefabsInEditor.wall && tile.Area != null)
                {
                    block.GetComponent<Renderer>().material = wallMaterials[tile.Area.Id];
                }

                // Instantiate item if one exists
                if (tile.Item != null)
                {
                    block.GetComponent<Renderer>().material = wallMaterials[tile.Area.Id];
                    prefab = Prefabs[tile.Item.Name];
                    GameObject item = InstantiateObject(prefab, row, col, true, ItemsParent);
                    if (tile.Item.Name == "StartPortal")
                    {
                        if (Player != null)
                        {
                            throw new InvalidOperationException("More than one start portal in the dungeon");
                        }
                        Player = InstantiateObject(Prefabs["Player"], row, col, false);
                    }
                }
            }
        }

        if (Player == null)
        {
            throw new InvalidOperationException("No start portal in the dungeon");
        }
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

    private void Initialize()
    {
        texturesScript = GetComponent<Textures>();
        wallMaterials = texturesScript.PopulateWallMaterials(8);
    }
}
