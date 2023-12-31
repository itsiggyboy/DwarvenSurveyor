using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchButtonCameraJump : MonoBehaviour
{
    public bool region = false;
    public GameObject targetReticle;
    public int indexToJumpTo;
    public Vector2 positionToJumpTo = Vector2.zero;
    Vector2 offset = Vector2.zero;

    public MapXMLParser parser;

    public Material highlightMat;

    public void Jump()
    {
        if(!region)
        {
            offset = new Vector2(-0.25f, -0.25f);
            targetReticle.transform.position = new Vector3(positionToJumpTo.x + offset.x, positionToJumpTo.y + offset.y, -10);
            targetReticle.SetActive(false);
            targetReticle.SetActive(true);
        }
        else
        {
            if (parser.regionHighlight != null)
            {
                Destroy(parser.regionHighlight.gameObject);
            }
            GameObject go = Instantiate(parser.regionMeshes[indexToJumpTo],parser.regionParent);
            parser.regionHighlight = go;

            Debug.Log("rotation now " + parser.regionHighlight.transform.eulerAngles);
            go.transform.localPosition = new Vector3(0, 0, 2);
            Debug.Log("position now " + parser.regionHighlight.transform.position);
            List<MeshRenderer> renderers = new List<MeshRenderer>();
            if(go.GetComponent<MeshRenderer>()!= null)
            {
                renderers.Add(go.GetComponent<MeshRenderer>());
                go.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            }
            else
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    renderers.Add(go.transform.GetChild(i).GetComponent<MeshRenderer>());
                }
                go.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }

            foreach (MeshRenderer renderer in renderers)
            {
                renderer.material = highlightMat;
            }
        }
        Camera.main.transform.position = new Vector3(positionToJumpTo.x, positionToJumpTo.y, Camera.main.transform.position.z);
        Camera.main.orthographicSize = 5;
        Camera.main.GetComponent<CameraMover>().zoomLevel = 3;
    }

}
