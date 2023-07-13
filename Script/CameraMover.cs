using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{


    public float moveSpeed = 5f; // The speed of camera movement
    public float smoothing = 0.1f; // The smoothing factor for camera movement
    int speedMultiplier = 1;
    public bool allowUp = true;
    public bool allowDown = true;
    public bool allowLeft = true;
    public bool allowRight = true;

    private Vector3 targetPosition; // The target position to move the camera towards

    public Camera cam;
    public float zoomLevel = 0.5f;
    public float minCamZoom = 400;//Zoom out
    public float maxCamZoom = 20;//Zoom in
    public float zoomMultiplier = 1f;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            speedMultiplier = 5;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            speedMultiplier = 1;
        }

        zoomLevel += Input.GetAxis("Mouse ScrollWheel") * zoomMultiplier;
        zoomLevel = Mathf.Clamp(zoomLevel, 0, 3);
        cam.orthographicSize = Mathf.Lerp(minCamZoom, maxCamZoom, zoomLevel/3) ;
        moveSpeed = Mathf.Lerp(200 * speedMultiplier, 10 * speedMultiplier, zoomLevel/3);

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (allowUp && verticalInput > 0f)
        {
            MoveObject(Vector3.up * verticalInput);
        }
        else if (allowDown && verticalInput < 0f)
        {
            MoveObject(Vector3.down * Mathf.Abs(verticalInput));
        }

        if (allowRight && horizontalInput > 0f)
        {
            MoveObject(Vector3.right * horizontalInput);
        }
        else if (allowLeft && horizontalInput < 0f)
        {
            MoveObject(Vector3.left * Mathf.Abs(horizontalInput));
        }

    }
    private void MoveObject(Vector3 direction)
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}
