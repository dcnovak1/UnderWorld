using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public static class PerlinNoiseGenerator
{

    public static float[,,] GenerateNoiseMap(int mapSizeCube, Vector3 worldPosition,PerlinNoiseSettings perlinNoiseSettings)
    {
        //mapSizeCube += 1;


        Vector3 offset = perlinNoiseSettings.offset + worldPosition;

        float[,,] noiseMap = new float[mapSizeCube, mapSizeCube, mapSizeCube];

        System.Random prng = new System.Random(perlinNoiseSettings.seed);
        Vector3[] octaveOffsets = new Vector3[perlinNoiseSettings.octaves];

        float maxPossibleHeightGlobal = 0;
        float amplitude = 1;
        float frequency;



        for (int i = 0; i < perlinNoiseSettings.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            float offsetZ = prng.Next(-100000, 100000) + offset.z;
            octaveOffsets[i] = new Vector3(offsetX, offsetY, offsetZ);

            maxPossibleHeightGlobal += amplitude;
            amplitude *= perlinNoiseSettings.persistance;
        }


        if (perlinNoiseSettings.noiseScale == 0)
        {
            perlinNoiseSettings.noiseScale = 0.0001f;
        }

        float minLocalHeightValue = float.MaxValue;
        float maxLocalHeightValue = float.MinValue;


        float halfWidth = mapSizeCube / 2;
        float halfHeight = mapSizeCube / 2;
        float halfDepth = mapSizeCube / 2;


        for (int x = 0; x < mapSizeCube; x++)
        {
            for (int y = 0; y < mapSizeCube; y++)
            {
                for (int z = 0; z < mapSizeCube; z++)
                {


                    float noiseHeight = 0;
                    amplitude = 1;
                    frequency = 1;

                    for (int i = 0; i < perlinNoiseSettings.octaves; i++)
                    {
                        float sampleX = (x - halfWidth + octaveOffsets[i].x) / perlinNoiseSettings.noiseScale * frequency;
                        float sampleY = (y - halfHeight + octaveOffsets[i].y) / perlinNoiseSettings.noiseScale * frequency;
                        float sampleZ = (z - halfDepth + octaveOffsets[i].z) / perlinNoiseSettings.noiseScale * frequency;

                        float perlinValue = (PerlinNoise.NoiseFloat(sampleX, sampleY, sampleZ) * 2) - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= perlinNoiseSettings.persistance;
                        frequency *= perlinNoiseSettings.lacunarity;
                    }
                    if (minLocalHeightValue > noiseHeight)
                    {
                        minLocalHeightValue = noiseHeight;
                    }
                    if (maxLocalHeightValue < noiseHeight)
                    {
                        maxLocalHeightValue = noiseHeight;
                    }


                    noiseMap[x, y, z] = noiseHeight;
                }
            }
        }


        for (int x = 0; x < mapSizeCube; x++)
        {
            for (int y = 0; y < mapSizeCube; y++)
            {
                for (int z = 0; z < mapSizeCube; z++)
                {
                    if (perlinNoiseSettings.normalizeMode == PerlinNoiseSettings.NormalizeMode.Local)
                    {
                        noiseMap[x, y, z] = Mathf.InverseLerp(minLocalHeightValue, maxLocalHeightValue, noiseMap[x, y, z]);
                    }
                    else if (perlinNoiseSettings.normalizeMode == PerlinNoiseSettings.NormalizeMode.Global)
                    {
                        float normalizedHeight = (noiseMap[x, y, z] + 1) / (2 * maxPossibleHeightGlobal / PerlinNoiseSettings.GlobalMaxEstamitionCorrection);
                        noiseMap[x, y, z] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                    }
                }
            }
        }

        return noiseMap;
    }





}
