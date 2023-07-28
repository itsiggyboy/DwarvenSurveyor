using UnityEngine;
public class CoordinateChangeHandler : MonoBehaviour
{
    public MapXMLParser xmlParser;
    string currentName;
    string currentType;
    string currentEvilness;
    public RegionPanel regionPanel;

    Transform lastObject;
    private void Start()
    {
        MouseCoords mousePositionDetector = GetComponent<MouseCoords>();
        mousePositionDetector.OnCoordinateChange += HandleCoordinateChange;
    }

    private void HandleCoordinateChange(Vector3Int newCoordinates)
    {
        // Your custom logic here
        // This method will be called whenever the integer coordinates change
        // Use the 'newCoordinates' parameter for the new integer coordinates
        //Debug.Log("Time " + Time.time  + " |    " + newCoordinates);

        if (xmlParser.regionDataMap != null)
        {
            Vector2Int coord = new Vector2Int(Mathf.Clamp(newCoordinates.x, 0, xmlParser.regionDataMap.GetLength(0) - 1), Mathf.Clamp(newCoordinates.y, 0, xmlParser.regionDataMap.GetLength(1) - 1));
            currentName = xmlParser.regionDataMap[coord.x,coord.y].name;
            currentType = xmlParser.regionDataMap[coord.x,coord.y].type;
            currentEvilness = xmlParser.regionDataMap[coord.x,coord.y].evilness;
            regionPanel.Display(currentName, currentType, currentEvilness);

            if (lastObject != null)
            {
                lastObject.transform.position = new Vector3(lastObject.transform.position.x, lastObject.transform.position.y, 0);
            }
            Transform currentRegion = xmlParser.regionMeshes[xmlParser.regionDataMap[coord.x, coord.y].index].transform;
            currentRegion.transform.position = new Vector3(currentRegion.transform.position.x, currentRegion.transform.position.y, 5);

            lastObject = currentRegion;
        }

        // Update is called once per frame
        //void Update()
        //{
        //    //point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        //    //x = Mathf.CeilToInt(Mathf.Clamp(point.x -1, 0, xmlParser.maxX));
        //    //y = Mathf.CeilToInt(Mathf.Clamp(point.y, 0, xmlParser.maxY));


        //    //    if (xmlParser.regionDataMap != null)
        //    //    {
        //    //        currentName = xmlParser.regionDataMap[x, y].name;
        //    //        currentType = xmlParser.regionDataMap[x, y].type;
        //    //        currentEvilness = xmlParser.regionDataMap[x, y].evilness;
        //    //        regionPanel.Display(currentName, currentType, currentEvilness);

        //    //    }

        //}//update
    }
}