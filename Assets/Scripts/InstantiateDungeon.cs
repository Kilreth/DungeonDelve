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
    }
    public PrefabsSelect prefabsInEditor;
    public Dictionary<Block, GameObject> prefabs;

    public GameObject instantiateParent;

    private DungeonGenerator dungeonGenerator;
    private Dungeon dungeon;

    void Awake()
    {
        prefabs = new Dictionary<Block, GameObject>
        {
            { Block.Granite, prefabsInEditor.wall },
            { Block.Rock, prefabsInEditor.wall },
            { Block.Wall, prefabsInEditor.wall },

            { Block.Room, prefabsInEditor.path },
            { Block.Door, prefabsInEditor.path },
            { Block.Path, prefabsInEditor.path }
        };

        dungeonGenerator = new DungeonGenerator(60, 80);
        dungeon = dungeonGenerator.Dungeon;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int row = 0; row < dungeon.Height; ++row)
        {
            for (int col = 0; col < dungeon.Width; ++col)
            {
                Tile tile = dungeon.GetTile(row, col);
                GameObject block = prefabs[tile.Block];
                Instantiate(block, new Vector3(col, 0, row), Quaternion.identity, instantiateParent.transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
