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
    }
    public PrefabsSelect prefabsInEditor;
    public Dictionary<Block, GameObject> prefabs;

    public GameObject BlocksParent { get; private set; }
    public DungeonGenerator DungeonGenerator { get; private set; }
    public Dungeon Dungeon { get; private set; }

    private Textures texturesScript;

    public void CreateDungeon()
    {
        Initialize();

        prefabs = new Dictionary<Block, GameObject>
        {
            { Block.Granite, prefabsInEditor.wall },
            { Block.Rock, prefabsInEditor.wall },
            { Block.Wall, prefabsInEditor.wall },

            { Block.Room, prefabsInEditor.path },
            { Block.Door, prefabsInEditor.path },
            { Block.Path, prefabsInEditor.path },

            { Block.StairsUp, prefabsInEditor.stairs },
            { Block.StairsDown, prefabsInEditor.stairs },
            { Block.Key, prefabsInEditor.path },
        };

        DungeonGenerator = new DungeonGenerator(60, 80);
        Dungeon = DungeonGenerator.Dungeon;
        CreateDungeonObjects();
    }

    private void CreateDungeonObjects()
    {
        BlocksParent = new GameObject("Blocks");
        Material wallMaterial = texturesScript.RandomWallMaterial();

        for (int row = 0; row < Dungeon.Height; ++row)
        {
            for (int col = 0; col < Dungeon.Width; ++col)
            {
                Tile tile = Dungeon.GetTile(row, col);
                GameObject prefab = prefabs[tile.Block];
                GameObject block = Instantiate(prefab, new Vector3(col * GM.Instance.BlockScale,
                                                                   prefab.transform.position.y,
                                                                   row * GM.Instance.BlockScale),
                                               Quaternion.identity, BlocksParent.transform);
                block.transform.localScale = new Vector3(GM.Instance.BlockScale,
                                                         GM.Instance.BlockScale * prefab.transform.localScale.y,
                                                         GM.Instance.BlockScale);
                if (prefabs[tile.Block] == prefabsInEditor.wall)
                {
                    block.GetComponent<Renderer>().material = wallMaterial;
                }
            }
        }
    }

    private void Initialize()
    {
        texturesScript = GetComponent<Textures>();
    }
}
