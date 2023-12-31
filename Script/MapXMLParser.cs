using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;
using System.IO;
using System;
using System.Globalization;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;

public class MapXMLParser : MonoBehaviour
{
    public GameObject testCube;
    public SiteRenderInfo[] siteRenderInfo;
    public Material[] regionMaterials;//0wetland, 1forest, 2grassland, 3hills, 4desert, 5lake, 6tundra, 7glacier, 8ocean, 9mountains
    public Color[] regionColors;
    public List<GameObject> regionMeshes = new List<GameObject>();

    public GameObject targetReticle;
    public GameObject regionHighlight;

    public GameObject panelRegion;
    public GameObject xmlInfoPanel;
    public GameObject resetButton;
    public GameObject searchButton;

    public GameObject disclaimer;

    public GameObject searchButtonPrefab;
    public Transform searchScrollViewContent;
    public GameObject regionDivider;
    public GameObject siteDivider;

    public Transform semiTransparentQuad;

    public RegionData[,] regionDataMap;

    public SitePanel uiPanelScript;
    public GameObject siteObject;
    public string xmlPath;
    public string xmlPlusPath;

    public ErrorManager errorManager;
    public InputField xmlInputField;
    public InputField xmlPlusInputField;
    public Button generateButton;

    public CameraMover cameraMover;

    float highLightTimer;
    float highLightInterval = 0.5f;

    public Transform parent;
    public Transform regionParent;
    Transform regionColliderParent;
    Transform siteParent;

    public float regionScaleFactor = 1;
    public float siteScaleFactor = 0.9363f;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [SerializeField]
    public List<SiteData> sites = new List<SiteData>();
    [SerializeField]
    public List<RegionData> regions = new List<RegionData>();
    private void Start()
    {
        xmlPath = LoadPaths("xmlPath");
        xmlPlusPath = LoadPaths("xmlPlusPath");

        xmlInputField.text = xmlPath;
        xmlPlusInputField.text = xmlPlusPath;
    }

    private void Update()
    {
        if (regionHighlight != null)
        {
            highLightTimer += Time.deltaTime;
            if(highLightTimer > highLightInterval)
            {
                regionHighlight.SetActive(!regionHighlight.activeSelf);
                highLightTimer = 0;
            }
        }
        if (cameraMover.transform.position.x > maxX) { cameraMover.allowRight = false; }
        else { cameraMover.allowRight = true; }

        if (cameraMover.transform.position.x < minX) { cameraMover.allowLeft = false; }
        else { cameraMover.allowLeft = true; }

        if (cameraMover.transform.position.y > maxY) { cameraMover.allowUp = false; }
        else { cameraMover.allowUp = true; }

        if (cameraMover.transform.position.y < minY) { cameraMover.allowDown = false; }
        else { cameraMover.allowDown = true; }
    }

    public void Reset()
    {
        minX = 0;
        maxX = 0;
        minY = 0;
        maxY = 0;
        regionMeshes.Clear();
        sites.Clear();
        regions.Clear();
        disclaimer.SetActive(true);
        Destroy(transform.GetChild(0).gameObject);

    }

    public void SearchText(string s)
    {
        Debug.Log("Searching... " + s);

        s.ToLower();
        
        List<SiteData> compatibleSites = new List<SiteData>();
        List<RegionData> compatibleRegions = new List<RegionData>();

        compatibleSites.Clear();
        compatibleRegions.Clear();

        foreach (Transform child in searchScrollViewContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < sites.Count; i++)
        {
            if(sites[i].name.ToLower().Contains(s))
            {
                compatibleSites.Add(sites[i]);
            }
        }
        for (int i = 0; i < regions.Count; i++)
        {
            if (regions[i].name.ToLower().Contains(s))
            {
                compatibleRegions.Add(regions[i]);
            }
        }

        // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        GameObject siteDividerObj = Instantiate(siteDivider, searchScrollViewContent);
        for (int i = 0; i < compatibleSites.Count; i++)
        {
            GameObject siteButton = Instantiate(searchButtonPrefab, searchScrollViewContent);
            siteButton.name = compatibleSites[i].name + "-searchresult";
            siteButton.transform.GetChild(0).GetComponent<Text>().text = compatibleSites[i].name;
            SearchButtonCameraJump cameraJumpScript = siteButton.GetComponent<SearchButtonCameraJump>();
            cameraJumpScript.positionToJumpTo = compatibleSites[i].realPosition;
            cameraJumpScript.targetReticle = targetReticle;
            cameraJumpScript.region = false;
            cameraJumpScript.parser = this;
        }
        GameObject regionDividerObj = Instantiate(regionDivider, searchScrollViewContent);
        for (int i = 0; i < compatibleRegions.Count; i++)
        {
            GameObject regionButton = Instantiate(searchButtonPrefab, searchScrollViewContent);
            regionButton.name = compatibleRegions[i].name + "-searchresult";
            regionButton.transform.GetChild(0).GetComponent<Text>().text = compatibleRegions[i].name;
            SearchButtonCameraJump cameraJumpScript = regionButton.GetComponent<SearchButtonCameraJump>();
            cameraJumpScript.positionToJumpTo = compatibleRegions[i].realPosition;
            cameraJumpScript.targetReticle = targetReticle;
            cameraJumpScript.region = true;
            cameraJumpScript.parser = this;
            cameraJumpScript.indexToJumpTo = compatibleRegions[i].index;

        }
        float buttonHeight = searchButtonPrefab.GetComponent<RectTransform>().sizeDelta.y;

        searchScrollViewContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, buttonHeight * (compatibleSites.Count + compatibleRegions.Count));
    }

    public void CheckInputFields()
    {
        bool xmlOk = false;
        bool xmlPlusOk = false;

        if (xmlInputField.text != null && xmlInputField.text != "")
        {
            if (File.Exists(xmlInputField.text))
            {
                xmlOk = true;
            }
        }
        if (xmlPlusInputField.text != null && xmlPlusInputField.text != "")
        {
            if (File.Exists(xmlPlusInputField.text))
            {
                xmlPlusOk = true;
            }
        }

        if (xmlOk && xmlPlusOk)
        {
            generateButton.interactable = true;
        }
        else
        {
            generateButton.interactable = false;
        }
    }
    public void Generate()
    {
        Debug.Log("Attempting generation...");
        xmlPath = xmlInputField.text;
        xmlPlusPath = xmlPlusInputField.text;


        if (!File.Exists(xmlInputField.text))
        {
            errorManager.GenerateError("XML path doesn't exist.", Color.red);
        }
        if (!File.Exists(xmlPlusInputField.text))
        {
            errorManager.GenerateError("XML plus path doesn't exist.", Color.red);
        }
        SavePaths();

        ParseXML();
    }
    void ParseXML()
    {
        Debug.Log("Parsing XMLs...");
        bool regularXmlSiteError = false;
        bool regularXmlRegionError = false;
        bool XmlPlusRegionError = false;
        List<RegionDataPlus> regionsPlus = new List<RegionDataPlus>();
        List<RegionDataNormal> regionsNormal = new List<RegionDataNormal>();

        XmlReader reader = XmlReader.Create(new StreamReader(xmlPath, System.Text.Encoding.UTF8));
        bool isInsideSiteElement = false;
        bool isInsideStructureElement = false;

        //SITE REGULAR XML
        SiteData sData = new SiteData();
        // Iterate over the nodes in the XML document.

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name == "site")
                {
                    isInsideSiteElement = true;
                    sData = new SiteData();
                    // Process attributes of the "site" element if needed.
                    // e.g., if the "site" element has an "id" attribute:
                    // int id = int.Parse(reader.GetAttribute("id"));
                }
                else if (reader.Name == "structure")
                {
                    isInsideStructureElement = true;
                }
                else if (isInsideSiteElement && !isInsideStructureElement)
                {
                    switch (reader.Name)
                    {
                        case "type":
                            sData.type = ConvertWordsToTitleCase(reader.ReadElementContentAsString());
                            break;

                        case "name":
                            sData.name = ConvertWordsToTitleCase(reader.ReadElementContentAsString());
                            break;

                        case "coords":
                            reader.ReadStartElement();
                            string coordString = reader.Value;
                            string[] coordArray = coordString.Split(',');
                            sData.coord = new Vector2Int(int.Parse(coordArray[0]), int.Parse(coordArray[1]));
                            break;

                        case "rectangle":
                            reader.ReadStartElement();
                            string rectString = reader.Value;
                            string[] rectCoords = rectString.Split(':', ',');
                            int xMin = int.Parse(rectCoords[0]);
                            int yMin = int.Parse(rectCoords[1]);
                            int xMax = int.Parse(rectCoords[2]);
                            int yMax = int.Parse(rectCoords[3]);
                            sData.rectangle = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
                            break;
                    }
                }
            }

            if (reader.NodeType == XmlNodeType.EndElement)
            {
                if (reader.Name == "site")
                {
                    sites.Add(sData);
                    isInsideSiteElement = false;
                    sData = null;
                }
                else if (reader.Name == "structure")
                {
                    isInsideStructureElement = false;
                }
            }
        }

        if (sites.Count < 1)
        {
            errorManager.GenerateError("XML contains " + sites.Count + " sites. You probably are trying to use the wrong XML file.", Color.red);
            regularXmlSiteError = true;
        }
        for (int i = 0; i < sites.Count; i++)
        {
            sites[i].typeIndex = SiteTypeStringToTypeIndex(sites[i].type);
        }


        //REGION REGULAR XML
        XmlReader readerRegion = XmlReader.Create(new StreamReader(xmlPath, System.Text.Encoding.UTF8));
        RegionDataNormal rDataNormal = new RegionDataNormal();
        bool isInsideRegionElement = false;
        // Iterate over the nodes in the XML document.

        while (readerRegion.Read())
        {
            // Check the node type.
            if (readerRegion.NodeType == XmlNodeType.Element)
            {

                if (readerRegion.Name == "region")
                {
                    isInsideRegionElement = true;
                    rDataNormal = new RegionDataNormal();
                } else if (isInsideRegionElement)
                {
                    switch (readerRegion.Name)
                    {
                        case "name":
                            rDataNormal.name = readerRegion.ReadElementContentAsString();
                            break;
                        case "type":
                            rDataNormal.type = readerRegion.ReadElementContentAsString(); ;
                            break;
                        default:
                            // Do nothing.
                            break;
                    }
                }

            }
            if (readerRegion.NodeType == XmlNodeType.EndElement)
            {
                if (readerRegion.Name == "region")
                {
                    regionsNormal.Add(rDataNormal);
                    isInsideRegionElement = false;
                }
            }
        }

        if (regionsNormal.Count < 1)
        {
            //No regions.
            errorManager.GenerateError("XML contains " + regionsNormal.Count + " regions. You probably are trying to use the wrong XML file.", Color.red);
            regularXmlRegionError = true;
        }

        //SITE XML PLUS
        XmlReader readerPlus = XmlReader.Create(new StreamReader(xmlPlusPath, System.Text.Encoding.ASCII));

        RegionDataPlus rDataPlus = new RegionDataPlus();

        bool isInsideRegionPlus = false;
        // Iterate over the nodes in the XML document.
        while (readerPlus.Read())
        {
            // Check the node type.
            if (readerPlus.NodeType == XmlNodeType.Element)
            {
                // Get the node name.
                if (readerPlus.Name == "region")
                {
                    isInsideRegionPlus = true;
                    rDataPlus = new RegionDataPlus();
                } else if (isInsideRegionPlus)
                {
                    switch (readerPlus.Name)
                    {
                        case "evilness":
                            rDataPlus.evilness = readerPlus.ReadElementContentAsString();
                            break;
                        case "coords":
                            string coordString = readerPlus.ReadElementContentAsString();
                            rDataPlus.coords = ParseCoordinates(coordString);
                            break;
                        default:
                            // Do nothing.
                            break;
                    }
                }
            }
            if (readerPlus.NodeType == XmlNodeType.EndElement)
            {
                if (readerPlus.Name == "region")
                {
                    regionsPlus.Add(rDataPlus);
                    isInsideRegionPlus = false;
                }
            }
        }
        Debug.Log("---------------------------------------------------------------------------------------------------");

        if (regionsPlus.Count < 1)
        {
            string errorText = "XML plus contains " + regionsPlus.Count + " regions. You probably are trying to use the wrong XML file.";
            errorManager.GenerateError(errorText, Color.red);
            Debug.LogError(errorText);
            XmlPlusRegionError = true;
        }

        if (regularXmlRegionError || regularXmlSiteError || XmlPlusRegionError)
        {
            errorManager.GenerateError("Not enough data to build map. This is probably due to an incorrect XML file being provided.", Color.red);
            return;
        }

        //errorManager.GenerateError("Regions normal is " + regionsNormal.Count + " regions. Regions plus is " + regionsPlus.Count, Color.red);

        for (int i = 0; i < regionsNormal.Count; i++)
        {
            RegionData rData = new RegionData();
            rData.name = ConvertWordsToTitleCase(regionsNormal[i].name);
            rData.type = regionsNormal[i].type;
            rData.index = i;
            rData.typeIndex = StringRegionTypeToInt(regionsNormal[i].type);
            rData.coords = regionsPlus[i].coords;
            //Debug.Log("Adding coords to " + rData.name + regionsPlus[i].coords);
            rData.evilness = regionsPlus[i].evilness;
            regions.Add(rData);
        }

        InstantiateSites();

        for (int i = 0; i < regions.Count; i++)
        {
            if (regions[i].coords.Length < 10000)//If the mesh has under 10.000 vertices, put it in a single mesh. Otherwise, put it in 4 separate meshes.
            {
                GameObject go = GenerateRegionMesh(i, regions[i].coords, regions[i].name, regions[i].type);

                if (parent != null)
                {
                    if (regionParent == null)
                    {
                        GameObject regionParentLocal = new GameObject();
                        regionParentLocal.transform.parent = parent;
                        regionParentLocal.name = "Regions";
                        regionParent = regionParentLocal.transform;
                        go.transform.parent = regionParent.transform;
                    }
                    else
                    {
                        go.transform.parent = regionParent.transform;
                    }
                }
                regionMeshes.Add(go);
            }
            else
            {
                Vector2Int[][] separatedArrays = SeparateArray(regions[i].coords);
                GameObject fourMeshes = new GameObject();
                GameObject mesh1 = GenerateRegionMesh(i, separatedArrays[0], regions[i].name, regions[i].type);
                mesh1.transform.parent = fourMeshes.transform;
                GameObject mesh2 = GenerateRegionMesh(i, separatedArrays[1], regions[i].name, regions[i].type);
                mesh2.transform.parent = fourMeshes.transform;
                GameObject mesh3 = GenerateRegionMesh(i, separatedArrays[2], regions[i].name, regions[i].type);
                mesh3.transform.parent = fourMeshes.transform;
                GameObject mesh4 = GenerateRegionMesh(i, separatedArrays[3], regions[i].name, regions[i].type);
                mesh4.transform.parent = fourMeshes.transform;

                if (parent != null)
                {
                    fourMeshes.name = mesh1.name;
                    if (regionParent == null)
                    {
                        GameObject regionParentLocal = new GameObject();
                        regionParentLocal.transform.parent = parent;
                        regionParentLocal.name = "Regions";
                        regionParent = regionParentLocal.transform;
                        fourMeshes.transform.parent = regionParent.transform;
                    }
                    else
                    {
                        fourMeshes.transform.parent = regionParent.transform;
                    }
                }
                regionMeshes.Add(fourMeshes);
            }

            for (int u = 0; u < sites.Count; u++)
            {
                Vector2 siteOffset = new Vector2(-0.25f, 0.75f);
                Vector2 sitePos = sites[u].rectangle.center /16;
                sitePos.y = maxY - sitePos.y;
                sites[u].realPosition = sitePos + siteOffset;
            } 

           
        }//Create Region Meshes

        regionDataMap = new RegionData[(int)maxX + 1, (int)maxY + 1];



        semiTransparentQuad.transform.position = new Vector3(maxX / 2, maxY / 2, 2.5f);
        semiTransparentQuad.transform.localScale = new Vector3(maxX - minX, maxY - minY, 1);
        //Debug.Log("Region map created with x " + (int)maxX + ",y " + (int)maxY);
        for (int i = 0; i < regions.Count; i++)
        {
            //Debug.Log("--------Scanning region " + i + " - " + regions[i].name);
            for (int u = 0; u < regions[i].coords.Length; u++)
            {
                //Debug.Log("-Coord " + u + " | " + "regionDataMap[" + regions[i].coords[u].x + "," + regions[i].coords[u].y + "]");
                RegionData rd = new RegionData();
                rd.name = regions[i].name;
                rd.type = regions[i].type;
                rd.evilness = regions[i].evilness;
                rd.coords = regions[i].coords;
                rd.index = regions[i].index;
                regionDataMap[regions[i].coords[u].x, regions[i].coords[u].y] = rd;

            }
        }//Assigns region data to each tile
        regionDataMap = InvertRegionDataArrayOnYAxis(regionDataMap);
        //Array.Reverse(regionDataMap);

        if (regionParent != null)
        {
            regionParent.eulerAngles = new Vector3(180, 0, 0);
            regionParent.transform.position = new Vector3(-0.5f, maxY + 0.5f, 0);
            regionParent.transform.localScale = new Vector3(regionScaleFactor, regionScaleFactor, regionScaleFactor);
        }
        if (siteParent != null)
        {
            siteParent.eulerAngles = new Vector3(180, 0, 0);
            siteParent.transform.localPosition = new Vector3(-0.5f, maxY + 0.5f, -0.1f);
            siteParent.transform.localScale = new Vector3(siteScaleFactor, siteScaleFactor, siteScaleFactor);


        }

        FindCenters();
        xmlInfoPanel.SetActive(false);
        uiPanelScript.gameObject.SetActive(true);
        semiTransparentQuad.gameObject.SetActive(true);
        disclaimer.SetActive(false);

        cameraMover.transform.position = new Vector3(maxX/2, maxY/2, -20);
        resetButton.SetActive(true);
        searchButton.SetActive(true);
        panelRegion.SetActive(true);
    }//Parse XML

    private int SiteTypeStringToTypeIndex(string typeString)
    {
            int typeIndex = -1;
            switch (typeString)
            {
                case "Camp":
                    typeIndex = 0;
                    break;
                case "Cave":
                    typeIndex = 1;
                    break;
                case "Dark Fortress":
                    typeIndex = 2;
                    break;
                case "Dark Pits":
                    typeIndex = 3;
                    break;
                case "Forest Retreat":
                    typeIndex = 4;
                    break;
                case "Fortress":
                    typeIndex = 5;
                    break;
                case "Castle":
                    typeIndex = 6;
                    break;
                case "Fort":
                    typeIndex = 7;
                    break;
                case "Hamlet":
                    typeIndex = 8;
                    break;
                case "Hillocks":
                    typeIndex = 9;
                    break;
                case "Labyrinth":
                    typeIndex = 10;
                    break;
                case "Lair":
                    typeIndex = 11;
                    break;
                case "Monastery":
                    typeIndex = 12;
                    break;
                case "Mountain Halls":
                    typeIndex = 13;
                    break;
                case "Ruins":
                    typeIndex = 14;
                    break;
                case "Shrine":
                    typeIndex = 15;
                    break;
                case "Tomb":
                    typeIndex = 16;
                    break;
                case "Tower":
                    typeIndex = 17;
                    break;
                case "Town":
                    typeIndex = 18;
                    break;
                case "Vault":
                    typeIndex = 19;
                    break;
        }
        return typeIndex;
    }

    private Vector2Int[][] SeparateArray(Vector2Int[] originalArray)
    {
        int numArrays = 4;
        int arraySize = Mathf.CeilToInt((float)originalArray.Length / numArrays);

        Vector2Int[][] separatedArrays = new Vector2Int[numArrays][];

        for (int i = 0; i < numArrays; i++)
        {
            int startIndex = i * arraySize;
            int length = Mathf.Min(arraySize, originalArray.Length - startIndex);

            separatedArrays[i] = new Vector2Int[length];
            System.Array.Copy(originalArray, startIndex, separatedArrays[i], 0, length);
        }

        return separatedArrays;
    }

    public void InstantiateSites()
    {
        Debug.Log("Instantiating sites...");
        if (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
        if (sites.Count > 0)
        {
            GameObject parentLocal = new GameObject();
            parentLocal.transform.parent = transform;
            parentLocal.name = "Parent";
            parent = parentLocal.transform;

            if(siteParent == null)
            {
                GameObject siteParentLocal = new GameObject();
                siteParentLocal.name = "Sites";
                siteParentLocal.transform.parent = parent;
                siteParent = siteParentLocal.transform;
            }

            for (int i = 0; i < sites.Count; i++)
            {
                GameObject instantiatedSite = Instantiate(siteObject);
                instantiatedSite.name = "Site | " + sites[i].name;
                instantiatedSite.transform.parent = siteParent.transform;

                Site _site = instantiatedSite.transform.GetChild(0).GetComponent<Site>();

                _site.name = sites[i].name;
                _site.type = sites[i].type;
                _site.coord = sites[i].coord;
                _site.uiPanel = uiPanelScript;

                //Debug.Log("Attempting " + _site.type);
                if(sites[i].typeIndex == -1)
                {
                    //Debug.LogError("site " + i + " (" + sites[i].type + ") has a value of -1.");
                    //errorManager.GenerateError("site " + i + " (" + sites[i].type + ") has a value of -1.",Color.red);
                }
                else
                {

                    _site.spriteR.color = siteRenderInfo[sites[i].typeIndex].color;
                    Vector3 scale = new Vector3(sites[i].rectangle.width, sites[i].rectangle.height, 0);

                    if(scale.x == 0)
                    {
                        scale.x = 1;
                    }
                    if(scale.y == 0)
                    {
                        scale.y = 1;
                    }
                    instantiatedSite.transform.localScale = scale;

                    instantiatedSite.transform.position = new Vector3(sites[i].rectangle.x, sites[i].rectangle.y, -1);
                }
            }
        }
    }//InstantiateSites


    public Color GetRandomColor()
    {
        float r = UnityEngine.Random.Range(0f, 1f);
        float g = UnityEngine.Random.Range(0f, 1f);
        float b = UnityEngine.Random.Range(0f, 1f);

        return new Color(r, g, b);
    }

    private GameObject GenerateRegionMesh(int regionIndex, Vector2Int[] coordinates, string name, string type)
    {
        // Create a new mesh
        Mesh mesh = new Mesh();
        Vector2 topLeft = Vector2.zero;
        Vector2 bottomRight = Vector2.zero;
        // Vertices
        Vector3[] vertices = new Vector3[coordinates.Length * 4];
        int vertexIndex = 0;
        // Vertices
        for (int i = 0; i < coordinates.Length; i++)
        {
            Vector2Int coordinate = coordinates[i];

            // Create a square at each coordinate
            float x = coordinate.x;
            float y = 0;
            float z = coordinate.y;

            // Define the vertices of the square
            vertices[vertexIndex++] = new Vector3(x, y, z);
            vertices[vertexIndex++] = new Vector3(x, y, z + 1);
            vertices[vertexIndex++] = new Vector3(x + 1, y, z + 1);
            vertices[vertexIndex++] = new Vector3(x + 1, y, z);
        }

        // Triangles
        int[] triangles = new int[coordinates.Length * 6];
        int triangleIndex = 0;
        for (int i = 0; i < coordinates.Length; i++)
        {
            int vertexOffset = i * 4;

            // Define the triangles of the square
            triangles[triangleIndex++] = vertexOffset;
            triangles[triangleIndex++] = vertexOffset + 1;
            triangles[triangleIndex++] = vertexOffset + 2;
            triangles[triangleIndex++] = vertexOffset;
            triangles[triangleIndex++] = vertexOffset + 2;
            triangles[triangleIndex++] = vertexOffset + 3;


            if (coordinates[i].x < minX)
            {
                minX = coordinates[i].x;
            }
            if (coordinates[i].x > maxX)
            {
                maxX = coordinates[i].x;
            }
            if (coordinates[i].y < minY)
            {
                minY = coordinates[i].y;
            }
            if (coordinates[i].y > maxY)
            {
                maxY = coordinates[i].y;
            }

        }

        // Assign vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Recalculate normals and bounds
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Create a new game object with a mesh renderer and filter
        GameObject meshObject = new GameObject("FlatMesh");
        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();

        meshObject.name = "Region | " + name;
        meshObject.transform.localScale = new Vector3(1, 1, 1);
        // Assign the generated mesh to the filter
        meshFilter.mesh = mesh;
        meshObject.transform.eulerAngles = new Vector3(-90, 0, 0);
        meshRenderer.material = regionMaterials[StringRegionTypeToInt(type)];

        FlipNormals(meshFilter);



        return meshObject;
    }//GenerateMesh

    public void DestroySearchHighlight()
    {
        if (regionHighlight == null)
        {
            return;
        }
        Destroy(regionHighlight);
    }

    public void FindCenters()
    {
        Debug.Log("Finding centers...");
        for (int i = 0; i < regionMeshes.Count; i++)
        {
            Mesh mesh;
            if (regionMeshes[i].GetComponent<MeshFilter>() == null)
            {
                //Debug.LogError("Region Mesh " + i + " (" + regions[i].name + ") has no mesh filter");
                //return;
                mesh = regionMeshes[i].transform.GetChild(0).GetComponent<MeshFilter>().mesh;
            }
            else
            {
                mesh = regionMeshes[i].GetComponent<MeshFilter>().mesh;
            }

            Vector3[] vertices = mesh.vertices;

            Vector3 topLeft = new Vector2(maxX, 0);
            Vector3 bottomRight = new Vector2(0, maxY);

            Vector3[] verticesFixedYFlipping = mesh.vertices;

            for (int u = 0; u < verticesFixedYFlipping.Length; u++)
            {
                verticesFixedYFlipping[u] = new Vector3(vertices[u].x, maxY - vertices[u].z, 0);
            }

            for (int u = 0; u < vertices.Length; u++)
            {

                Vector3 coordinate = verticesFixedYFlipping[u];

                Debug.Log("Coordinates: " + coordinate);

                // Update topLeft
                topLeft.x = Mathf.Min(topLeft.x, coordinate.x);
                topLeft.y = Mathf.Max(topLeft.y, coordinate.y);

                // Update bottomRight
                bottomRight.x = Mathf.Max(bottomRight.x, coordinate.x);
                bottomRight.y = Mathf.Min(bottomRight.y, coordinate.y);
            }
            Vector3 offset = new Vector3(-0.5f, 0.5f, 0);
            //topLeftObj.transform.position = topLeft + offset;
            //bottomRightObj.transform.position = bottomRight + offset;
            Vector3 centerWorldPosition = CalculateRectangleCenter(topLeft, bottomRight);

            regions[i].realPosition = centerWorldPosition + offset;
        }
        Debug.Log("Done finding centers.");
    }

    Vector2 CalculateRectangleCenter(Vector2 topLeft, Vector2 bottomRight)
    {
        float centerX = (topLeft.x + bottomRight.x) / 2.0f;
        float centerY = (topLeft.y + bottomRight.y) / 2.0f;

        return new Vector2(centerX, centerY);
    }

    Vector2 CalculateBoundingBoxCenter(Vector2 topLeft, Vector2 bottomRight)
    {
        float centerX = Mathf.Lerp(topLeft.x, bottomRight.x, 0.5f);
        float centerY = Mathf.Lerp(topLeft.y, bottomRight.y, 0.5f);

        return new Vector2(centerX, centerY);
    }

    public int RegionCoordsToRegionIndex(int x, int y)
    {
        return regionDataMap[x, y].index;
    }
    private void FlipNormals(MeshFilter meshFilter)
    {
        // Check if the MeshFilter component and mesh exist
        if (meshFilter != null && meshFilter.mesh != null)
        {
            // Get the mesh
            Mesh mesh = meshFilter.mesh;

            // Reverse the normals
            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -normals[i];
            }
            mesh.normals = normals;

            // Reverse the winding order of the triangles
            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int temp = triangles[i];
                triangles[i] = triangles[i + 2];
                triangles[i + 2] = temp;
            }
            mesh.triangles = triangles;

            // Recalculate the bounds and normals
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
    }


    private void CreateRegionColliders()
    {
        if(regionColliderParent == null)
        {
            GameObject regionColliderParentLocal = new GameObject();
            regionColliderParentLocal.transform.parent = parent;
            regionColliderParent = regionColliderParentLocal.transform;
            regionColliderParentLocal.name = "Region Colliders";
        }
        for (int i = 0; i < regions.Count; i++)
        {
            for (int u = 0; u < regions[i].coords.Length; u++)
            {
                GameObject regionCol = new GameObject();
                regionCol.name = (regions[i].coords[u].x + "," + regions[i].coords[u].y + " " + regions[i].name);
                regionCol.AddComponent<Region>();
                regionCol.AddComponent<BoxCollider2D>();
                regionCol.transform.position = new Vector3(regions[i].coords[u].x, regions[i].coords[u].y, 0);
                Region region = regionCol.GetComponent<Region>();
                region.name = regions[i].name;
                region.type = regions[i].type;
                region.uiPanel = uiPanelScript;
                regionCol.transform.parent = regionColliderParent;
            }
        }
        regionColliderParent.parent = regionParent;
        regionColliderParent.transform.localPosition = new Vector3(0.5f, 0.5f, 0);
        regionColliderParent.localRotation = Quaternion.identity;
        regionColliderParent.localScale = new Vector3(1, 1, 1);
    }
   
    public int StringRegionTypeToInt(string type)
    {
        //0wetland, 1forest, 2grassland, 3hills, 4desert, 5lake, 6tundra, 7glacier, 8ocean, 9mountains
        int i = 10;
        switch (type)
        {
            case "Wetland":
                i = 0;
                break;
            case "Forest":
                i = 1;
                break;
            case "Grassland":
                i = 2;
                break;
            case "Hills":
                i = 3;
                break;
            case "Desert":
                i = 4;
                break;
            case "Lake":
                i = 5;
                break;
            case "Tundra":
                i = 6;
                break;
            case "Glacier":
                i = 7;
                break;
            case "Ocean":
                i = 8;
                break;
            case "Mountains":
                i = 9;
                break;
        }
        return i;
    }

    public void SavePaths()
    {
        PlayerPrefs.SetString("xmlPath", xmlPath);
        PlayerPrefs.SetString("xmlPlusPath", xmlPlusPath);
        PlayerPrefs.Save();
    }

    public string LoadPaths(string key)
    {
        if(PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetString(key);
        }
        else
        {
            Debug.Log("No saved string found for " + key);
            return string.Empty;
        }
    }

    RegionData[,] InvertRegionDataArrayOnYAxis(RegionData[,] originalArray)
    {
        int width = originalArray.GetLength(0);
        int height = originalArray.GetLength(1);

        RegionData[,] invertedArray = new RegionData[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int invertedY = height - 1 - y; // Invert the Y-axis index

                invertedArray[x, invertedY] = originalArray[x, y];
            }
        }

        return invertedArray;
    }
    public Vector2Int[] ParseCoordinates(string rawCoordinates)
    {
        string[] coordinateStrings = rawCoordinates.Split('|');

        Vector2Int[] coordinates = new Vector2Int[coordinateStrings.Length - 1];

        for (int i = 0; i < coordinateStrings.Length -1; i++)
        {
            string[] values = coordinateStrings[i].Split(',');

            int x = int.Parse(values[0]);
            int y = int.Parse(values[1]);

            coordinates[i] = new Vector2Int(x, y);
        }

        return coordinates;
    }

    public static string ConvertWordsToTitleCase(string input)
    {
        // Check for null or empty input
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Initialize a CultureInfo for title casing
        CultureInfo cultureInfo = CultureInfo.CurrentCulture;

        // Split the input into individual words
        string[] words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        // Convert the first letter of each word to uppercase
        for (int i = 0; i < words.Length; i++)
        {
            // Check if the word is not empty
            if (!string.IsNullOrEmpty(words[i]))
            {
                // Convert the first letter to uppercase and append the rest of the word
                words[i] = char.ToUpper(words[i][0], cultureInfo) + words[i].Substring(1);
            }
        }

        // Join the words back together with spaces
        string result = string.Join(" ", words);

        return result;
    }


}//Class



[System.SerializableAttribute]
public class SiteData
{
    public string name;
    public string type;
    public int typeIndex;
    public int index;
    public Vector2Int coord;
    public Vector2 realPosition;
    public Rect rectangle;
}

[System.SerializableAttribute]
public class RegionData
{
    public string name;
    public string type;
    public string evilness;
    public int typeIndex;
    public int index;
    public Vector2Int[] coords;
    public Vector2 realPosition;
}

public class RegionDataNormal
{
    public string name;
    public string type;
}
public class RegionDataPlus
{
    public string evilness;
    public Vector2Int[] coords;
}

[System.SerializableAttribute]
public class SiteRenderInfo
{
    public string name;
    public Color color;
    public Sprite sprite;
}
