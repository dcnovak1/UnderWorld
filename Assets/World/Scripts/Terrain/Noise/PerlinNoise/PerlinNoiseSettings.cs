using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Noise settings for Perlin Noise Generation
/// Stores the data values nessary for Perlin noise generation
/// Contains a function to point to the perlin noise generator
/// </summary>
[CreateAssetMenu]
public class PerlinNoiseSettings : NoiseSettings
{

    //local - using local min max 
    //global - estamating global min max
    public enum NormalizeMode
    {
        Local,
        Global
    }

    //Determines how large the gradients are
    public float noiseScale;

    //currently only global works well
    public NormalizeMode normalizeMode = NormalizeMode.Global;

    //the number of levels of detail you want you perlin noise to have.
    public int octaves;

    //number that determines how much each octave contributes to the overall shape (adjusts amplitude).
    [Range(0, 1)]
    public float persistance;

    //number that determines how much detail is added or removed at each octave (adjusts frequency).
    /*Lacunarity of more than 1 means that each octave will increase it’s level of fine grained detail (increased frqeuency). 
     * Lacunarity of 1 means that each octave will have the sam level of detail. Lacunarity of less than one means that each octave will get smoother. 
     * The last two are usually undesirable so a lacunarity of 2 works quite well.
     */
    public float lacunarity;

    public int seed;

    public Vector3 offset;

    public const float GlobalMaxEstamitionCorrection = 2f;

    public NoiseSettings BiomeEdgeMerger;

    /// <summary>
    /// Used to get the noise map for perlin noise
    /// </summary>
    /// <param name="worldPosition">world position of chunk</param>
    /// <param name="terrainWorldSettings">Used to get the chunk size</param>
    /// <returns></returns>
    public override float[,,] GetNoiseMap(Vector3 worldPosition, TerrainWorldSettings terrainWorldSettings, NoiseSettings[] adjacentChunks)
    {

        foreach (NoiseSettings adjacentChunk in adjacentChunks)
        {
            if (adjacentChunk != null)
            {
                Transform chunkContainer = GameObject.Find("Chunks").transform;
                return BiomeEdgeMerger.GetNoiseMap(worldPosition, terrainWorldSettings, adjacentChunks, this);
            }
        }


        return PerlinNoiseGenerator.GenerateNoiseMap(terrainWorldSettings.ChunkSize, worldPosition, this);
    }

}
