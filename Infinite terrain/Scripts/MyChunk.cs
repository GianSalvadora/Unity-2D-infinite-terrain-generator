using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyChunk : MonoBehaviour
{

    public MyGrid chunkGrid;
    Transform player;
    Vector2Int chunkSize;

    int tileSize;
    void Start()
    {
        chunkSize = MyChunkGen.instance.chunkSize;
        tileSize = MyChunkGen.instance.tileSize;
        player = MyWorldGen.instance.player;
    }

    void Update()
    {
        if (MyChunkGen.instance.mainChunk == this)
        {
            Vector3 playerPos = player.position;
            Vector3 selfPos = transform.position;
            Vector2Int dimensions = chunkSize;
            if ((playerPos.x < selfPos.x || playerPos.x > selfPos.x + dimensions.x * tileSize) || (playerPos.y < selfPos.y || playerPos.y > selfPos.y + dimensions.y * tileSize))
            {
                MyChunkGen.instance.OnMainChunkChange();
            }
        }
    }

    public void TilesInit()
    {
        foreach (MyTile tile in chunkGrid.tiles)
        {
            tile.Load();
        }
    }
}
