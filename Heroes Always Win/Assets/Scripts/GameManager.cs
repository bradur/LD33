﻿using UnityEngine;
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
    public InventoryItem selectedItem;
    public SpriteRenderer followSprite;
    Transform followSpriteTransform;
    public RectTransform inventoryContents;
    public MeshTileMap meshTileMap;
    Map tileMap;
    Vector3 mouseToWorld;
    public float inputZ = 15f;
    public GameObject helpPopup;
    public GameObject showHelpButton;
    bool allowSpawn = false;
    bool anItemIsHovered = false;
    bool itemMoveIsInProcess = false;
    public LevelTimer timer;
    public SelectedItemPopup selectedItemPopup;

    public SpawnedItemHoverPopup itemPopup;

    public VictoryMenu victoryMenu;
    public GameOverMenu gameOverMenu;
    public bool noSelection = true;
    public AudioSource minionDie;
    public AudioSource pickup;

    public GameObject messageContainer;
    public Text txtMessage;

    public GameObject endGameMenu;

    // Use this for initialization
    void Start () {
        //meshTileMap.SetMap(tmxMap);
        //meshTileMap.GenerateMesh();
        meshTileMap.GenerateMesh(dbManager.GetLevel());
        if (dbManager.HelpShouldBeShown())
        {
            helpPopup.SetActive(true);
        }
        else
        {
            showHelpButton.SetActive(true);
        }

    }

    public void MapGenerationCallBack(MapData mapData)
    {
        inventoryItems = dbManager.GetInventory(mapData.villains, mapData.gold);
        for (int i = 0; i < inventoryItems.Count; i += 1)
        {
            InventoryItem item = inventoryItems[i];
            item.gameManager = this;
            GameObject newInventoryItem = Instantiate(item).gameObject;

            newInventoryItem.transform.SetParent(inventoryContents, false);
            RectTransform rectTransform = newInventoryItem.GetComponent<RectTransform>();
            float x = rectTransform.anchoredPosition.x + rectTransform.sizeDelta.x * i + 20f * i;
            float y = rectTransform.anchoredPosition.y;
            if (i > 1)
            {
                y -= rectTransform.sizeDelta.y * i + 20f * i;
            }

            rectTransform.anchoredPosition = new Vector2(x, y);

            inventoryItems[i] = newInventoryItem.GetComponent<InventoryItem>();
        }
    }

    public void EndGame()
    {
        //
        endGameMenu.SetActive(true);
       // Debug.Log("Game end!");
    }

    public void HideHelp(){
        dbManager.HideHelp();
    }

    public bool SelectInventoryItem(InventoryItem item)
    {
        
        if (tileMap == null)
        {
            tileMap = meshTileMap.mapData.tileMap;
        }
//        Debug.Log(item.itemCount);
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
            if (itemPopup.isActivated)
            {
                itemPopup.DeActivate();
            }
            if (!itemMoveIsInProcess) { 
                item.UpdateCount(-1);
            }
            selectedItemPopup.Show(selectedItem.itemName, selectedItem.image.sprite);
            noSelection = false;
            return true;
        }
        else if (selectedItem == item)
        {
            CancelItemSelection();
        }
        return false;
    }

    public void LevelFinished()
    {
        if (timer.enoughTime)
        {
            ShowVictoryMenu();
        }
        else
        {
            ShowGameOverMenu();
        }
        timer.StopTimer();
    }

    public void ShowVictoryMenu()
    {
        victoryMenu.gameObject.SetActive(true);
    }

    public void ShowGameOverMenu()
    {
        gameOverMenu.gameObject.SetActive(true);
    }

    public void NextLevel()
    {
        dbManager.NextLevel();
        Application.LoadLevel(1);
    }

    public void RestartLevel()
    {
        Application.LoadLevel(1);
    }

    public void OpenMainMenu()
    {
        Application.LoadLevel(0);
    }

    public void DisplayMessage(string message)
    {
        messageContainer.SetActive(true);
        txtMessage.text = message;
    }

    public void HideMessage()
    {
        messageContainer.SetActive(false);
    }

    public void StartLevel()
    {
        bool allowStart = true;
        foreach(InventoryItem item in inventoryItems){
            if (item.itemCount != 0)
            {
                allowStart = false;
            }
        }
        if (allowStart) { 
            Hero hero = GameObject.FindGameObjectWithTag("Hero").GetComponent<Hero>();
            if (hero == null)
            {
                Debug.Log("ERROR: no hero!");
            }
            hero.StartMoving();
            timer.StartTimer();
        }
        else
        {
            DisplayMessage("Place all the items first!");
        }
    }

    // if user cancels use (esc key, click same item)
    public void CancelItemSelection()
    {
        itemMoveIsInProcess = false;
        if (selectedItem != null)
        {
            selectedItem.UpdateCount(1);
            UnselectItem();
        }
        if (itemPopup != null && itemPopup.isActivated)
        {
            itemPopup.DeActivate();
        }

//        Debug.Log("Stop item move!");
    }

    // if user places item
    public void UseItem()
    {
        InventoryItem cachedItem = selectedItem;
        UnselectItem();
        if (cachedItem.itemCount > 0 && !itemMoveIsInProcess)
        {
            SelectInventoryItem(cachedItem);
        }
        else
        {

            //Debug.Log("Stop item move!");
            itemMoveIsInProcess = false;
        }
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
        selectedItemPopup.Hide();
        noSelection = true;
    }

    // when player hovers over spawned item
    public void HoverOverSpawnedItem(SpawnedItemBase spawnedItem)
    {
        anItemIsHovered = true;
    }

    public void ClickSpawnedItem(SpawnedItemBase spawnedItem)
    {
        itemPopup.Activate(spawnedItem.itemName, spawnedItem);
    }

    public void HoverOverSpawnedItemEnd()
    {
        //Debug.Log("End hover");
        anItemIsHovered = false;
    }

    public bool RemoveItemThroughHoverPopup(string itemName, SpawnedItemBase itemOnGround)
    {
        foreach (InventoryItem item in inventoryItems)
        {
            if (item.itemName == itemName)
            {
                item.UpdateCount(1);
                return true;
            }
        }
        return false;
    }

    public bool SelectInventoryItemThroughHoverPopup(string itemName, SpawnedItemBase itemOnGround)
    {
        foreach (InventoryItem item in inventoryItems)
        {
            if (item.itemName == itemName)
            {
//                Debug.Log("START item move!");
                itemMoveIsInProcess = true;
                meshTileMap.SelectComeBack();
                SelectInventoryItem(item);
                return true;
            }
        }
        return false;
    }


    // Update is called once per frame
    void Update () {

        if (selectedItem != null)
        {

            mouseToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, inputZ));
//            Debug.Log(Input.mousePosition + " -> " + mouseToWorld);
            followSpriteTransform.position = new Vector3(mouseToWorld.x, followSpriteTransform.position.y, mouseToWorld.z);


            if (Input.GetMouseButtonUp(0) && !anItemIsHovered)
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

                    GameObject spawnedObject = (GameObject)Instantiate(selectedItem.prefab, new Vector3(newx, followSpriteTransform.position.y, newz), followSpriteTransform.rotation);
                    SpawnedItemBase spawnedItem = spawnedObject.GetComponent<SpawnedItemBase>();
                    MapNode testNode = tileMap.GetNodeNormalized(newx, newz);
                    spawnedItem.Init(this, selectedItem.itemName, testNode);
                    //Debug.Log((int)(newx) + ", " + (int)(newz) + " -> " + testNode.x + ", " + testNode.y);
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
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StartLevel();
        }
    }
}
