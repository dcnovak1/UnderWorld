using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CellularAutomataNoise
{

    public enum TileType
    {
        Air = 0,
        Wall = 1
        
    }

    public static float[,,] Noise(float[,,] startingNoise, int iterationCount, int wallCountThreshHold)
    {
        float[,,] noise = startingNoise;
        float[,,] tempNoise = startingNoise;
        for (int iteration = 0; iteration < iterationCount; iteration++)
        {
            noise = tempNoise;

            for (int x = 1; x < noise.GetLength(0) - 1; x++)
            {
                for (int y = 1; y < noise.GetLength(1) - 1; y++)
                {
                    for (int z = 1; z < noise.GetLength(2) - 1; z++)
                    {
                        int wallCount = DetermineWallOrFloor(x, y, z, noise);

                        if (wallCount < wallCountThreshHold)
                        {
                            tempNoise[x, y, z] = (int)TileType.Air;
                        }
                        else if (wallCount > wallCountThreshHold)
                        {
                            tempNoise[x, y, z] = (int)TileType.Wall;
                        }

                    }
                }
            }
        }

        return noise;
    }

    //wall is 1
    //floor is 0
    private static int DetermineWallOrFloor(int X, int Y, int Z, float[,,] noise)
    {
        int wallCount = 0;
        for (int x = X - 1; x < X + 2; x++)
        {
            for (int y = Y - 1; y < Y + 2; y++)
            {
                for (int z = Z - 1; z < Z + 2; z++)
                {
                    if (x == X && y == Y && z == Z)
                    {
                        break;
                    }
                    else if (x < 0 || x >= noise.GetLength(0) || y < 0 || y >= noise.GetLength(1) || z < 0 || z >= noise.GetLength(2))
                    {
                        wallCount++;
                    }
                    else if (noise[x, y, z] >= 0.5f)
                    {
                        wallCount++;
                    }

                }
            }
        }

        return wallCount;

    }


    public static float[,,] Noise(CellularAutomataGenerator.CellularData[,,] startingNoise, int wallCountThreshHold)
    {
        int sizeX = startingNoise.GetLength(0), sizeY = startingNoise.GetLength(1), sizeZ = startingNoise.GetLength(2);
        bool keepGoing = true;
        int iterationCountStop = 0;
        while(keepGoing)
        {

            keepGoing = false;

            for (int x = 1; x < sizeX - 1; x++)
            {
                for (int y = 1; y < sizeY - 1; y++)
                {
                    for (int z = 1; z < sizeZ - 1; z++)
                    {

                        if (startingNoise[x,y,z].isStatic)
                        {
                            break;
                        }
                        else
                        {
                            keepGoing = true;

                            //check static neighbors
                            int staticNeighborCount = 0;
                            int wallCount = 0;
                            for (int X = x - 1; X < x + 2; X++)
                            {
                                for (int Y = y - 1; Y < y + 2; Y++)
                                {
                                    for (int Z = z - 1; Z < z + 2; Z++)
                                    {
                                        if (x == X && y == Y && z == Z)
                                        {
                                            break;
                                        }
                                        else if (X < 0 || X >= sizeX || Y < 0 || Y >= sizeY || Z < 0 || Z >= sizeZ)
                                        {
                                            staticNeighborCount++;
                                            wallCount++;
                                        }
                                        else if (startingNoise[X,Y,Z].isStatic)
                                        {
                                            staticNeighborCount++;

                                            if (startingNoise[X, Y, Z].value > 0.5f)
                                            {
                                                wallCount++;
                                            }

                                        }

                                    }
                                }
                            }

                            if (staticNeighborCount >= 10)
                            {
                                if (wallCount < wallCountThreshHold)
                                {
                                    startingNoise[x, y, z].value = (int)TileType.Air;
                                }
                                else if (wallCount > wallCountThreshHold)
                                {
                                    startingNoise[x, y, z].value = (int)TileType.Wall;
                                }
                            }

                        } 

                    }
                    if (iterationCountStop > 40)
                    {
                        keepGoing = false;
                    }
                    iterationCountStop++;
                }
            }
        }

        float[,,] noise = new float[sizeX, sizeY, sizeZ];

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {

                    noise[x, y, z] = startingNoise[x, y, z].value; 


                }
            }
        }

        return noise;

    }

}
