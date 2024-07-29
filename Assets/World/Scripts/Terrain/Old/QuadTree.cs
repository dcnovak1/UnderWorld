using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree : ScriptableObject
{
    // Arbitrary constant to indicate how many elements can be stored in this quad tree node
    const int QT_NODE_CAPACITY = 4;
    const int MAX_DEPTH = 2;
    int currentDepth = 1;

    static Vector3[] meshVerts = new Vector3[0];
    static int[] meshTris = new int[0];

    // Axis-aligned bounding box stored as a center with half-dimensions
    // to represent the boundaries of this quad tree
    BoundingArea boundary;

    int numberOfChildren = 0;
    QuadTree[] children;


    QuadTree(Vector3 boundingArea, int size)
    {
        boundary = new BoundingArea(boundingArea, size);

        if(currentDepth != int.MaxValue)
        {
            numberOfChildren = QT_NODE_CAPACITY;
            children = new QuadTree[numberOfChildren];

            int halfOfSize = size / 2;

            //bLeft
            children[0] = new QuadTree(boundingArea, halfOfSize);

            //bRight
            children[1] = new QuadTree(new Vector3(boundingArea.x + halfOfSize, boundingArea.y, boundingArea.z), halfOfSize, currentDepth + 1);
            //tLeft
            children[2] = new QuadTree(new Vector3(boundingArea.x, boundingArea.y, boundingArea.z + halfOfSize), halfOfSize, currentDepth + 1);
            //tRight
            children[3] = new QuadTree(new Vector3(boundingArea.x + halfOfSize, boundingArea.y, boundingArea.z + halfOfSize), halfOfSize, currentDepth + 1);
        }

    }

    QuadTree(Vector3 boundingArea, int size, int newDepth)
    {
        boundary = new BoundingArea(boundingArea, size);

        currentDepth = newDepth;

        if (currentDepth != int.MaxValue)
        {
            numberOfChildren = QT_NODE_CAPACITY;
            children = new QuadTree[numberOfChildren];

            int halfOfSize = size / 2;

            //bLeft
            children[0] = new QuadTree(boundingArea, halfOfSize);

            //bRight
            children[1] = new QuadTree(new Vector3(boundingArea.x + halfOfSize, boundingArea.y, boundingArea.z), halfOfSize, currentDepth + 1);
            //tLeft
            children[2] = new QuadTree(new Vector3(boundingArea.x, boundingArea.y, boundingArea.z + halfOfSize), halfOfSize, currentDepth + 1);
            //tRight
            children[3] = new QuadTree(new Vector3(boundingArea.x + halfOfSize, boundingArea.y, boundingArea.z + halfOfSize), halfOfSize, currentDepth + 1);
        }

    }

    // Methods
    //function __construct(AABB _boundary) { ...}
    //function insert(XY p) { ...}
    //function subdivide() { ...} // create four children that fully divide this quad into four quads of equal area
    //function queryRange(AABB range) { ...}

    private void createChildren()
    {

    }

}

// Axis-aligned bounding box with half dimension and center
struct BoundingArea
{
    Vector3 bottomLeft;
    Vector3 bottomRight;
    Vector3 topLeft;
    Vector3 topRight;


    public BoundingArea(Vector3 bLeft, int areaSize)
    {
        this.bottomLeft = bLeft;
        this.bottomRight = new Vector3(bLeft.x, bLeft.y, bLeft.z + areaSize);
        this.topLeft = new Vector3(bLeft.x + areaSize, bLeft.y, bLeft.z);
        this.topRight = new Vector3(topLeft.x, bLeft.y, bottomRight.z);
    }

    //public bool WithinArea(Vector3 point)

    //function __construct(XY _center, float _halfDimension) { ...}
    //function intersectsAABB(AABB other) { ...}
}
