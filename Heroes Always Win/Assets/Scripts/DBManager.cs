using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DBManager : MonoBehaviour {

    GameObject[] inventoryItems;

    // Use this for initialization
    void Start () {
        LoadInventory();
    }



    // Update is called once per frame
    void Update () {
    
    }

    void LoadInventory()
    {
        if (inventoryItems == null) { 
            inventoryItems = Resources.LoadAll<GameObject>("Items");
            //PlayerPrefs.SetInt("Villain", 6);
            //PlayerPrefs.SetInt("Gold", 3);
        }
    }

    public void HideHelp()
    {
        PlayerPrefs.SetInt("HideHelp", 1);
    }

    public bool HelpShouldBeShown()
    {
        return (PlayerPrefs.GetInt("HideHelp") == 1 ? false : true);
    }

    /*public void SetInventory(List<InventoryItem> items)
    {
        foreach (InventoryItem item in items)
        {
            PlayerPrefs.SetInt(item.itemName, item.itemCount);
        }
    }*/

    public List<InventoryItem> GetInventory(int villains, int gold)
    {
        List<InventoryItem> currentItems = new List<InventoryItem>();
        LoadInventory();
        for (int i = 0; i < inventoryItems.Length; i += 1)
        {

            InventoryItem item = inventoryItems[i].GetComponent<InventoryItem>();
            int itemCount = 0;
            if (item.itemName == "Minion")
            {
                itemCount = villains;
            }
            else
            {
                itemCount = gold;
            }
            //int itemCount = PlayerPrefs.GetInt(item.itemName);
            if (itemCount > 0)
            {
                item.Init(itemCount);
                currentItems.Add(item);
            }
        }
        return currentItems;
    }
}
