using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CellularNoiseBiomeMergingSettings : NoiseSettings
{

    public int iterationCount;

    public int wallCountThreshHold;

    [Range(0,100)]
    public int fillPercentage;

    public int seed;


    public override float[,,] GetNoiseMap(Vector3 worldPosition, TerrainWorldSettings terrainWorldSettings, NoiseSettings[] adjacentChunks, NoiseSettings currentChunk)
    {
        return CellularAutomataGenerator.GenerateMergingNoiseMap(terrainWorldSettings, worldPosition, this, adjacentChunks, currentChunk);
    }
}
