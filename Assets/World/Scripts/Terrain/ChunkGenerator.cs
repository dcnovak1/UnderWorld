using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkGenerator
{

    public static GameObject GenerateChunk(Vector3 worldPosition, NoiseSettings noiseSettings, TerrainWorldSettings terrainWorldSettings, Transform chunkContainer, NoiseSettings[] adjacentChunks)
    {

        float[,,] map = noiseSettings.GetNoiseMap(worldPosition, terrainWorldSettings, adjacentChunks);

        if (map.GetLength(0) != terrainWorldSettings.ChunkSize)
        {
            Debug.Log("Chunk at: " + worldPosition + "chunk size of: " + map.GetLength(0) + " does not match chunk setting size: " + terrainWorldSettings.ChunkSize);
        }

        Mesh chunkMesh = PerformMarchingCubes(map, noiseSettings.marchingCubesWeedOutValue);

        return SpawnChunk(worldPosition, chunkMesh, noiseSettings.chunkMeshPrefab, chunkContainer);

    }

    private static Mesh PerformMarchingCubes(float[,,] noiseMap, float weedOutValue)
    {
        return MarchingCubes.GenerateMesh(noiseMap, 1, weedOutValue);
    }

    private static GameObject SpawnChunk(Vector3 worldPosition, Mesh chunkMesh, GameObject chunkPrefab, Transform chunkContainer)
    {

        GameObject chunk = GameObject.Instantiate(chunkPrefab, worldPosition, new Quaternion(0,0,0,0), chunkContainer);

        chunk.GetComponent<MeshFilter>().sharedMesh = chunkMesh;

        return chunk;
    }
}
