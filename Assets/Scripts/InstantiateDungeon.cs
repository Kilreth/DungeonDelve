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
    private Dictionary<Block, GameObject> blockPrefabs;
    private Dictionary<string, GameObject> prefabs;
    private GameObject blocksParent;
    private GameObject itemsParent;
    private List<Material> wallMaterials;

    private DungeonGenerator dungeonGenerator;
    private Dungeon dungeon;

    public void CreateDungeon()
    {
        dungeonGenerator = new DungeonGenerator(GM.Instance.DungeonParameters,
                                                GM.Instance.Random);
        dungeon = dungeonGenerator.Dungeon;
        LoadPrefabs();
        InitializeMaterials();
        CreateDungeonObjects();
    }

    private void LoadPrefabs()
    {
        blockPrefabs = new Dictionary<Block, GameObject>
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

        prefabs = new Dictionary<string, GameObject>
        {
            { "Key",         PrefabsInEditor.key },
            { "StartPortal", PrefabsInEditor.portal },

            { "Player",      PrefabsInEditor.player },
        };
    }

    private void InitializeMaterials()
    {
        wallMaterials = GetComponent<Textures>().PopulateWallMaterials(dungeon.Rooms.Count);
    }

    private void CreateDungeonObjects()
    {
        blocksParent = new GameObject("Blocks");
        itemsParent = new GameObject("Items");

        for (int row = 0; row < dungeon.Height; ++row)
        {
            for (int col = 0; col < dungeon.Width; ++col)
            {
                Tile tile = dungeon.GetTile(row, col);

                // Instantiate block
                GameObject prefab = blockPrefabs[tile.Block];
                GameObject block = InstantiateObject(prefab, row, col, true, blocksParent);

                // If a room wall, apply an assigned material featuring a color and pattern
                if (blockPrefabs[tile.Block] == PrefabsInEditor.wall && tile.Area != null)
                {
                    block.GetComponent<Renderer>().material = wallMaterials[tile.Area.Id];
                }

                // Instantiate item (eg. key, start portal) if one exists
                if (tile.Item != null)
                {
                    // add wall texture to the floor block under the item for increased visibility
                    block.GetComponent<Renderer>().material = wallMaterials[tile.Area.Id];

                    prefab = prefabs[tile.Item.Name];
                    InstantiateObject(prefab, row, col, true, itemsParent);
                }
            }
        }
    }

    public GameObject InstantiatePlayer()
    {
        return InstantiateObject(prefabs["Player"], dungeon.StartTile.Row, dungeon.StartTile.Col, false);
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
