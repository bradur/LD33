using TiledSharp;
using UnityEngine;
using System.Xml.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapData: MonoBehaviour {

    public Map tileMap;
    public int horizontal_tiles;
    public int vertical_tiles;
    public int tile_width;
    public int tile_height;
    public string tileSetName;

    public int tileCount;

    private GameObject wallPref;
    public Transform tileContainer;
    public Transform objectContainer;
    public Transform tempContainer;
    public string mapTitle;
    private Material wallMaterial;
    public Text mapDebug;
    public MapSquare[] tiles;
    public LevelTimer timer;
    int[] firstGids;
    int endX;
    public int time;
    int endY;
    public int villains;
    public int gold;
    Hero hero;
    public GameManager manager;

    public int IntParseFast(string value)
    {
        int result = 0;
        for (int i = 0; i < value.Length; i += 1)
        {
            char letter = value[i];
            result = 10 * result + (letter - 48);
        }
        return result;
    }

    public void Init(TextAsset mapFile, Material tileSheet, Material wallMaterial)
    {

        foreach (Transform child in tileContainer)
        {
            DestroyObject(child.gameObject);
        }
        foreach (Transform child in objectContainer)
        {
            DestroyObject(child.gameObject);
        }
        TmxMap map;

        map = new TmxMap(mapFile.text, "rnd");
        this.wallMaterial = wallMaterial;

        horizontal_tiles = map.Width;
        vertical_tiles = map.Height;

        tile_width = map.TileWidth;
        tile_height = map.TileHeight;

        //tileSetName = map.Tilesets[0].Name;
        firstGids = new int[map.Tilesets.Count];
        mapTitle = map.Properties["Title"];
        time = IntParseFast(map.Properties["Time"]);
        villains = IntParseFast(map.Properties["Villain"]);
        gold = IntParseFast(map.Properties["Gold"]);
        timer.Init(time);

        tileMap = new Map(map.Width, map.Height);

        if (map.Layers.Count > 0)
        {
            GenerateTiles(map.Layers);
            mapDebug.text = tileMap.PrintableString();
        }

        if (map.ObjectGroups.Count > 0)
        {
            GenerateObjects(map.ObjectGroups);
        }
        hero.SetEndSpot(endX, endY);

        foreach (Transform child in tempContainer)
        {
            DestroyObject(child.gameObject);
        }
        manager.MapGenerationCallBack(this);
    }

    public void DestroyObject(GameObject objectToDestroy)
    {
        if (Application.isPlaying)
        {
            Destroy(objectToDestroy);
        }
        else
        {
            DestroyImmediate(objectToDestroy);
        }
    }
/*        int wallCount = map.ObjectGroups[2].Objects.Count;
        CreateWall(tileSheet);

        for (int i = 0; i < wallCount; i++)
        {
            TmxObjectGroup.TmxObject wall = map.ObjectGroups[2].Objects[i];
            //Debug.Log("object at [" + enemy.X + ", " + enemy.Y + "]");
            int startEndX = (int)wall.X / tile_width;
            int startEndY = (int)wall.Y / tile_height;
            Vector3 worldPos = new Vector3(-startEndX, 0.5f, startEndY);

            GameObject wallObject = (GameObject)GameObject.Instantiate(wallPref, worldPos, Quaternion.identity);
            //wallObject.transform.parent = wallContainer.transform;
            wallObject.transform.localPosition = worldPos;
        }
        if (Application.isPlaying)
        {
            GameObject.Destroy(wallPref);
        }*/

        /*
        int objectCount = map.ObjectGroups[3].Objects.Count;

        for (int i = 0; i < objectCount; i++)
        {
            TmxObjectGroup.TmxObject genericObject = map.ObjectGroups[3].Objects[i];
            //Debug.Log("object at [" + enemy.X + ", " + enemy.Y + "]");
            int startEndX = (int)genericObject.X / tile_width;
            int startEndY = (int)genericObject.Y / tile_height;
            Vector3 worldPos = new Vector3(-startEndX + 0.5f, 0.5f, startEndY -1.5f);

        }*/


    public void GenerateTiles(TmxLayerList layerList)
    {
        
        int layerCount = layerList.Count;
        for (int i = 0; i < layerCount; i += 1)
        {
            TmxLayer layer = layerList[i];
            int tileCount = layer.Tiles.Count;
            PropertyDict properties = layer.Properties;

            string tileType = "Ground";
            if (properties.ContainsKey("Type"))
            {
                tileType = properties["Type"];
            }
            if (tileType == "Ground")
            {
                tiles = new MapSquare[layer.Tiles.Count];
            }
            Transform layerContainer = new GameObject(layer.Name).transform;
            layerContainer.rotation = Quaternion.identity;
            layerContainer.transform.SetParent(tileContainer);

            //Debug.Log(tileCount);
            
            for (int j = 0; j < tileCount; j += 1)
            {
                TmxLayerTile tmxTile = layer.Tiles[j];
                int tileSetId = FindTileSetId(tmxTile.Gid);

                if (tileSetId == -1)
                {
                    continue;
                }
                if (tileType == "Wall")
                {
                    if (tmxTile.Gid == 0)
                    {
                        continue;
                    }
                    tileMap.AddNode(tmxTile.X, tmxTile.Y, false);

                    int xpos = (int)tmxTile.X;
                    int ypos = (int)tmxTile.Y;
                    Vector3 worldPos = new Vector3(-xpos + 0.5f, 0.5f, ypos-0.5f);
                    //Debug.Log("[" + tmxTile.X + ", " + tmxTile.Y + "]");
                    GameObject spawnedTile;
                    if (wallPref == null) {
                        CreateWall();
                    }
                    spawnedTile = (GameObject)Instantiate(wallPref, worldPos, Quaternion.identity);
                    spawnedTile.name = "Wall_" + tmxTile.Gid;
                    spawnedTile.transform.position = worldPos;
                    spawnedTile.transform.parent = layerContainer;
                }
                if (tileType == "Ground")
                {
                    if (tmxTile.Gid != 0) {
                        tileMap.AddNode(tmxTile.X, tmxTile.Y);
                    }
                    tiles[j] = new MapSquare(tmxTile);
                    
                }


            }


        }


    }

    int FindTileSetId(int gid)
    {
        // loop from last to first (largest gid first)
        for (int i = firstGids.Length - 1; i >= 0; i--)
        {
            if (gid >= firstGids[i])
            {
                return firstGids[i];
            }
        }
        // error: no such tileset
        return -1;
    }

    public void GenerateObjects(TmxObjectGroupList objectGroupList){

        int objectGroupCount = objectGroupList.Count;
        for (int i = 0; i < objectGroupCount; i += 1 )
        {
            TmxObjectGroup objectGroup = objectGroupList[i];
            int objectCount = objectGroup.Objects.Count;
            PropertyDict properties = objectGroup.Properties;

            Transform layerContainer = new GameObject(objectGroup.Name).transform;
            layerContainer.rotation = Quaternion.identity;
            layerContainer.transform.SetParent(objectContainer);
            for (int j = 0; j < objectCount; j += 1)
            {
                if (!properties.ContainsKey("Type"))
                {
                    Debug.Log("ERROR: Object Layer \"" + objectGroup.Name + "\" doesn't have a Type property!");
                    continue;
                }
                string objectType = properties["Type"];
                TmxObjectGroup.TmxObject tmxObject = objectGroup.Objects[j];

                int xpos = (int)tmxObject.X / tile_width;
                int ypos = (int)tmxObject.Y / tile_height - 1;

                Vector3 worldPos = new Vector3(-xpos + 0.5f, 0.5f, ypos - 0.5f);
                GameObject spawnedObject = (GameObject)GameObject.Instantiate(Resources.Load("Objects/" + objectType), worldPos, Quaternion.identity);
                spawnedObject.name = tmxObject.ToString();

                spawnedObject.transform.position = worldPos;
                spawnedObject.transform.parent = layerContainer;

                if (objectType == "Hero")
                {
                    hero = spawnedObject.GetComponent<Hero>();
                    hero.Init(tileMap, xpos, ypos);
                }
                if (objectType == "End")
                {
                    endX = xpos;
                    endY = ypos;
                }
                /*TmxObjectGroup.TmxObject startEnd = ObjectGroups[i].Objects[j];

                int startEndX = (int)startEnd.X / tile_width;
                int startEndY = (int)startEnd.Y / tile_height;
                Vector3 worldPos = new Vector3(-startEndX + 0.5f, 1f, startEndY - 1.5f);

                if (startEnd.Name == "Start")
                {
                    //player.Spawn(worldPos, tile_width, tile_height);
                    GameObject startObject = (GameObject)GameObject.Instantiate(Resources.Load("SpawnPoint"), worldPos, Quaternion.identity);
                }
                else if (startEnd.Name == "End")
                {
                    GameObject endObjectPrefab = (GameObject)GameObject.Instantiate(Resources.Load("LevelEndTrigger"));
                    endObjectPrefab.transform.position = worldPos;
                }*/
            }
        }

        
    }

    
    void CreateWall()
    {
        wallPref = (GameObject)GameObject.Instantiate(Resources.Load("Wall"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        wallPref.GetComponent<Renderer>().material = wallMaterial;
        wallPref.transform.parent = tempContainer;
        //Mesh mesh = wallPref.GetComponent<MeshFilter>().mesh;
        Mesh mesh = wallPref.GetComponent<MeshFilter>().sharedMesh;
        //Vector2 texture = new Vector2(1f, 1f);
        float tileUnit = 1f;
        //float offset = tileUnit / tile_width * 2;
        //int height = 8;
        //int offsetY = (int)(height - texture.y);
        //float margin = tileUnit / tile_width;

        //float left = tileUnit * texture.x + texture.x * offset + margin;
        //float right = left + tileUnit + margin;
        //float bottom = tileUnit * texture.y - margin;
        //float top = bottom + tileUnit;
        float left = 0f;
        float top = 1f;

        Rect wallText = new Rect(
            left,
            top,
            tileUnit,
            tileUnit
        );

        Vector2 text1 = new Vector2(wallText.x, wallText.y);
        Vector2 text2 = new Vector2(wallText.x + tileUnit, wallText.y);
        Vector2 text3 = new Vector2(wallText.x, wallText.y - tileUnit); ;
        Vector2 text4 = new Vector2(wallText.x + wallText.width, wallText.y - tileUnit);
        //print(text1 + ", " + text2 + "," + text3 + "," + text4);

        Vector2[] uv = new Vector2[mesh.uv.Length];
        uv = mesh.uv;
        // FRONT    2    3    0    1
        uv[2] = text1;
        uv[3] = text2;
        uv[0] = text3;
        uv[1] = text4;
        // BACK    6    7   10   11
        uv[6] = text1;
        uv[7] = text2;
        uv[10] = text3;
        uv[11] = text4;
        // LEFT   19   17   16   18
        uv[19] = text1;
        uv[17] = text2;
        uv[16] = text3;
        uv[18] = text4;
        // RIGHT   23   21   20   22
        uv[23] = text1;
        uv[21] = text2;
        uv[20] = text3;
        uv[22] = text4;

        // TOP    4    5    8    9
        uv[4] = text1;
        uv[5] = text2;
        uv[8] = text3;
        uv[9] = text4;

        // BOTTOM   15   13   12   14
        uv[15] = text1;
        uv[13] = text2;
        uv[12] = text3;
        uv[14] = text4;
        mesh.uv = uv;

    }

}
