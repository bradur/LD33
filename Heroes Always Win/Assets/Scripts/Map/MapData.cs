using TiledSharp;
using UnityEngine;
using System.Xml.Linq;

public class MapData {

    public MapSquare[] tiles;
    public int horizontal_tiles;
    public int vertical_tiles;
    public int tile_width;
    public int tile_height;
    public string tileSetName;

    public int tileCount;

    private GameObject wallPref;
    public string mapTitle;

    public MapData(TextAsset mapFile, Material tileSheet)
    {

        TmxMap map;

        map = new TmxMap(mapFile.text, "rnd");


        horizontal_tiles = map.Width;
        vertical_tiles = map.Height;

        tile_width = map.TileWidth;
        tile_height = map.TileHeight;

        tileSetName = map.Tilesets[0].Name;
        mapTitle = map.Properties["Title"];

        tileCount = map.Layers[0].Tiles.Count;
        tiles = new MapSquare[tileCount];
        for (int i = 0; i < tileCount; i++)
        {
            tiles[i] = new MapSquare(map.Layers[0].Tiles[i]);
        }

        if (map.ObjectGroups.Count > 0) {
            GenerateObjects(map.ObjectGroups);
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

    }

    public void GenerateObjects(TmxObjectGroupList objectGroupList){

        int objectGroupCount = objectGroupList.Count;
        for (int i = 0; i < objectGroupCount; i += 1 )
        {
            TmxObjectGroup objectGroup = objectGroupList[i];
            int objectCount = objectGroup.Objects.Count;
            
            for (int j = 0; j < objectCount; j += 1)
            {
                TmxObjectGroup.TmxObject tmxObject = objectGroup.Objects[j];

                int xpos = (int)tmxObject.X / tile_width;
                int ypos = (int)tmxObject.Y / tile_height;
                Vector3 worldPos = new Vector3(-xpos + 0.5f, 1f, ypos - 1.5f);
                GameObject spawnedObject = new GameObject(tmxObject.Name);
                spawnedObject.transform.position = worldPos;

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

    
    void CreateWall(Material wallMaterial)
    {
        wallPref = (GameObject)GameObject.Instantiate(Resources.Load("Wall"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        wallPref.GetComponent<Renderer>().material = wallMaterial;
        //Mesh mesh = wallPref.GetComponent<MeshFilter>().mesh;
        Mesh mesh = wallPref.GetComponent<MeshFilter>().sharedMesh;
        Vector2 texture = new Vector2(1f, 7f);
        float tileUnit = 0.125f;
        float offset = tileUnit / 64 * 2;
        int height = 8;
        int offsetY = (int)(height - texture.y);
        float margin = tileUnit / 64;

        float left = tileUnit * texture.x + texture.x * offset + margin;
        float right = left + tileUnit + margin;
        float bottom = tileUnit * texture.y - margin;
        float top = bottom + tileUnit;

        Rect wallText = new Rect(
            left,
            top,
            tileUnit,
            tileUnit
        );

        Debug.Log(wallText);

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
