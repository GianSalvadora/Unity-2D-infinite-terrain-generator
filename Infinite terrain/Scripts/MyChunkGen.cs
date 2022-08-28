using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyChunkGen : MonoBehaviour
{
    //chunks
    public List<MyChunk> activeChunks;
    public Vector2Int chunkSize;
    public int tileSize;

    public Queue<MyChunk> chunkPool;
    public int chunkPoolSize = 0;
    public GameObject baseChunk;


    public MyChunk mainChunk;


    //Player
    public Transform player;
    public int renderDistance;


    //self
    public static MyChunkGen instance;

    void Start()
    {
        instance = this;
        for (int i = -renderDistance; i <= renderDistance; i++)
        {
            chunkPoolSize++;
        }

        chunkPoolSize = (chunkPoolSize * chunkPoolSize);
        chunkPool = new Queue<MyChunk>(chunkPoolSize);

        //fills the pool
        for (int x = 0; x < chunkPoolSize; x++)
        {
            MyChunk temp = Instantiate(baseChunk).GetComponent<MyChunk>();
            temp.name = x.ToString();
            chunkPool.Enqueue(temp);
        }


        FastNoiseLite waterFNL = MyWorldGen.instance.waterLevelNoise.GetNoise();
        float waterLevel = MyWorldGen.instance.waterLevel;
        float sandSize = MyWorldGen.instance.sandSize;
        bool playerIsSafe = false;
        Vector2 playerPos = Vector2.zero;
        while (!playerIsSafe)
        {
            if (waterFNL.GetNoise(playerPos.x, playerPos.y) > waterLevel + sandSize)
            {
                playerIsSafe = true;
                player.position = playerPos;
            }
            else
            {
                playerPos += Vector2.up;
            }
        }

        //gets the coordinate of the chunks to load with the player at the center chunk
        List<Vector3Int> finalChunkCoords = LoadChunkCoords(PlayerChunkCoord());

        //loads the chunks then gets the main chunk
        for (int x = 0; x < finalChunkCoords.Count; x++)
        {
            MyChunk tempChunk = SetUpChunk(chunkPool.Dequeue(), finalChunkCoords[x], true);

            if (Mathf.CeilToInt(finalChunkCoords.Count / 2) == x)
            {
                mainChunk = tempChunk;
            }
        }
    }

    public void OnMainChunkChange()
    {
        mainChunk = null;
        //Gets the new main chunk
        foreach (MyChunk chunk in activeChunks)
        {
            Vector3 playerPos = player.position;
            Vector3 selfPos = chunk.transform.position;
            if ((playerPos.x > selfPos.x && playerPos.x < selfPos.x + chunkSize.x * tileSize) && (playerPos.y > selfPos.y && playerPos.y < selfPos.y + chunkSize.y * tileSize))
            {
                mainChunk = chunk;
                break;
            }
        }
        if (mainChunk == null)
        {
            foreach (MyChunk chunk in activeChunks)
            {
                chunkPool.Enqueue(Dismantle(chunk));
            }
            activeChunks = new List<MyChunk>();

            List<Vector3Int> newChun = LoadChunkCoords(PlayerChunkCoord());
            for (int x = 0; x < newChun.Count; x++)
            {
                MyChunk tempChunk = SetUpChunk(chunkPool.Dequeue(), newChun[x], true);

                if (Mathf.CeilToInt(newChun.Count / 2) == x)
                {
                    mainChunk = tempChunk;
                }
            }
        }
        //unload
        Vector3Int mainChunkPos = Vector3Int.FloorToInt(mainChunk.transform.position);
        List<Vector3Int> finalChunkCoords = LoadChunkCoords(new Vector2Int(mainChunkPos.x, mainChunkPos.y));
        //unload byproduct
        List<MyChunk> newActiveChunks = new List<MyChunk>();//chunks that are already loaded and shouldnt be unloaded
        foreach (MyChunk chunk in activeChunks)
        {
            Vector3 oldChunkCenterPosition = chunk.transform.position + (new Vector3(chunkSize.x, chunkSize.y) * 0.5f);
            bool isInsideANewChunk = false;
            foreach (Vector3Int newChunkPos in finalChunkCoords)
            {
                if ((oldChunkCenterPosition.x > newChunkPos.x && oldChunkCenterPosition.x < newChunkPos.x + chunkSize.x * tileSize) && (oldChunkCenterPosition.y > newChunkPos.y && oldChunkCenterPosition.y < newChunkPos.y + chunkSize.y * tileSize))
                {
                    isInsideANewChunk = true;
                    newActiveChunks.Add(chunk);

                }
            }

            if (!isInsideANewChunk)
            {
                chunkPool.Enqueue(Dismantle(chunk));
            }
        }

        //load
        foreach (Vector3Int newChunkPos in finalChunkCoords)
        {
            Vector3 newChunkCenterPosition = newChunkPos + (new Vector3(chunkSize.x, chunkSize.y) * 0.5f);
            bool exists = false;
            foreach (MyChunk activeChunk in activeChunks)
            {
                if (activeChunk.gameObject.activeSelf)
                {
                    Vector3Int activeChunkPos = Vector3Int.FloorToInt(activeChunk.transform.position);
                    if ((newChunkCenterPosition.x > activeChunkPos.x && newChunkCenterPosition.x < activeChunkPos.x + (chunkSize.x * tileSize)) && (newChunkCenterPosition.y > activeChunkPos.y && newChunkCenterPosition.y < activeChunkPos.y + (chunkSize.y * tileSize)))
                    {

                        exists = true;
                    }
                }
            }

            if (!exists)
            {
                newActiveChunks.Add(SetUpChunk(chunkPool.Dequeue(), newChunkPos, false));
            }
        }

        activeChunks = newActiveChunks;
    }

    private MyChunk Dismantle(MyChunk chunk)
    {
        chunk.transform.position = Vector3.zero;
        chunk.gameObject.SetActive(false);
        return chunk;
    }
    private MyChunk SetUpChunk(MyChunk chunk, Vector3Int chunkPos, bool addToActive)
    {
        chunk.transform.position = chunkPos;
        chunk.gameObject.SetActive(true);
        if (addToActive)
            activeChunks.Add(chunk);

        if (chunk.chunkGrid == null)
        {
            chunk.chunkGrid = new MyGrid(chunkSize, tileSize, chunk);
        }
        chunk.TilesInit();

        return chunk;
    }
    List<Vector3Int> LoadChunkCoords(Vector2Int centerChunkCoord)
    {
        List<Vector3Int> toReturn = new List<Vector3Int>();

        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int y = -renderDistance; y <= renderDistance; y++)
            {
                toReturn.Add(Vector3Int.FloorToInt(centerChunkCoord + new Vector2(chunkSize.x * x, chunkSize.y * y)));
            }
        }

        return toReturn;
    }
    Vector2Int PlayerChunkCoord()
    {
        Vector2Int t = chunkSize * tileSize;

        int x = Mathf.FloorToInt(player.position.x / t.x);
        int y = Mathf.FloorToInt(player.position.y / t.y);

        Vector2Int chunkPos = Vector2Int.FloorToInt(transform.position + new Vector3(t.x * x, t.y * y));

        return chunkPos;
    }

}
