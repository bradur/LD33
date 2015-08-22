using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public enum PlacementArea
{
    None,
    Villain,
    Door,
    Key
}

public class GameManager : MonoBehaviour {

    public DBManager dbManager;
    List<InventoryItem> inventoryItems;
    InventoryItem selectedItem;
    public SpriteRenderer followSprite;
    Transform followSpriteTransform;
    public RectTransform inventoryContents;
    public MeshTileMap meshTileMap;
    Map tileMap;
    Vector3 mouseToWorld;
    public float inputZ = 15f;
    bool allowSpawn = false;

    // Use this for initialization
    void Start () {
        //meshTileMap.SetMap(tmxMap);
        //meshTileMap.GenerateMesh();
        inventoryItems = dbManager.GetInventory();
        foreach (InventoryItem item in inventoryItems)
        {
            item.gameManager = this;
            GameObject newInventoryItem = Instantiate(item).gameObject;
            newInventoryItem.transform.SetParent(inventoryContents, false);
        }
    }

    public bool SelectInventoryItem(InventoryItem item)
    {
        if (tileMap == null)
        {
            tileMap = meshTileMap.mapData.tileMap;
        }
        if (selectedItem == null)
        {
            selectedItem = item;
            followSprite.sprite = selectedItem.image.sprite;
            followSprite.enabled = true;
            followSpriteTransform = followSprite.GetComponent<Transform>();
            if (item.placementArea == PlacementArea.Villain)
            {
                meshTileMap.SelectGround();
            }
            return true;
        }
        else if (selectedItem == item)
        {
            CancelItemSelection();
        }
        return false;
    }

    // if user cancels use (esc key, click same item)
    public void CancelItemSelection()
    {
        selectedItem.UpdateCount(1);
        UnselectItem();
    }

    // if user places item
    public void UseItem()
    {
        UnselectItem();
    }

    // generic unselection of selected item
    public void UnselectItem()
    {
        if (selectedItem.placementArea == PlacementArea.Villain)
        {
            meshTileMap.UnselectGround();
        }
        selectedItem = null;
        followSprite.enabled = false;
    }

    // Update is called once per frame
    void Update () {
        if (selectedItem != null)
        {

            mouseToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, inputZ));
//            Debug.Log(Input.mousePosition + " -> " + mouseToWorld);
            followSpriteTransform.position = new Vector3(mouseToWorld.x, followSpriteTransform.position.y, mouseToWorld.z);
            if (Input.GetMouseButtonUp(0))
            {
                if (selectedItem.placementArea == PlacementArea.Villain)
                {
                    // if mouse is on top of placement area (this time meshTileMap)
                    if (meshTileMap.isHovering)
                    {
                        allowSpawn = true;
                    }
                }
                if (allowSpawn)
                {
                    float x = followSpriteTransform.position.x;
                    float z = followSpriteTransform.position.z;

                    // SNAP TO GRID
                    float newx = Mathf.Round(x);
                    if (x - newx > 0)
                    {
                        newx += 0.5f;
                    }
                    else
                    {
                        newx -= 0.5f;
                    }
                    float newz = Mathf.Round(z);
                    if (z - newz > 0)
                    {
                        newz += 0.5f;
                    }
                    else
                    {
                        newz -= 0.5f;
                    }

                    GameObject spawnedItem = (GameObject)Instantiate(selectedItem.prefab, new Vector3(newx, followSpriteTransform.position.y, newz), followSpriteTransform.rotation);
                    spawnedItem.transform.SetParent(followSpriteTransform.parent, true);
                    UseItem();
                    allowSpawn = false;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            CancelItemSelection();
        }
    }
}
