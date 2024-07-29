using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinitTerrain : MonoBehaviour
{

    [SerializeField] TerrainWorldSettings terrainWorldSettings;
    [SerializeField] Biomes biomes;
    [SerializeField] Transform chunkContainer;

    public int chunkLoadRadius;

    [SerializeField] private Transform viewer;
    private Vector3 viewerLastPosition = Vector3.zero;
    public readonly float viewerDistanceTillUpdate = 10f;

    //Delete later for testing only
    public void EdiorGenerateChunks()
    {
        RemoveAllChunks();


        GenerateChunkRadius(viewer.position);
    }


    // Start is called before the first frame update
    void Start()
    {
        loadedChunks = new Dictionary<Vector3, GameObject>();
        GenerateChunkRadius(viewer.position);
        
    }

    // Update is called once per frame
    void Update()
    {

        float distance = Vector3.Distance(viewer.position, viewerLastPosition);

        if (distance > viewerDistanceTillUpdate)
        {
            GenerateChunkRadius(viewer.position);
            viewerLastPosition = viewer.position;
        }

    }

    private Dictionary<Vector3, GameObject> loadedChunks;

    private void GenerateChunkRadius(Vector3 viewerPosition)
    {

        Vector3 viewersChunkLocation = WorldCordsToChunkCords(viewerPosition);

        for (int x = (int)viewersChunkLocation.x - chunkLoadRadius; x < (chunkLoadRadius + (int)viewersChunkLocation.x); x++)
        {
            for (int y = (int)viewersChunkLocation.y - chunkLoadRadius; y < (chunkLoadRadius + (int)viewersChunkLocation.y); y++)
            {
                for (int z = (int)viewersChunkLocation.z - chunkLoadRadius; z < (chunkLoadRadius + (int)viewersChunkLocation.z); z++)
                {

                    if (loadedChunks.ContainsKey(new Vector3(x,y,z)))
                    {
                        break;
                    }

                    Vector3 worldPositionOfChunk = new Vector3((x * (terrainWorldSettings.ChunkSize-1)), (y * (terrainWorldSettings.ChunkSize - 1)), (z * (terrainWorldSettings.ChunkSize - 1)));
                    BiomeSettings biomeSetting = biomes.GetBiome(new Vector3(x,y,z));

                    NoiseSettings[] adjacentBiomes = Biomes.GetAdjacentBiomes(x,y,z, biomes, biomeSetting);

                    GameObject chunk = ChunkGenerator.GenerateChunk(worldPositionOfChunk, biomeSetting.noiseSetting, terrainWorldSettings, chunkContainer, adjacentBiomes);
                    loadedChunks.Add(new Vector3(x, y, z), chunk);
                }
            }
        }
    }

    private Vector3 WorldCordsToChunkCords(Vector3 viewerPosition)
    {
        Vector3 residingChunk = new Vector3();
        residingChunk.x = ((int)viewerPosition.x) / terrainWorldSettings.ChunkSize;
        residingChunk.y = ((int)viewerPosition.y) / terrainWorldSettings.ChunkSize;
        residingChunk.z = ((int)viewerPosition.z) / terrainWorldSettings.ChunkSize;

        Vector3 correctNegative = new Vector3(((residingChunk.x < 0) ? -1 : 0), ((residingChunk.y < 0) ? -1 : 0), ((residingChunk.z < 0) ? -1 : 0));

        return residingChunk + correctNegative;
    }


    private void RemoveAllChunks()
    {
        var children = new List<GameObject>();
        foreach (Transform child in chunkContainer) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        loadedChunks.Clear();
    }

}
