using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryItem : MonoBehaviour {

    public Text count;
    public string itemName;
    public Image image;
    public int itemCount;
    public GameObject prefab;
    public GameManager gameManager;
    public PlacementArea placementArea;
    public Text txtTitle;

    public void Init(int itemCount)
    {
        this.itemCount = itemCount;
        txtTitle.text = this.itemName;
        UpdateCount(0);
    }

    public void UpdateCount(int difference)
    {
        itemCount += difference;
        count.text = itemCount.ToString();
    }

    public void Select(){
        if (gameManager.selectedItem == this) {
            gameManager.CancelItemSelection();
        }
        else if (itemCount < 1)
        {
            // play sound?
        }
        else {
            gameManager.SelectInventoryItem(this);
        }
        //{
            //UpdateCount(-1);
        //};
    }

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
    
    }

}
