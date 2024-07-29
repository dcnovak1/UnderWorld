using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Biomes : ScriptableObject
{
    
    [SerializeField] BiomeSettings[] biomes;
    [SerializeField] PerlinNoiseSettings biomePerlinNoiseSetting;

    public BiomeSettings GetBiome(Vector3 chunkLocation)
    {
        float noiseValue = GetNoiseValue(chunkLocation, biomePerlinNoiseSetting);

        //Debug.Log(noiseValue);

        foreach (BiomeSettings biome in biomes)
        {
            if (WithinRange(noiseValue, biome.biomeRange))
            {
                return biome;
            }
        }

        return biomes[0];

    }

    private static float GetNoiseValue(Vector3 chunkLocation, PerlinNoiseSettings biomePerlinNoiseSetting)
    {

        Vector3 offset = biomePerlinNoiseSetting.offset + chunkLocation;

        System.Random prng = new System.Random(biomePerlinNoiseSetting.seed);
        Vector3[] octaveOffsets = new Vector3[biomePerlinNoiseSetting.octaves];


        float amplitude = 1;
        float frequency;



        for (int i = 0; i < biomePerlinNoiseSetting.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            float offsetZ = prng.Next(-100000, 100000) + offset.z;
            octaveOffsets[i] = new Vector3(offsetX, offsetY, offsetZ);

            amplitude *= biomePerlinNoiseSetting.persistance;
        }


        if (biomePerlinNoiseSetting.noiseScale == 0)
        {
            biomePerlinNoiseSetting.noiseScale = 0.0001f;
        }


        float noiseHeight = 0;
        amplitude = 1;
        frequency = 1;

        for (int i = 0; i < biomePerlinNoiseSetting.octaves; i++)
        {
            float sampleX = (octaveOffsets[i].x) / biomePerlinNoiseSetting.noiseScale * frequency;
            float sampleY = (octaveOffsets[i].y) / biomePerlinNoiseSetting.noiseScale * frequency;
            float sampleZ = (octaveOffsets[i].z) / biomePerlinNoiseSetting.noiseScale * frequency;

            float perlinValue = (PerlinNoise.NoiseFloat(sampleX, sampleY, sampleZ) * 2) - 1;
            noiseHeight += perlinValue * amplitude;

            amplitude *= biomePerlinNoiseSetting.persistance;
            frequency *= biomePerlinNoiseSetting.lacunarity;
        }


        return noiseHeight;
    }

    private static bool WithinRange(float value, Vector2 range)
    {
        if (range.x <= value && value < range.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public enum OrientationOrdering
    {
        Forward,
        Backward,
        RightSide,
        LeftSide,
        Up,
        Down
    }


    public static Vector3 GetOrientation(OrientationOrdering orientation)
    {
        switch (orientation)
        {
            case OrientationOrdering.Forward:
                return Vector3.forward;
            case OrientationOrdering.Backward:
                return Vector3.back;
            case OrientationOrdering.RightSide:
                return Vector3.right;
            case OrientationOrdering.LeftSide:
                return Vector3.left;
            case OrientationOrdering.Up:
                return Vector3.up;
            case OrientationOrdering.Down:
                return Vector3.down;
            default:
                return Vector3.zero;

        }
    }

    public static NoiseSettings[] GetAdjacentBiomes(int x, int y, int z, Biomes biomes, BiomeSettings currentBiome)
    {
        Vector3 currentPosition = new Vector3(x, y, z);
        NoiseSettings[] neighboringBiomes = new NoiseSettings[6];

        Vector3 positionToCheck = currentPosition + (GetOrientation(OrientationOrdering.Forward));
        BiomeSettings biomeToCheck = biomes.GetBiome(positionToCheck);

        if (!BiomeSettings.CompareBiomes(currentBiome, biomeToCheck))
        {
            neighboringBiomes[(int)OrientationOrdering.Forward] = biomeToCheck.noiseSetting;
        }

        positionToCheck = currentPosition + (GetOrientation(OrientationOrdering.Backward));
        biomeToCheck = biomes.GetBiome(positionToCheck);

        if (!BiomeSettings.CompareBiomes(currentBiome, biomeToCheck))
        {
            neighboringBiomes[(int)OrientationOrdering.Backward] = biomeToCheck.noiseSetting;
        }

        positionToCheck = currentPosition + (GetOrientation(OrientationOrdering.RightSide));
        biomeToCheck = biomes.GetBiome(positionToCheck);

        if (!BiomeSettings.CompareBiomes(currentBiome, biomeToCheck))
        {
            neighboringBiomes[(int)OrientationOrdering.RightSide] = biomeToCheck.noiseSetting;
        }

        positionToCheck = currentPosition + (GetOrientation(OrientationOrdering.LeftSide));
        biomeToCheck = biomes.GetBiome(positionToCheck);

        if (!BiomeSettings.CompareBiomes(currentBiome, biomeToCheck))
        {
            neighboringBiomes[(int)OrientationOrdering.LeftSide] = biomeToCheck.noiseSetting;
        }

        positionToCheck = currentPosition + (GetOrientation(OrientationOrdering.Down));
        biomeToCheck = biomes.GetBiome(positionToCheck);

        if (!BiomeSettings.CompareBiomes(currentBiome, biomeToCheck))
        {
            neighboringBiomes[(int)OrientationOrdering.Down] = biomeToCheck.noiseSetting;
        }

        positionToCheck = currentPosition + (GetOrientation(OrientationOrdering.Up));
        biomeToCheck = biomes.GetBiome(positionToCheck);

        if (!BiomeSettings.CompareBiomes(currentBiome, biomeToCheck))
        {
            neighboringBiomes[(int)OrientationOrdering.Up] = biomeToCheck.noiseSetting;
        }


        return neighboringBiomes;
    }

}
