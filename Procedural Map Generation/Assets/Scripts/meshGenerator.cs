using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class meshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] p_heightMap, meshSettings p_meshSettings, int p_levelOfDetail)
    {
        // SkipIncrement is the amount vertices to be skipped for the mesh;
        // if the LOD is 0, then the skip increment is 1, otherwise the skip increment is the LOD * 2;

        int skipIncrement = (p_levelOfDetail == 0) ? 1 : p_levelOfDetail * 2;

        // Sets the number of vertices per line from the mesh settings script;
        int numVertsPerLine = p_meshSettings.numVertsPerLine;

        // Sets the top left to -1 x and 1 y values in the vector 2 and then multiplying that with the mesh world size divided by 2f;
        Vector2 topLeft = new Vector2(-1, 1) * p_meshSettings.meshWorldSize / 2f;

        // Creates a new mesh data object from the mesh data class, assigning the number of vertices per line, skip increment and the bool variable of using flatshading or not
        MeshData meshData = new MeshData(numVertsPerLine, skipIncrement, p_meshSettings.useFlatShading);

        // Creates a new vertex indices map
        int[,] vertexIndicesMap = new int[numVertsPerLine, numVertsPerLine];

        // Default indexes for mesh vertices and out of mesh vertices
        int meshVertexIndex = 0;
        int outOfMeshVertexIndex = -1;

        // For loop for settings the skipped and out of mesh vertices
        for (int y = 0; y < numVertsPerLine; y++)
        {
            for (int x = 0; x < numVertsPerLine; x++)
            {
                // Checks if the vertex at x and y is an out of mesh vertex
                bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;

                // Checks if the vertex at x and y is a skipped vertex
                bool isSkippedVertex = x > 2 && x < numVertsPerLine - 3 && y > 2 && y < numVertsPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                // If it is an out of mesh vertex, the vertexIndicesMap at x and y is set to the out of mesh vertex index
                if (isOutOfMeshVertex)
                {
                    vertexIndicesMap[x, y] = outOfMeshVertexIndex;
                    outOfMeshVertexIndex--;     // The out of mesh index is then decremented
                }

                // If it is a skipped vertex, the vertexIndicesMap at x and y is set to the mesh vertex index
                else if (!isSkippedVertex)
                {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;          // The mesh vertex index is then incremented
                }
            }
        }

        // For loop for the setting the heights and the triangles for the meshes

        for (int y = 0; y < numVertsPerLine; y++)
        {
            for (int x = 0; x < numVertsPerLine; x++)
            {
                // Only sets the heights and triangles if the vertices are not skipped vertices
                bool isSkippedVertex = x > 2 && x < numVertsPerLine - 3 && y > 2 && y < numVertsPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);
                if (!isSkippedVertex)
                { 
                    // Checks if it's an out of mesh vertex if x and y == 0 or == number of vertices per line - 1
                    bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;

                    // Checks if it's mesh edge vertex by checking if x and y is == 0 or number of vertices per line - 2 and it isn't an out of mesh vertex
                    bool isMeshEdgeVertex = (y == 1 || y == numVertsPerLine - 2 || x == 1 || x == numVertsPerLine - 2) && !isOutOfMeshVertex;

                    // Checks if it's a main vertex if x and y - 2 is == 0 by modding it with the skip increment and if it is neither of a out of mesh or mesh edge vertex
                    bool isMainVertex = (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0 && !isOutOfMeshVertex && !isMeshEdgeVertex;

                    // Checks if it's an edge connection vertex if x and y == 2 or number of vertices per line - 3 and is niether a main, mesh edge or out of mesh vertices
                    bool isEdgeConnectionVertex = (y == 2 || y == numVertsPerLine - 3 || x == 2 || x == numVertsPerLine - 3) && !isOutOfMeshVertex && !isMeshEdgeVertex && !isMainVertex;

                    // Sets to the vertex index to the vertex indices map at x and y
                    int vertexIndex = vertexIndicesMap[x, y];

                    // sets the percent value at the vector2 position of x and y - 1 divided by the numver of vertices per line - 3
                    Vector2 percent = new Vector2(x - 1, y - 1) / (numVertsPerLine - 3);

                    // the vertex position is then set to equal the top left value + a new vector 2 value of the percent values of being positive at x and negative at y
                    // The reason why we set the percent to be negative at y is because the terrain doesn't generate properly and becomes inverted
                    // we then multiply this value to the mesh world size
                    Vector2 vertexPosition2D = topLeft + new Vector2(percent.x, -percent.y) * p_meshSettings.meshWorldSize;

                    // the height value is set to the height map at x and y
                    float height = p_heightMap[x, y];

                    // If the vertex at x and y is a edge connection vertex, we generate the height value
                    if (isEdgeConnectionVertex)
                    {
                        // Checks if the vertex is verticle
                        bool isVertical = x == 2 || x == numVertsPerLine - 3;

                        // If verticle, distance to main vertex A is set to y - 2, otherwise its x - 2, then we mod that value by the skip increment;
                        int distanceToMainVertexA = ((isVertical) ? y - 2 : x - 2) % skipIncrement;

                        // The distance to main vertex b is calculated by the skip increment minus the vertex a distance
                        int distanceToMainVertexB = skipIncrement - distanceToMainVertexA;

                        // The distance percent from A to B is calculated by the main vertex a divided by the skip increment
                        float distancePrecentFromAToB = distanceToMainVertexA / (float)skipIncrement;

                        /* 
                         * height of main vertex a is set the the heightMap
                         * heightMap x is set to either x if is vertical is true or x - distance to main vertex a if it's false
                         * heightMap y is set to y - distance to main vertex a if true, otherwise its set to y if false;
                         * height of main vertex b is the same as above but instead of -, set to the +;
                        */
                        float heightMainVertexA = p_heightMap[(isVertical) ? x : x - distanceToMainVertexA, (isVertical) ? y - distanceToMainVertexA : y];
                        float heightMainVertexB = p_heightMap[(isVertical) ? x : x + distanceToMainVertexB, (isVertical) ? y + distanceToMainVertexB : y];

                        // the height variable is then set to equal the height of main vertex a, multiplied but the distance percent - 1,
                        // plus the height of main vertex b multiplied by the distance percent;
                        height = heightMainVertexA * (1 - distancePrecentFromAToB) + heightMainVertexB * distancePrecentFromAToB;
                    }

                    // Adds the vertex data to the mesh data
                    meshData.AddVertex(new Vector3(vertexPosition2D.x, height, vertexPosition2D.y), percent, vertexIndex);

                    // Checks the requirements to create a triangle
                    bool createTriangle = x < numVertsPerLine - 1 && y < numVertsPerLine - 1 && (!isEdgeConnectionVertex || (x != 2 && y != 2));

                    // If true, creates the triangles for the mesh data
                    if (createTriangle)
                    {
                        int currentIncrement = (isMainVertex && x != numVertsPerLine - 3 && y != numVertsPerLine - 3) ? skipIncrement : 1;
                        int A = vertexIndicesMap[x, y];
                        int B = vertexIndicesMap[x + currentIncrement, y];
                        int C = vertexIndicesMap[x, y + currentIncrement];
                        int D = vertexIndicesMap[x + currentIncrement, y + currentIncrement];

                        meshData.addTriangle(A, D, C);
                        meshData.addTriangle(D, A, B);
                    }
                    
                }
            }
        }
        // Calls the process mesh function
        meshData.ProcessMesh();

        // Returns the mesh data;
        return meshData;
    }
}
public class MeshData
{
    // Variables for the mesh data
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    Vector3[] bakedNormals;
    Vector3[] outOfMeshVertices;
    int[] outOfMeshTriangles;

    // Mesh triangle indexs
    int triangleIndex;
    int outOfMeshTriangleIndex;

    // Bool to use flat shading or not
    bool useFlatShading;

    public MeshData(int numVertsPerLine,int skipIncrement, bool useFlatShading)
    {
        this.useFlatShading = useFlatShading;

        // Calculates all the points for the different vertices
        int numMeshEdgeVertices = (numVertsPerLine - 2) * 4 - 4;
        int numEdgeConnectionVertices = (skipIncrement - 1) * (numVertsPerLine - 5) / skipIncrement * 4;
        int numMainVerticesPerLine = (numVertsPerLine - 5) / skipIncrement + 1;
        int numMainVertices = numMainVerticesPerLine * numMainVerticesPerLine;

        // Adds all the above into the vertices vector 3 variable;
        vertices = new Vector3[numMeshEdgeVertices + numEdgeConnectionVertices + numMainVertices];

        // uvs are set to the length of the vertices variable;
        uvs = new Vector2[vertices.Length];

        // calculates the mesh edge triangles and the main triangles
        int numMeshEdgeTriangles = 8 * (numVertsPerLine - 4);
        int numMainTriangles = (numMainVerticesPerLine - 1) * (numMainVerticesPerLine - 1) * 2;

        // adds the main and mesh edge triangles together and multiply it by 3
        triangles = new int[(numMeshEdgeTriangles + numMainTriangles)*3];

        // sets the out of mesh vertices and triangles
        outOfMeshVertices = new Vector3[numVertsPerLine * 4 - 4];
        outOfMeshTriangles = new int[24* (numVertsPerLine -2)];

    }

    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
    {
        // Adds the vertices position to the vertices and uvs based on the vertex index
        if (vertexIndex < 0)
        {
            outOfMeshVertices[-vertexIndex - 1] = vertexPosition;
        }
        else
        {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }
    }

    public void addTriangle(int a, int b, int c)
    {
        // if a or b or c are 0, the triangle is an out of mesh triangle, otherwise the values are added to the triangles;
        if (a < 0 || b < 0 || c < 0)
        {
            outOfMeshTriangles[outOfMeshTriangleIndex] = a;
            outOfMeshTriangles[outOfMeshTriangleIndex + 1] = b;
            outOfMeshTriangles[outOfMeshTriangleIndex + 2] = c;
            outOfMeshTriangleIndex += 3;
        }
        else
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
    }

    // Calculates the vertex normals for the meshes
    Vector3[] CalculateNormals()
    {
        // sets the vertex normals to the length of the vertices
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        
        // sets the triangle count to the length of the triangles
        int triangleCount = triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            // sets the vertex index of A, B, C to the triangles at the normal triangle index
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            // Sets the triangle normal of a cross product from the the above vertex indexes
            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

            // Applies the triangle normals to the vertex normals at the different vertex indexes;
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }
        
        // Sets the border triangle count to the length of the out of mesh triangles divided by 3
        int borderTriangleCount = outOfMeshTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++)
        {
            // sets the vertex index of A, B, C to the triangles at the normal triangle index
            int normalTriangleIndex = i * 3;
            int vertexIndexA = outOfMeshTriangles[normalTriangleIndex];
            int vertexIndexB = outOfMeshTriangles[normalTriangleIndex + 1];
            int vertexIndexC = outOfMeshTriangles[normalTriangleIndex + 2];

            // Sets the triangle normal of a cross product from the the above vertex indexes
            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

            // applies the triangle normals to the vertex normals at the different vertex indexes, but only if the indexes are greater than or equal to 0
            if (vertexIndexA >= 0)
            {
                vertexNormals[vertexIndexA] += triangleNormal;
            }
            if (vertexIndexB >= 0)
            {
                vertexNormals[vertexIndexB] += triangleNormal;
            }
            if (vertexIndexC >= 0)
            {
                vertexNormals[vertexIndexC] += triangleNormal;
            }
        }

        // nornmalizes and returns the vertex normals;
        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }
        return vertexNormals;
    }

    Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
    {
        // sets the different points to an out of mesh vertex if the index is less than 0, otherwise it's set to the vertex
        Vector3 pointA = (indexA < 0) ? outOfMeshVertices[-indexA - 1] : vertices[indexA];
        Vector3 pointB = (indexB < 0) ? outOfMeshVertices[-indexB - 1] : vertices[indexB];
        Vector3 pointC = (indexC < 0) ? outOfMeshVertices[-indexC - 1] : vertices[indexC];

        // calculates the different sides
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;

        // Returns a normalized cross product of the sides
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public void ProcessMesh()
    {
        // Calls the different functions for processing the meshes if using flat shading or not
        if (useFlatShading)
        {
            FlatShading();
        }
        else
        {
            BakeNormals();
        }
    }

    void BakeNormals()
    {
        bakedNormals = CalculateNormals();
    }

    void FlatShading()
    {
        // sets the vertices and uvs to be flat shadded
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShadedUVs = new Vector2[triangles.Length];

        for (int i = 0; i<triangles.Length; i++)
        {
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUVs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }
        vertices = flatShadedVertices;
        uvs = flatShadedUVs;
    }

    public Mesh createMesh()
    {
        // Mesh data for creating the mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        // Sets the normals to be reculated if flat shaded or the baked normals if not flat shadded
        if (useFlatShading)
        {
            mesh.RecalculateNormals();
        }
        else
        {
            mesh.normals = bakedNormals;
        }
        // returns the mesh data
        return mesh;
    }
}
