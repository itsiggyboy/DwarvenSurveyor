using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchButtonCameraJump : MonoBehaviour
{

    public Vector2 positionToJumpTo = Vector2.zero;

    public void Jump()
    {
        Camera.main.transform.position = new Vector3(positionToJumpTo.x, positionToJumpTo.y, Camera.main.transform.position.z);
        Camera.main.orthographicSize = 5;
        Camera.main.GetComponent<CameraMover>().zoomLevel = 3;
    }
}
