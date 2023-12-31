using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCenterFinder : MonoBehaviour
{
    public MeshRenderer mRend;
    public Transform centerObj;
    public Transform topLeftObj;
    public Transform bottomRightObj;
    public GameObject testCube;
    public int maxX = 32;
    public int maxY = 32;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [ContextMenu("Find center")]
    public void FindCenter()
    {
        Mesh mesh = mRend.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        Vector3 topLeft = new Vector2(maxX, 0);
        Vector3 bottomRight = new Vector2(0, maxY);

        Vector3[] verticesFixedYFlipping = mesh.vertices;

        for (int i = 0; i < verticesFixedYFlipping.Length; i++)
        {
            verticesFixedYFlipping[i] = new Vector3(vertices[i].x, maxY - vertices[i].z, 0);
        }

        for (int i = 0; i < vertices.Length; i++)
        {

            Vector3 coordinate = verticesFixedYFlipping[i];

            Debug.Log("Coordinates: " + coordinate);

            // Update topLeft
            topLeft.x = Mathf.Min(topLeft.x, coordinate.x);
            topLeft.y = Mathf.Max(topLeft.y, coordinate.y);

            // Update bottomRight
            bottomRight.x = Mathf.Max(bottomRight.x, coordinate.x);
            bottomRight.y = Mathf.Min(bottomRight.y, coordinate.y);
        }
        Vector3 offset = new Vector3(-0.5f, 0.5f, 0);
        topLeftObj.transform.position = topLeft + offset;
        bottomRightObj.transform.position = bottomRight + offset;
        Vector3 centerWorldPosition = CalculateRectangleCenter(topLeft, bottomRight);
        centerObj.position = centerWorldPosition + offset ;
    }

    Vector2 CalculateRectangleCenter(Vector2 topLeft, Vector2 bottomRight)
    {
        float centerX = (topLeft.x + bottomRight.x) / 2.0f;
        float centerY = (topLeft.y + bottomRight.y) / 2.0f;

        return new Vector2(centerX, centerY);
    }
}
