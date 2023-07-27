using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel; // Import this namespace for PropertyChanged event

public class MouseCoords : MonoBehaviour
{
    public LineRenderer lRend;
    public RegionPanel regionPanel;
    Vector3 point;
    // Property to store the int coordinates
    private int intX;
    private int intY;
    public MapXMLParser xmlParser;

    string currentName;
    string currentType;
    string currentEvilness;

    // Event handler for property changes
    public event PropertyChangedEventHandler PropertyChanged;

    // Property for intX
    public int IntX
    {
        get { return intX; }
        private set
        {
            if (intX != value)
            {
                intX = value;
                // Call the PropertyChanged event whenever the intX changes
                OnPropertyChanged("IntX");
            }
        }
    }

    // Property for intY
    public int IntY
    {
        get { return intY; }
        private set
        {
            if (intY != value)
            {
                intY = value;
                // Call the PropertyChanged event whenever the intY changes
                OnPropertyChanged("IntY");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        int newIntX = Mathf.CeilToInt(Mathf.Clamp(point.x -1, 0, xmlParser.maxX));
        int newIntY = Mathf.CeilToInt(Mathf.Clamp(point.y, 0, xmlParser.maxY));

        // Update int coordinates only when they change
        if (newIntX != IntX)
        {
            IntX = newIntX;
        }

        if (newIntY != IntY)
        {
            IntY = newIntY;
        }
 
            if (xmlParser.regionDataMap != null)
            {
                //Debug.Log(x + "," + y + " | " + xmlParser.regionDataMap[x, y].name);
                currentName = xmlParser.regionDataMap[x, y].name;
                currentType = xmlParser.regionDataMap[x, y].type;
                currentEvilness = xmlParser.regionDataMap[x, y].evilness;
                regionPanel.Display(currentName, currentType, currentEvilness);
                //GameObject localRegion = xmlParser.regionMeshes[xmlParser.RegionCoordsToRegionIndex(x, y)];
                //if (oldObject != null)
                //{
                //    oldObject.transform.position = oldV3;
                //}
                //oldV3 = localRegion.transform.position;
                //localRegion.transform.position = new Vector3(oldV3.x, oldV3.y, oldV3.z + 5);
                //oldObject = localRegion.transform;
            }

    }//update

    // Method called whenever a property changes
    protected virtual void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    // Method to handle the change in intX
    private void OnIntXChanged()
    {
        // Your custom code here
        Debug.Log("intX changed: " + IntX);
    }

    // Method to handle the change in intY
    private void OnIntYChanged()
    {
        // Your custom code here
        Debug.Log("intY changed: " + IntY);
    }

    private void OnEnable()
    {
        // Subscribe the methods to the PropertyChanged event
       // PropertyChanged += MouseCoordinateTracker_PropertyChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe the methods from the PropertyChanged event
        PropertyChanged -= MouseCoordinateTracker_PropertyChanged;
    }
}
