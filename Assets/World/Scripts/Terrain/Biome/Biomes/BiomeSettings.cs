using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BiomeSettings : ScriptableObject
{
    public string Name;
    public NoiseSettings noiseSetting;
    public Vector2 biomeRange;

    /// <summary>
    /// true == same , 
    /// false == different
    /// </summary>
    /// <param name="MainBiome">BiomeSettings type</param>
    /// <param name="SecondBiome">BiomeSettings type</param>
    /// <returns></returns>
    public static bool CompareBiomes(BiomeSettings Biome1, BiomeSettings Biome2)
    {
        if (Biome1.Name == Biome2.Name)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
