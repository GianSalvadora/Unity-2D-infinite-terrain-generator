using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid
{
    public Vector2Int dimensions;
    public int tileSize = 1;

    public MyChunk ownerChunk;

    public MyTile[,] tiles;

    public MyGrid(Vector2Int dimensions, int tileSize, MyChunk chunk)
    {
        this.dimensions = dimensions;
        this.tileSize = tileSize;
        this.ownerChunk = chunk;

        tiles = new MyTile[dimensions.x, dimensions.y];

        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                // Debug.DrawLine(GetWorldPosition(x, y, false), GetWorldPosition(x + 1, y, false), Color.black, Mathf.Infinity);
                // Debug.DrawLine(GetWorldPosition(x, y, false), GetWorldPosition(x, y + 1, false), Color.black, Mathf.Infinity);


                MyTile tile = tiles[x, y];
                if (!tile)
                {
                    tiles[x, y] = MyWorldGen.instance.GetTile();
                    tile = tiles[x, y];
                    tile.transform.position = GetWorldPosition(x, y, true);
                    tile.transform.parent = chunk.transform;
                }

            }
        }
    }

    public Vector2 GetWorldPosition(int x, int y, bool center)
    {
        Vector2 toReturn = (new Vector3(x, y) + ownerChunk.transform.position) * tileSize;

        if (center)
        {
            return toReturn + (new Vector2(tileSize, tileSize) * .5f);
        }
        else
        {
            return toReturn;
        }
    }
}
