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
        public GameObject path;
        public GameObject wall;
        public GameObject stairs;
        public GameObject portal;
        public GameObject key;
    }
    public PrefabsSelect PrefabsInEditor;
    public Dictionary<Block, GameObject> BlockPrefabs;
    public Dictionary<string, GameObject> ItemPrefabs;
    public GameObject BlocksParent { get; private set; }
    public GameObject ItemsParent { get; private set; }
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

        ItemPrefabs = new Dictionary<string, GameObject>
        {
            { "Key",         PrefabsInEditor.key },
            { "StartPortal", PrefabsInEditor.portal },
            { "EndPortal",   PrefabsInEditor.portal },
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
                GameObject block = InstantiateObject(prefab, BlocksParent, row, col);

                // If a room wall, apply an assigned material featuring a color and pattern
                if (BlockPrefabs[tile.Block] == PrefabsInEditor.wall && tile.Area != null)
                {
                    block.GetComponent<Renderer>().material = wallMaterials[tile.Area.Id];
                }

                // Instantiate item if one exists
                if (tile.Item != null)
                {
                    block.GetComponent<Renderer>().material = wallMaterials[tile.Area.Id];
                    prefab = ItemPrefabs[tile.Item.Name];
                    GameObject item = InstantiateObject(prefab, ItemsParent, row, col);
                }
            }
        }
    }

    private GameObject InstantiateObject(GameObject prefab, GameObject parent, int row, int col)
    {
        float x = (col + prefab.transform.position.x) * GM.Instance.BlockScale;
        float y =        prefab.transform.position.y  * GM.Instance.BlockScale;
        float z = (row + prefab.transform.position.z) * GM.Instance.BlockScale;
        GameObject obj = Instantiate(prefab, new Vector3(x, y, z), prefab.transform.rotation, parent.transform);
        obj.transform.localScale = new Vector3(prefab.transform.localScale.x * GM.Instance.BlockScale,
                                               prefab.transform.localScale.y * GM.Instance.BlockScale,
                                               prefab.transform.localScale.z * GM.Instance.BlockScale);
        return obj;
    }

    private void Initialize()
    {
        texturesScript = GetComponent<Textures>();
        wallMaterials = texturesScript.PopulateWallMaterials(8);
    }
}
