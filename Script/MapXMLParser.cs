using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;
using System.IO;
using System;

public class MapXMLParser : MonoBehaviour
{
    public SiteRenderInfo[] siteRenderInfo;
    public Material[] regionMaterials;//0wetland, 1forest, 2grassland, 3hills, 4desert, 5lake, 6tundra, 7glacier, 8ocean, 9mountains
    public Material outlineMaterial;
    public Color[] regionColors;

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

    public Transform parent;
    Transform regionParent;
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
        if (cameraMover.transform.position.x > maxX) { cameraMover.allowRight = false; }
        else { cameraMover.allowRight = true; }

        if (cameraMover.transform.position.x < minX) { cameraMover.allowLeft = false; }
        else { cameraMover.allowLeft = true; }

        if (cameraMover.transform.position.y > maxY) { cameraMover.allowUp = false; }
        else { cameraMover.allowUp = true; }

        if (cameraMover.transform.position.y < minY) { cameraMover.allowDown = false; }
        else { cameraMover.allowDown = true; }
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
        List<RegionDataPlus> regionsPlus = new List<RegionDataPlus>();
        List<RegionDataNormal> regionsNormal = new List<RegionDataNormal>();

        //Regular XML

        XmlDocument xml = new XmlDocument();
        xml.Load(xmlPath);

        XmlNodeList siteNodes = xml.GetElementsByTagName("site");

        foreach (XmlNode siteNode in siteNodes)
        {
            SiteData data = new SiteData();

            data.name = siteNode.SelectSingleNode("name").InnerText;
            data.type = siteNode.SelectSingleNode("type").InnerText;

            int typeIndex = -1;
            switch (data.type)
            {
                case "camp":
                    typeIndex = 0;
                    break;
                case "cave":
                    typeIndex = 1;
                    break;
                case "dark fortress":
                    typeIndex = 2;
                    break;
                case "dark pits":
                    typeIndex = 3;
                    break;
                case "forest retreat":
                    typeIndex = 4;
                    break;
                case "fortress":
                    typeIndex = 5;
                    break;
                case "castle":
                    typeIndex = 6;
                    break;
                case "fort":
                    typeIndex = 7;
                    break;
                case "hamlet":
                    typeIndex = 8;
                    break;
                case "hillocks":
                    typeIndex = 9;
                    break;
                case "labyrinth":
                    typeIndex = 10;
                    break;
                case "lair":
                    typeIndex = 11;
                    break;
                case "monastery":
                    typeIndex = 12;
                    break;
                case "mountain halls":
                    typeIndex = 13;
                    break;
                case "ruins":
                    typeIndex = 14;
                    break;
                case "shrine":
                    typeIndex = 15;
                    break;
                case "tomb":
                    typeIndex = 16;
                    break;
                case "tower":
                    typeIndex = 17;
                    break;
                case "town":
                    typeIndex = 18;
                    break;
                case "vault":
                    typeIndex = 19;
                    break;
            }
            data.typeIndex = typeIndex;

            string coordString = siteNode.SelectSingleNode("coords").InnerText;
            string[] coordArray = coordString.Split(',');
            data.coord = new Vector2Int(int.Parse(coordArray[0]), int.Parse(coordArray[1]));

            XmlNode rectNode = siteNode.SelectSingleNode("rectangle");
            string rectString = rectNode.InnerText;
            string[] rectCoords = rectString.Split(':', ',');

            int xMin = int.Parse(rectCoords[0]);
            int yMin = int.Parse(rectCoords[1]);
            int xMax = int.Parse(rectCoords[2]);
            int yMax = int.Parse(rectCoords[3]);

            data.rectangle = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);

            sites.Add(data);
        }

        XmlNodeList regionNodes = xml.GetElementsByTagName("region");

        foreach (XmlNode regionNodeNormal in regionNodes)
        {
            RegionDataNormal data = new RegionDataNormal();

            XmlNode nameNode = regionNodeNormal.SelectSingleNode("name");
            data.name = nameNode.InnerText;

            XmlNode typeNode = regionNodeNormal.SelectSingleNode("type");
            data.type = typeNode.InnerText;

            regionsNormal.Add(data);
        }

        //XML Plus

        XmlDocument xmlPlus = new XmlDocument();
        xmlPlus.Load(xmlPlusPath);

        XmlNodeList regionNodesPlus = xmlPlus.GetElementsByTagName("region");

        foreach (XmlNode regionNodePlus in regionNodesPlus)
        {
            RegionDataPlus data = new RegionDataPlus();

            XmlNode evilnessNode = regionNodePlus.SelectSingleNode("evilness");
            data.evilness = evilnessNode.InnerText;

            XmlNode coordsNode = regionNodePlus.SelectSingleNode("coords");
            string rawCoords = coordsNode.InnerText;

            //Debug.Log("Trying to parse " + rawCoords);
            data.coords = ParseCoordinates(rawCoords);

            regionsPlus.Add(data);
        }
        //Debug.Log("Normal count " + regionsNormal.Count);
        //Debug.Log("Plus count " + regionsPlus.Count);

        for (int i = 0; i < regionsNormal.Count; i++)
        {
            RegionData data = new RegionData();
            data.name = regionsNormal[i].name;
            data.type = regionsNormal[i].type;
            data.coords = regionsPlus[i].coords;
            data.evilness = regionsPlus[i].evilness;
            regions.Add(data);
        }

        InstantiateSites();

        for (int i = 0; i < regions.Count; i++)
        {
            if(regions[i].coords.Length < 10000)
            {
                GenerateRegionMesh(i,regions[i].coords, regions[i].name, regions[i].type);
            }
            else
            {
                Vector2Int[][] separatedArrays = SeparateArray(regions[i].coords);

                GenerateRegionMesh(i,separatedArrays[0], regions[i].name, regions[i].type);
                GenerateRegionMesh(i,separatedArrays[1], regions[i].name, regions[i].type);
                GenerateRegionMesh(i,separatedArrays[2], regions[i].name, regions[i].type);
                GenerateRegionMesh(i,separatedArrays[3], regions[i].name, regions[i].type);
            }
        }//Create Region Meshes

        regionDataMap = new RegionData[(int)maxX + 1, (int)maxY + 1];
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
                rd.material = regions[i].material;
                regionDataMap[regions[i].coords[u].x, regions[i].coords[u].y] = rd;

            }
        }//Assigns region data to each tile
        regionDataMap = InvertRegionDataArrayOnYAxis(regionDataMap);
        //Array.Reverse(regionDataMap);

        if (regionParent != null)
        {
            regionParent.eulerAngles = new Vector3(180, 0, 0);
            regionParent.transform.position = new Vector3(0, maxY, 0);
            regionParent.transform.localScale = new Vector3(regionScaleFactor, regionScaleFactor, regionScaleFactor);
        }
        if (siteParent != null)
        {
            siteParent.eulerAngles = new Vector3(180, 0, 0);
            siteParent.transform.localPosition = new Vector3(0, maxY, -0.1f);
            siteParent.transform.localScale = new Vector3(siteScaleFactor, siteScaleFactor, siteScaleFactor);
        }
    }//Parse XML
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
                _site.spriteR.color = siteRenderInfo[sites[i].typeIndex].color;
                instantiatedSite.transform.localScale = new Vector3(sites[i].rectangle.width, sites[i].rectangle.height, 0);
                instantiatedSite.transform.position = new Vector3(sites[i].rectangle.x, sites[i].rectangle.y, -1);
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

    private void GenerateRegionMesh(int regionIndex, Vector2Int[] coordinates, string name, string type)
    {
        // Create a new mesh
        Mesh mesh = new Mesh();

        // Vertices
        Vector3[] vertices = new Vector3[coordinates.Length * 4];
        int vertexIndex = 0;
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
        MaterialResetter matRes = meshObject.AddComponent<MaterialResetter>();
        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();

        if (parent != null)
        {
            if (regionParent == null)
            {
                GameObject regionParentLocal = new GameObject();
                regionParentLocal.transform.parent = parent;
                regionParentLocal.name = "Regions";
                regionParent = regionParentLocal.transform;
                meshObject.transform.parent = regionParent.transform;
            }
            else
            {
                meshObject.transform.parent = regionParent.transform;
            }

        }
        meshObject.name = "Region | " + name;
        meshObject.transform.localScale = new Vector3(1, 1, 1);
        // Assign the generated mesh to the filter
        meshFilter.mesh = mesh;
        //meshObject.transform.position = new Vector3(coordinates[0].x,coordinates[0].y,0);
        meshObject.transform.eulerAngles = new Vector3(-90, 0, 0);

        // Optionally, attach a material to the mesh renderer for visualization
        regions[regionIndex].material = new Material(regionMaterials[StringTypeToInt(type)]);
        regions[regionIndex].material.name = "mat_" + regions[regionIndex].name;
        regions[regionIndex].typeIndex = StringTypeToInt(type);
        matRes.material = regions[regionIndex].material;
        matRes.initialColor = regions[regionIndex].material.color;
        Material[] mats = new Material[2];
        mats[0] = regions[regionIndex].material;
        //mats[1] = outlineMaterial;
        meshRenderer.materials = mats;

        FlipNormals(meshFilter);
    }//GenerateMesh

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
   
    public int StringTypeToInt(string type)
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
}//Class


[System.SerializableAttribute]
public class SiteData
{
    public string name;
    public string type;
    public int typeIndex;
    public Vector2Int coord;
    public Rect rectangle;
}

[System.SerializableAttribute]
public class RegionData
{
    public string name;
    public string type;
    public string evilness;
    public int typeIndex;
    public Vector2Int[] coords;
    public Material material;
    public Material outlineMat;
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