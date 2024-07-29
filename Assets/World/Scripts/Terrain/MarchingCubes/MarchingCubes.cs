using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MarchingCubes
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="noiseMap"></param>
    /// <param name="surfaceLevel"></param>
    /// <returns></returns>
    /// 

    public static Mesh GenerateMesh(float[,,] densities, float cubeSize, float surfaceLevel)
    {
        Mesh mesh = new Mesh();

        // Determine the dimensions of the grid
        int gridSizeX = densities.GetLength(0) - 1;
        int gridSizeY = densities.GetLength(1) - 1;
        int gridSizeZ = densities.GetLength(2) - 1;

        // Lists to store vertices and triangles of the generated mesh
        List<Vector3> vertices = new();
        List<int> triangles = new();

        List<(Vector3, int)>[,] cachedTriangles = new List<(Vector3, int)>[gridSizeY, gridSizeZ]; 

        // Vertex and edge layout:
        //
        //            6             7
        //            +-------------+               +-----6-------+   
        //          / |           / |             / |            /|   
        //        /   |         /   |          11   7         10   5
        //    2 +-----+-------+  3  |         +-----+2------+     |   
        //      |   4 +-------+-----+ 5       |     +-----4-+-----+   
        //      |   /         |   /           3   8         1   9
        //      | /           | /             | /           | /       
        //    0 +-------------+ 1             +------0------+         
        //
        // Triangulation cases are generated prioritising rotations over inversions, which can introduce non-manifold geometry.
        //
        // Iterate over each cell of the 3D grid
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    List<(Vector3, int)> newTraingleCache = new();

                    int[] triangulation = FindTriangulationFromDensities(x, y, z, densities, surfaceLevel);

                    /*
                    (List<Vector3> vertices, List<int> triangles) meshData = CalculateMeshData(x, y, z, triangulation, triangles, vertices, cubeSize);

                    vertices.AddRange(meshData.vertices);
                    triangles.AddRange(meshData.triangles);
                    */


                    // Iterate over the triangles in this cell's triangulation
                    for (int i = 0; i < triangulation.Length - 1; i += 3)
                    {
                        
                        for (int k = 0; k < 3; k++)
                        {
                            int edgeIndex = triangulation[i + k];

                            Vector3 vertex = GetPositionFromTriangulationIndex(x, y, z, edgeIndex, cubeSize);

                            //0,1,2,3,4,7,8,9,11
                            if (x == 0 || y == 0 || z == 0 || edgeIndex == 1 || edgeIndex == 2 || edgeIndex == 5 || edgeIndex == 6 || edgeIndex == 7 || edgeIndex == 10 || edgeIndex == 11)
                            {

                                int vertexIndex = vertices.Count;

                                vertices.Add(vertex);
                                triangles.Add(vertexIndex);

                                newTraingleCache.Add((vertex, vertexIndex));

                            }
                            else
                            {
                                int VertexIndexFromTrianglesIndex = GetVertexIndexFromTrianglesIndex(x, y, z, edgeIndex, vertex, cachedTriangles);

                                if (VertexIndexFromTrianglesIndex == -1)
                                {
                                    int vertexIndex = vertices.Count;

                                    vertices.Add(vertex);
                                    triangles.Add(vertexIndex);

                                    newTraingleCache.Add((vertex, vertexIndex));
                                }
                                else
                                {
                                    triangles.Add(VertexIndexFromTrianglesIndex);
                                    newTraingleCache.Add((vertex, VertexIndexFromTrianglesIndex));
                                }
                            }

                        }
                        

                        /*
                        // Get the edge indices for this triangle
                        int edgeIndex1 = triangulation[i];
                        int edgeIndex2 = triangulation[i + 1];
                        int edgeIndex3 = triangulation[i + 2];


                        Vector3 vertex1 = GetPositionFromTriangulationIndex(x, y, z, edgeIndex1, cubeSize);
                        Vector3 vertex2 = GetPositionFromTriangulationIndex(x, y, z, edgeIndex2, cubeSize);
                        Vector3 vertex3 = GetPositionFromTriangulationIndex(x, y, z, edgeIndex3, cubeSize);



                        // Add the vertices and triangles to their respective lists
                        int vertexIndex1 = vertices.Count;
                        int vertexIndex2 = vertices.Count + 1;
                        int vertexIndex3 = vertices.Count + 2;

                        vertices.Add(vertex1);
                        vertices.Add(vertex2);
                        vertices.Add(vertex3);

                        triangles.Add(vertexIndex1);
                        triangles.Add(vertexIndex2);
                        triangles.Add(vertexIndex3);
                        */
                    }

                    cachedTriangles[y, z] = newTraingleCache;
                }
            }
        }

        // Assign vertices and triangles to the mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    private static int[] FindTriangulationFromDensities(int x, int y, int z, float[,,] densities, float surfaceLevel)
    {
        // Sample density values for the eight corners of the current cell
        float[] density = {
                        densities[x, y, z], //0
                        densities[x + 1, y, z],//1
                        densities[x, y + 1, z],//2
                        densities[x + 1, y + 1, z],//3
                        densities[x, y, z + 1],//4
                        densities[x + 1, y, z + 1],//5
                        densities[x, y + 1, z + 1],//6
                        densities[x + 1, y + 1, z + 1]//7
                    };

        // Calculate the cube index based on the density values
        // The cube index is a bitmask representing which corners are below the surface level
        int cubeIndex = 0;
        if (density[0] < surfaceLevel) cubeIndex |= 1;
        if (density[1] < surfaceLevel) cubeIndex |= 2;
        if (density[2] < surfaceLevel) cubeIndex |= 4;
        if (density[3] < surfaceLevel) cubeIndex |= 8;
        if (density[4] < surfaceLevel) cubeIndex |= 16;
        if (density[5] < surfaceLevel) cubeIndex |= 32;
        if (density[6] < surfaceLevel) cubeIndex |= 64;
        if (density[7] < surfaceLevel) cubeIndex |= 128;

        // Retrieve the triangulation data for this cube configuration
        return MarchingCubesTriangulationTable.GetTriangulationFromTable(cubeIndex);

    }

    private static (List<Vector3> vertices, List<int> triangles) CalculateMeshData(int x, int y, int z, int[] triangulation, List<int> triangles, List<Vector3> vertices, float cubeSize)
    {
        List<Vector3> currentVertices = new();
        List<int> currentTriangles = new();

        //int vertexIndex = vertices.Count;

        // Iterate over the triangles in this cell's triangulation
        for (int i = 0; i < triangulation.Length - 1; i += 3)
        {
            /*
            for (int k = 0; k < 3; k++)
            {
                int edgeIndex = triangulation[i + k];

                //0,1,2,3,4,7,8,9,11
                if ((x == 0 && y == 0 && z == 0) && (edgeIndex == 5 || edgeIndex == 6 || edgeIndex == 10))
                {
                    Vector3 vertex = GetPositionFromTriangulationIndex(x, y, z, edgeIndex, cubeSize);

                    currentVertices.Add(vertex);
                    currentTriangles.Add(vertexIndex);
                    vertexIndex++;
                }
                else
                {
                    int VertexIndexFromTrianglesIndex = GetVertexIndexFromTrianglesIndex(x, y, z, edgeIndex, triangles);
                    currentTriangles.Add(VertexIndexFromTrianglesIndex);
                }

            }
            */
            // Get the edge indices for this triangle
            int edgeIndex1 = triangulation[i];
            int edgeIndex2 = triangulation[i + 1];
            int edgeIndex3 = triangulation[i + 2];


            Vector3 vertex1 = GetPositionFromTriangulationIndex(x, y, z, edgeIndex1, cubeSize);
            Vector3 vertex2 = GetPositionFromTriangulationIndex(x, y, z, edgeIndex2, cubeSize);
            Vector3 vertex3 = GetPositionFromTriangulationIndex(x, y, z, edgeIndex3, cubeSize);



            // Add the vertices and triangles to their respective lists
            int vertexIndex1 = vertices.Count + i;
            int vertexIndex2 = vertices.Count + i + 1;
            int vertexIndex3 = vertices.Count + i + 2;

            currentVertices.Add(vertex1);
            currentVertices.Add(vertex2);
            currentVertices.Add(vertex3);

            currentTriangles.Add(vertexIndex1);
            currentTriangles.Add(vertexIndex2);
            currentTriangles.Add(vertexIndex3);
        }

        return (currentVertices, currentTriangles);
    }


    private static Vector3 GetPositionFromTriangulationIndex(int x, int y, int z, int index, float cubeSize)
    {

        float halfCubeSize = cubeSize / 2;

        // Vertex and edge layout:
        //
        //            6             7
        //            +-------------+               +-----6-------+   
        //          / |           / |             / |            /|   
        //        /   |         /   |          11   7         10   5
        //    2 +-----+-------+  3  |         +-----+2------+     |   
        //      |   4 +-------+-----+ 5       |     +-----4-+-----+   
        //      |   /         |   /           3   8         1   9
        //      | /           | /             | /           | /       
        //    0 +-------------+ 1             +------0------+         
        //

        // 0,1,2,3,4,7,8,9,11

        switch (index)
        {
            case 0:
                return new Vector3(x + halfCubeSize, y, z);
            case 1:
                return new Vector3(x + cubeSize, y + halfCubeSize, z);
            case 2:
                return new Vector3(x + halfCubeSize, y + cubeSize, z);
            case 3:
                return new Vector3(x, y + halfCubeSize, z);
            case 4:
                return new Vector3(x + halfCubeSize, y, z + cubeSize);
            case 5:
                return new Vector3(x + cubeSize, y + halfCubeSize, z + cubeSize);
            case 6:
                return new Vector3(x + halfCubeSize, y + cubeSize, z + cubeSize);
            case 7:
                return new Vector3(x, y + halfCubeSize, z + cubeSize);
            case 8:
                return new Vector3(x, y, z + halfCubeSize);
            case 9:
                return new Vector3(x + cubeSize, y, z + halfCubeSize);
            case 10:
                return new Vector3(x + cubeSize, y + cubeSize, z + halfCubeSize);
            case 11:
                return new Vector3(x, y + cubeSize, z + halfCubeSize);
            default:
                return new Vector3(x, y, z);
        }
    }

    private static int GetVertexIndexFromTrianglesIndex(int x, int y, int z, int index, Vector3 vertex, List<(Vector3, int)>[,] cachedTriangles)
    {

        // Vertex and edge layout:
        //
        //            6             7
        //            +-------------+               +-----6-------+   
        //          / |           / |             / |            /|   
        //        /   |         /   |          11   7         10   5
        //    2 +-----+-------+  3  |         +-----+2------+     |   
        //      |   4 +-------+-----+ 5       |     +-----4-+-----+   
        //      |   /         |   /           3   8         1   9
        //      | /           | /             | /           | /       
        //    0 +-------------+ 1             +------0------+         
        //

        switch (index)
        {
            case 0:
                return SearchList(cachedTriangles[y - 1, z], vertex);
            case 1:
                return SearchList(cachedTriangles[y, z], vertex);
            case 2:
                return SearchList(cachedTriangles[y, z], vertex);
            case 3:
                return SearchList(cachedTriangles[y, z - 1], vertex);
            case 4:
                //Debug.Log(x + "," + y + "," + z );
                return SearchList(cachedTriangles[y - 1, z], vertex);
            case 7:
                return SearchList(cachedTriangles[y, z - 1], vertex);
            case 8:
                return SearchList(cachedTriangles[y - 1, z], vertex);
            case 9:
                return SearchList(cachedTriangles[y - 1, z], vertex);
            case 11:
                return SearchList(cachedTriangles[y, z - 1], vertex);
            default:
                return -1;
        }
    }

    private static int SearchList(List<(Vector3 vertex, int vertexIndex)> list, Vector3 vertex)
    {

        foreach (var tuple in list)
        {
            if (tuple.vertex == vertex)
            {
                return tuple.vertexIndex;
            }
        }

        return -1;
    }


    #region Surface Already Defined
    /*
    public static Mesh GenerateMeshNoWeed(float[,,] densities)
    {
        Mesh mesh = new Mesh();

        // Determine the dimensions of the grid
        int gridSizeX = densities.GetLength(0) - 1;
        int gridSizeY = densities.GetLength(1) - 1;
        int gridSizeZ = densities.GetLength(2) - 1;

        // Lists to store vertices and triangles of the generated mesh
        var vertices = new System.Collections.Generic.List<Vector3>();
        var triangles = new System.Collections.Generic.List<int>();


        // Vertex and edge layout:
        //
        //            6             7
        //            +-------------+               +-----6-------+   
        //          / |           / |             / |            /|   
        //        /   |         /   |          11   7         10   5
        //    2 +-----+-------+  3  |         +-----+2------+     |   
        //      |   4 +-------+-----+ 5       |     +-----4-+-----+   
        //      |   /         |   /           3   8         1   9
        //      | /           | /             | /           | /       
        //    0 +-------------+ 1             +------0------+         
        //
        // Triangulation cases are generated prioritising rotations over inversions, which can introduce non-manifold geometry.
        //
        // Iterate over each cell of the 3D grid
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    // Sample density values for the eight corners of the current cell
                    float[] density = {
                        densities[x, y, z], //0
                        densities[x + 1, y, z],//1
                        densities[x, y + 1, z],//2
                        densities[x + 1, y + 1, z],//3
                        densities[x, y, z + 1],//4
                        densities[x + 1, y, z + 1],//5
                        densities[x, y + 1, z + 1],//6
                        densities[x + 1, y + 1, z + 1]//7
                    };

                    // Calculate the cube index based on the density values
                    // The cube index is a bitmask representing which corners are below the surface level
                    int cubeIndex = 0;
                    cubeIndex += (int)density[0];
                    cubeIndex += (int)density[1] << 1;
                    cubeIndex += (int)density[2] << 2;
                    cubeIndex += (int)density[3] << 3;
                    cubeIndex += (int)density[4] << 4;
                    cubeIndex += (int)density[5] << 5;
                    cubeIndex += (int)density[6] << 6;
                    cubeIndex += (int)density[7] << 7;

                    // Retrieve the triangulation data for this cube configuration
                    int[] triangulation = MarchingCubesTriangulationTable.GetTriangulationFromTable(cubeIndex);

                    // Iterate over the triangles in this cell's triangulation
                    for (int i = 0; i < triangulation.Length - 1; i += 3)
                    {

                        // Get the edge indices for this triangle
                        int edgeIndex1 = triangulation[i];
                        int edgeIndex2 = triangulation[i + 1];
                        int edgeIndex3 = triangulation[i + 2];


                        Vector3 vertex1 = GetPositionFromTriangulationIndex(x, y, z, edgeIndex1, 1f);
                        Vector3 vertex2 = GetPositionFromTriangulationIndex(x, y, z, edgeIndex2, 1f);
                        Vector3 vertex3 = GetPositionFromTriangulationIndex(x, y, z, edgeIndex3, 1f);

                        // Add the vertices and triangles to their respective lists
                        int vertexIndex1 = vertices.Count;
                        vertices.Add(vertex1);
                        int vertexIndex2 = vertices.Count;
                        vertices.Add(vertex2);
                        int vertexIndex3 = vertices.Count;
                        vertices.Add(vertex3);

                        triangles.Add(vertexIndex1);
                        triangles.Add(vertexIndex2);
                        triangles.Add(vertexIndex3);
                    }
                }
            }
        }

        // Assign vertices and triangles to the mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }
    */
    #endregion


    #region Interpolated

    private static Vector3 InterpolateVertex(Vector3 p1, Vector3 p2, float d1, float d2, float surfaceLevel)
    {
        // Calculate the interpolation factor
        float t = (surfaceLevel - d1) / (d2 - d1);

        // Interpolate the vertex position
        return p1 + t * (p2 - p1);
    }


    public static Mesh GenerateMeshInterpolate(float[,,] densities, float cubeSize, float surfaceLevel)
    {
        Mesh mesh = new Mesh();

        // Determine the dimensions of the grid
        int gridSizeX = densities.GetLength(0) - 1;
        int gridSizeY = densities.GetLength(1) - 1;
        int gridSizeZ = densities.GetLength(2) - 1;

        // Lists to store vertices and triangles of the generated mesh
        var vertices = new System.Collections.Generic.List<Vector3>();
        var triangles = new System.Collections.Generic.List<int>();


        // Vertex and edge layout:
        //
        //            6             7
        //            +-------------+               +-----6-------+   
        //          / |           / |             / |            /|   
        //        /   |         /   |          11   7         10   5
        //    2 +-----+-------+  3  |         +-----+2------+     |   
        //      |   4 +-------+-----+ 5       |     +-----4-+-----+   
        //      |   /         |   /           3   8         1   9
        //      | /           | /             | /           | /       
        //    0 +-------------+ 1             +------0------+         
        //
        // Triangulation cases are generated prioritising rotations over inversions, which can introduce non-manifold geometry.
        //
        // Iterate over each cell of the 3D grid

        // x = y * gridSizeZ + z
        // x = 
        // y = y * gridSizeZ + z
        // z -1

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    // Sample density values for the eight corners of the current cell
                    float[] density = {
                        densities[x, y, z], //0
                        densities[x + 1, y, z],//1
                        densities[x, y + 1, z],//2
                        densities[x + 1, y + 1, z],//3
                        densities[x, y, z + 1],//4
                        densities[x + 1, y, z + 1],//5
                        densities[x, y + 1, z + 1],//6
                        densities[x + 1, y + 1, z + 1]//7
                    };

                    // Calculate the cube index based on the density values
                    // The cube index is a bitmask representing which corners are below the surface level
                    int cubeIndex = 0;
                    if (density[0] < surfaceLevel) cubeIndex |= 1;
                    if (density[1] < surfaceLevel) cubeIndex |= 2;
                    if (density[2] < surfaceLevel) cubeIndex |= 4;
                    if (density[3] < surfaceLevel) cubeIndex |= 8;
                    if (density[4] < surfaceLevel) cubeIndex |= 16;
                    if (density[5] < surfaceLevel) cubeIndex |= 32;
                    if (density[6] < surfaceLevel) cubeIndex |= 64;
                    if (density[7] < surfaceLevel) cubeIndex |= 128;

                    // Retrieve the triangulation data for this cube configuration
                    int[] triangulation = MarchingCubesTriangulationTable.GetTriangulationFromTable(cubeIndex);

                    // Iterate over the triangles in this cell's triangulation
                    for (int i = 0; i < triangulation.Length - 1; i += 3)
                    {

                        // Get the edge indices for this triangle
                        int edgeIndex1 = triangulation[i];
                        int edgeIndex2 = triangulation[i + 1];
                        int edgeIndex3 = triangulation[i + 2];


                        Vector3 vertex1 = GetPositionFromTriangulationIndexInterpolated(x, y, z, edgeIndex1, density, surfaceLevel);
                        Vector3 vertex2 = GetPositionFromTriangulationIndexInterpolated(x, y, z, edgeIndex2, density, surfaceLevel);
                        Vector3 vertex3 = GetPositionFromTriangulationIndexInterpolated(x, y, z, edgeIndex3, density, surfaceLevel);

                        // Add the vertices and triangles to their respective lists
                        int vertexIndex1 = vertices.Count;
                        vertices.Add(vertex1);
                        int vertexIndex2 = vertices.Count;
                        vertices.Add(vertex2);
                        int vertexIndex3 = vertices.Count;
                        vertices.Add(vertex3);

                        triangles.Add(vertexIndex1);
                        triangles.Add(vertexIndex2);
                        triangles.Add(vertexIndex3);
                    }
                }
            }
        }

        // Assign vertices and triangles to the mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }


    private static Vector3 GetPositionFromTriangulationIndexInterpolated(int x, int y, int z, int index, float[] density, float surfaceLevel)
    {

        // Vertex and edge layout:
        //
        //            6             7
        //            +-------------+               +-----6-------+   
        //          / |           / |             / |            /|   
        //        /   |         /   |          11   7         10   5
        //    2 +-----+-------+  3  |         +-----+2------+     |   
        //      |   4 +-------+-----+ 5       |     +-----4-+-----+   
        //      |   /         |   /           3   8         1   9
        //      | /           | /             | /           | /       
        //    0 +-------------+ 1             +------0------+         
        //

        //InterpolateVertex(Vector3 p1, Vector3 p2, float d1, float d2, float surfaceLevel)

        switch (index)
        {
            case 0:
                return InterpolateVertex(new Vector3(x, y, z), new Vector3(x + 1, y, z), density[0], density[1], surfaceLevel);
            case 1:
                return InterpolateVertex(new Vector3(x + 1, y, z), new Vector3(x + 1, y + 1, z), density[1], density[3], surfaceLevel);
            case 2:
                return InterpolateVertex(new Vector3(x, y + 1, z), new Vector3(x + 1, y + 1, z), density[2], density[3], surfaceLevel);
            case 3:
                return InterpolateVertex(new Vector3(x, y, z), new Vector3(x, y + 1, z), density[0], density[2], surfaceLevel);
            case 4:
                return InterpolateVertex(new Vector3(x, y, z + 1), new Vector3(x + 1, y, z + 1), density[4], density[5], surfaceLevel);
            case 5:
                return InterpolateVertex(new Vector3(x + 1, y, z + 1), new Vector3(x + 1, y + 1, z + 1), density[5], density[7], surfaceLevel);
            case 6:
                return InterpolateVertex(new Vector3(x, y + 1, z + 1), new Vector3(x + 1, y + 1, z + 1), density[6], density[7], surfaceLevel);
            case 7:
                return InterpolateVertex(new Vector3(x, y, z + 1), new Vector3(x, y + 1, z + 1), density[4], density[6], surfaceLevel);
            case 8:
                return InterpolateVertex(new Vector3(x, y, z), new Vector3(x, y, z + 1), density[0], density[4], surfaceLevel);
            case 9:
                return InterpolateVertex(new Vector3(x + 1, y, z), new Vector3(x + 1, y, z + 1), density[1], density[5], surfaceLevel);
            case 10:
                return InterpolateVertex(new Vector3(x + 1, y + 1, z), new Vector3(x + 1, y + 1, z + 1), density[3], density[7], surfaceLevel);
            case 11:
                return InterpolateVertex(new Vector3(x, y + 1, z), new Vector3(x, y + 1, z + 1), density[2], density[6], surfaceLevel);
            default:
                return new Vector3(x, y, z);
        }
    }
    #endregion
}
