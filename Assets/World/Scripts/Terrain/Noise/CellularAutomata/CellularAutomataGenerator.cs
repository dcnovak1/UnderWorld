using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class CellularAutomataGenerator
{
    public static float[,,] GenerateMergingNoiseMap(TerrainWorldSettings terrainWorldSettings, Vector3 worldPosition, CellularNoiseBiomeMergingSettings cellularNoiseSettings, NoiseSettings[] adjacentChunks, NoiseSettings currentChunkSettings)
    {

        CellularData[,,] cellularDatas = GeneratingBaseMap(terrainWorldSettings, worldPosition, cellularNoiseSettings, adjacentChunks, currentChunkSettings);

        return CellularAutomataNoise.Noise(cellularDatas, cellularNoiseSettings.wallCountThreshHold);


        //float[,,] startingNoise = GetMergingStartingNoise(terrainWorldSettings.ChunkSize, worldPosition, adjacentChunks, terrainWorldSettings, currentChunkSettings, cellularNoiseSettings);


        //return CellularAutomataNoise.Noise(startingNoise, cellularNoiseSettings.iterationCount, cellularNoiseSettings.wallCountThreshHold);

    }

    private static CellularData[,,] GeneratingBaseMap(TerrainWorldSettings terrainWorldSettings, Vector3 worldPosition, CellularNoiseBiomeMergingSettings cellularNoiseSettings, NoiseSettings[] adjacentChunks, NoiseSettings currentChunkSettings)
    {
        CellularData[,,] cellularDatas = new CellularData[terrainWorldSettings.ChunkSize, terrainWorldSettings.ChunkSize, terrainWorldSettings.ChunkSize];
        float[][,,] adjacentNoiseMaps = GetAdjacentNoiseMaps(terrainWorldSettings.ChunkSize, worldPosition, adjacentChunks, terrainWorldSettings, currentChunkSettings);
        float[,,] currentChunkNoiseMap = currentChunkSettings.GetNoiseMap(worldPosition, terrainWorldSettings, new NoiseSettings[6]);

        for (int x = 0; x < terrainWorldSettings.ChunkSize; x++)
        {
            for (int y = 0; y < terrainWorldSettings.ChunkSize; y++)
            {
                for (int z = 0; z < terrainWorldSettings.ChunkSize; z++)
                {
                    if (x == 0)
                    {
                        if (adjacentNoiseMaps[(int)Biomes.OrientationOrdering.LeftSide] != null)
                        {
                            if (y == 0 && adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Down] != null)
                            {
                                float noiseValue = adjacentNoiseMaps[(int)Biomes.OrientationOrdering.LeftSide][x, y, z] + adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Down][x, y, z];
                                cellularDatas[x, y, z] = new CellularData(noiseValue, true);
                            }
                            else if (z == 0 && adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Backward] != null)
                            {
                                float noiseValue = adjacentNoiseMaps[(int)Biomes.OrientationOrdering.LeftSide][x, y, z] + adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Backward][x, y, z];
                                cellularDatas[x, y, z] = new CellularData(noiseValue, true);
                            }
                            else
                            {
                                cellularDatas[x, y, z] = new CellularData(adjacentNoiseMaps[(int)Biomes.OrientationOrdering.LeftSide][x, y, z], true);
                            }
                        }
                        else
                        {
                            cellularDatas[x, y, z] = new CellularData(currentChunkNoiseMap[x, y, z], true);
                        }
                    }
                    else if (y == 0)
                    {
                        if (adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Down] != null)
                        {
                            if (z == 0 && adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Backward] != null)
                            {
                                float noiseValue = adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Down][x, y, z] + adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Backward][x, y, z];
                                cellularDatas[x, y, z] = new CellularData(noiseValue, true);
                            }
                            else
                            { 
                                cellularDatas[x, y, z] = new CellularData(adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Down][x, y, z], true); 
                            }
                        }

                        else
                        {
                            cellularDatas[x, y, z] = new CellularData(currentChunkNoiseMap[x, y, z], true);
                        }
                    }
                    else if (z == 0)
                    {
                        if (adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Backward] != null)
                        {
                            cellularDatas[x, y, z] = new CellularData(adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Backward][x, y, z], true);
                        }
                        else
                        {
                            cellularDatas[x, y, z] = new CellularData(currentChunkNoiseMap[x, y, z], true);
                        }
                    }
                    else if (x == terrainWorldSettings.ChunkSize - 1)
                    {
                        if (adjacentNoiseMaps[(int)Biomes.OrientationOrdering.RightSide] != null)
                        {
                            if (y == terrainWorldSettings.ChunkSize - 1 && adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Up] != null)
                            {
                                float noiseValue = adjacentNoiseMaps[(int)Biomes.OrientationOrdering.RightSide][x, y, z] + adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Up][x, y, z];
                                cellularDatas[x, y, z] = new CellularData(noiseValue, true);
                            }
                            else if (z == terrainWorldSettings.ChunkSize - 1 && adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Forward] != null)
                            {
                                float noiseValue = adjacentNoiseMaps[(int)Biomes.OrientationOrdering.RightSide][x, y, z] + adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Forward][x, y, z];
                                cellularDatas[x, y, z] = new CellularData(noiseValue, true);
                            }
                            else
                            {
                                cellularDatas[x, y, z] = new CellularData(adjacentNoiseMaps[(int)Biomes.OrientationOrdering.RightSide][x, y, z], true);
                            }
                        }
                        else
                        {
                            cellularDatas[x, y, z] = new CellularData(currentChunkNoiseMap[x, y, z], true);
                        }
                    }
                    else if (y == terrainWorldSettings.ChunkSize - 1)
                    {
                        if (adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Up] != null)
                        {
                            if (z == terrainWorldSettings.ChunkSize - 1 && adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Forward] != null)
                            {
                                float noiseValue = adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Up][x, y, z] + adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Forward][x, y, z];
                                cellularDatas[x, y, z] = new CellularData(noiseValue, true);
                            }
                            else
                            {
                                cellularDatas[x, y, z] = new CellularData(adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Up][x, y, z], true);
                            }
                        }
                        else
                        {
                            cellularDatas[x, y, z] = new CellularData(currentChunkNoiseMap[x, y, z], true);
                        }
                    }
                    else if (z == terrainWorldSettings.ChunkSize - 1)
                    {
                        if (adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Forward] != null)
                        {
                            cellularDatas[x, y, z] = new CellularData(adjacentNoiseMaps[(int)Biomes.OrientationOrdering.Forward][x, y, z], true);
                        }
                        else
                        {
                            cellularDatas[x, y, z] = new CellularData(currentChunkNoiseMap[x, y, z], true);
                        }
                    }
                    else
                    {
                        cellularDatas[x, y, z] = new CellularData(currentChunkNoiseMap[x, y, z], false);
                    }
                }
            }
        }

        return cellularDatas;
    }

    public static float[,,] GenerateNoiseMap(int mapSizeCube, Vector3 worldPosition, CellularNoiseSetting cellularNoiseSettings)
    {

        float[,,] startingNoise = CreateStartingNoiseMap(mapSizeCube, worldPosition, cellularNoiseSettings.seed, cellularNoiseSettings.fillPercentage);

        return CellularAutomataNoise.Noise(startingNoise, cellularNoiseSettings.iterationCount, cellularNoiseSettings.wallCountThreshHold);

    }

    private static float[,,] CreateStartingNoiseMap(int mapSizeCube, Vector3 worldPosition, int seed, int fillPercentage)
    {
        float[,,] startingNoise = new float[mapSizeCube, mapSizeCube, mapSizeCube];

        System.Random prng = new System.Random(((seed * 706973) + ((int)worldPosition.x * 279731) + ((int)worldPosition.y * 841411) + ((int)worldPosition.z * 409337)));

        for (int x = 0; x < mapSizeCube; x++)
        {
            for (int y = 0; y < mapSizeCube; y++)
            {
                for (int z = 0; z < mapSizeCube; z++)
                {
                    startingNoise[x, y, z] = ((float)prng.Next(0,100) > fillPercentage) ? 1:0;
                }
            }
        }

        return startingNoise;
    }


    private static float[,,] CreateStartingNoiseMapMerging(int mapSizeCube, Vector3 worldPosition, int seed, int fillPercentage, float[,,] startingNoise)
    {

        System.Random prng = new System.Random(((seed * 706973) + ((int)worldPosition.x * 279731) + ((int)worldPosition.y * 841411) + ((int)worldPosition.z * 409337)));

        for (int x = 1; x < mapSizeCube-1; x++)
        {
            for (int y = 1; y < mapSizeCube-1; y++)
            {
                for (int z = 1; z < mapSizeCube-1; z++)
                {
                    startingNoise[x, y, z] = ((float)prng.Next(0, 100) > fillPercentage) ? 1 : 0;
                }
            }
        }

        return startingNoise;
    }

    /// <summary>
    /// For each plane closest to surrounding chunks, sets values to the neighboring chunks noise map
    /// </summary>
    /// <param name="mapSizeCube"></param>
    /// <param name="worldPosition"></param>
    /// <param name="adjacentChunks"></param>
    /// <param name="terrainWorldSettings"></param>
    /// <param name="currentChunkSettings"></param>
    /// <param name="cellularNoiseSettings"></param>
    /// <returns></returns>
    private static float[,,] GetMergingStartingNoise(int mapSizeCube, Vector3 worldPosition, NoiseSettings[] adjacentChunks, TerrainWorldSettings terrainWorldSettings, NoiseSettings currentChunkSettings, CellularNoiseBiomeMergingSettings cellularNoiseSettings)
    {
        //float[,,] startingNoiseMap = new float[mapSizeCube, mapSizeCube, mapSizeCube];
        float[,,] startingNoiseMap = currentChunkSettings.GetNoiseMap(worldPosition, terrainWorldSettings, new NoiseSettings[6]);
        //startingNoiseMap = CreateStartingNoiseMapMerging(mapSizeCube, worldPosition, cellularNoiseSettings.seed, cellularNoiseSettings.fillPercentage, startingNoiseMap);

        float[][,,] noiseMaps = GetAdjacentNoiseMaps(mapSizeCube, worldPosition, adjacentChunks, terrainWorldSettings, currentChunkSettings);

        for (int i1 = 0; i1 < mapSizeCube; i1++)
        {
            for (int i2 = 0; i2 < mapSizeCube; i2++)
            {
                startingNoiseMap = SetValueForBoarderIndexWallsForNonNativeNeighboringChunks(i1, i2, noiseMaps, startingNoiseMap);
                //startingNoiseMap = SetValueForBoarderIndexBasedOnAdjacentChunks(i1, i2, noiseMaps, startingNoiseMap);
            }
        }


        return startingNoiseMap;
    }

    private static float[,,] SetValueForBoarderIndexWallsForNonNativeNeighboringChunks(int i1, int i2, float[][,,] noiseMaps, float[,,] startingNoiseMap)
    {
        int zero = 0;
        int lastIndex = startingNoiseMap.GetLength(0) - 1;

        if (noiseMaps[(int)Biomes.OrientationOrdering.Forward] != null)
        {
            startingNoiseMap[i1, i2, lastIndex] = (int)CellularAutomataNoise.TileType.Wall;
            startingNoiseMap[i1, i2, lastIndex-1] = (int)CellularAutomataNoise.TileType.Wall;
        }

        if (noiseMaps[(int)Biomes.OrientationOrdering.Backward] != null)
        {
            startingNoiseMap[i1, i2, zero] = (int)CellularAutomataNoise.TileType.Wall;
            startingNoiseMap[i1, i2, 1] = (int)CellularAutomataNoise.TileType.Wall;
        }

        if (noiseMaps[(int)Biomes.OrientationOrdering.RightSide] != null)
        {
            startingNoiseMap[lastIndex, i1, i2] = (int)CellularAutomataNoise.TileType.Wall;
            startingNoiseMap[lastIndex-1, i1, i2] = (int)CellularAutomataNoise.TileType.Wall;
        }

        if (noiseMaps[(int)Biomes.OrientationOrdering.LeftSide] != null)
        {
            startingNoiseMap[zero, i1, i2] = (int)CellularAutomataNoise.TileType.Wall;
            startingNoiseMap[1, i1, i2] = (int)CellularAutomataNoise.TileType.Wall;
        }

        if (noiseMaps[(int)Biomes.OrientationOrdering.Up] != null)
        {
            startingNoiseMap[i1, lastIndex, i2] = (int)CellularAutomataNoise.TileType.Wall;
            startingNoiseMap[i1, lastIndex-1, i2] = (int)CellularAutomataNoise.TileType.Wall;
        }

        if (noiseMaps[(int)Biomes.OrientationOrdering.Down] != null)
        {
            startingNoiseMap[i1, zero, i2] = (int)CellularAutomataNoise.TileType.Wall;
            startingNoiseMap[i1, 1, i2] = (int)CellularAutomataNoise.TileType.Wall;
        }

        return startingNoiseMap;
    }

    private static float[,,] SetValueForBoarderIndexBasedOnAdjacentChunks(int i1, int i2, float[][,,] noiseMaps, float[,,] startingNoiseMap)
    {
        int zero = 0;
        int lastIndex = startingNoiseMap.GetLength(0) - 1;

        if (noiseMaps[(int)Biomes.OrientationOrdering.Forward] != null)
        {
            startingNoiseMap[i1, i2, lastIndex] = noiseMaps[(int)Biomes.OrientationOrdering.Forward][i1, i2, lastIndex];
        }

        if (noiseMaps[(int)Biomes.OrientationOrdering.Backward] != null)
        {
            startingNoiseMap[i1, i2, zero] = noiseMaps[(int)Biomes.OrientationOrdering.Backward][i1, i2, zero];
        }

        if (noiseMaps[(int)Biomes.OrientationOrdering.RightSide] != null)
        {
            startingNoiseMap[lastIndex, i1, i2] = noiseMaps[(int)Biomes.OrientationOrdering.RightSide][lastIndex, i1, i2];
        }

        if (noiseMaps[(int)Biomes.OrientationOrdering.LeftSide] != null)
        {
            startingNoiseMap[zero, i1, i2] = noiseMaps[(int)Biomes.OrientationOrdering.LeftSide][zero, i1, i2];
        }

        if (noiseMaps[(int)Biomes.OrientationOrdering.Up] != null)
        {
            startingNoiseMap[i1, lastIndex, i2] = noiseMaps[(int)Biomes.OrientationOrdering.Up][i1, lastIndex, i2];
        }

        if (noiseMaps[(int)Biomes.OrientationOrdering.Down] != null)
        {
            startingNoiseMap[i1, zero, i2] = noiseMaps[(int)Biomes.OrientationOrdering.Down][i1, zero, i2];
        }

        return startingNoiseMap;
    }

    private static float[][,,] GetAdjacentNoiseMaps(int mapSizeCube, Vector3 worldPosition, NoiseSettings[] adjacentChunksNoiseSettings, TerrainWorldSettings terrainWorldSettings, NoiseSettings currentChunkNoise)
    {
        float[][,,] noiseMaps = new float[6][,,];

        foreach (Biomes.OrientationOrdering oo in Enum.GetValues(typeof(Biomes.OrientationOrdering)))
        {
            GetNoiseMap(oo);
        }

        return noiseMaps;


        void GetNoiseMap(Biomes.OrientationOrdering orientationOrdering)
        {
            if (adjacentChunksNoiseSettings[(int)orientationOrdering] != null)
            {
                noiseMaps[(int)orientationOrdering] = adjacentChunksNoiseSettings[(int)orientationOrdering].GetNoiseMap(worldPosition + (Biomes.GetOrientation(orientationOrdering) * mapSizeCube), terrainWorldSettings, new NoiseSettings[6]);
            }
        }
    }


    public struct CellularData
    {
        public float value;
        public bool isStatic;

        public CellularData(float value, bool isStatic)
        {
            this.value = value;
            this.isStatic = isStatic;
        }
    } 


}
