using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel; // Import this namespace for PropertyChanged event

public class MouseCoords : MonoBehaviour
{
    // The method to call when the integer coordinates change
    public delegate void CoordinateChangeHandler(Vector3Int newPosition);
    public event CoordinateChangeHandler OnCoordinateChange;

    // The current integer coordinates of the mouse
    private Vector3Int currentCoordinates;

    private void Start()
    {
        currentCoordinates = GetMouseWorldCoordinates();
    }

    private void Update()
    {
        Vector3Int newCoordinates = GetMouseWorldCoordinates();

        // Check if the coordinates have changed
        if (newCoordinates != currentCoordinates)
        {
            currentCoordinates = newCoordinates;
            OnCoordinateChange?.Invoke(currentCoordinates);
        }
    }

    private Vector3Int GetMouseWorldCoordinates()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3Int worldCoordinates = Vector3Int.RoundToInt(hit.point);
            return worldCoordinates;
        }

        return Vector3Int.zero;
    }
}
