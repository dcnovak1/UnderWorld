using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CellularNoiseSetting : NoiseSettings
{
    public int iterationCount;

    public int wallCountThreshHold;

    [Range(0, 100)]
    public int fillPercentage;

    public int seed;

    public NoiseSettings BiomeEdgeMerger;

    public override float[,,] GetNoiseMap(Vector3 worldPosition, TerrainWorldSettings terrainWorldSettings, NoiseSettings[] adjacentChunks)
    {

        foreach (var adjacentChunk in adjacentChunks)
        {
            if (adjacentChunk != null)
            {
                return BiomeEdgeMerger.GetNoiseMap(worldPosition, terrainWorldSettings, adjacentChunks);
            }
        }

        Debug.Log("No Biome Merge but calls biome merger at: " + worldPosition);
        return new float[terrainWorldSettings.ChunkSize, terrainWorldSettings.ChunkSize, terrainWorldSettings.ChunkSize];//CellularAutomataGenerator.GenerateNoiseMap(terrainWorldSettings.ChunkSize, worldPosition, this);
    }
}
