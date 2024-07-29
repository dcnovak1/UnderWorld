using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base template class for noise settings,
/// with NoiseGeneration Reference function
/// </summary>
public abstract class NoiseSettings : ScriptableObject
{
    //EveryNoiseNeeds a weed out value for the marching cubes to generate the mesh
    public float marchingCubesWeedOutValue;

    //every biome/noise needs a mesh prefab for different stylization
    public GameObject chunkMeshPrefab;

    /// <summary>
    /// Allows each child to define which noise function to use.
    /// This makes it possible to use different noise functions not just perlin
    /// </summary>
    /// <param name="worldPosition">The world space the chunk starts</param>
    /// <param name="terrainWorldSettings">The world settings to define chunk size and more</param>
    /// <returns></returns>
    public virtual float[,,] GetNoiseMap(Vector3 worldPosition, TerrainWorldSettings terrainWorldSettings, NoiseSettings[] adjacentChunks)
    {
        return null;
    }

    public virtual float[,,] GetNoiseMap(Vector3 worldPosition, TerrainWorldSettings terrainWorldSettings, NoiseSettings[] adjacentChunks, NoiseSettings currentNoise)
    {
        return null;
    }

}
