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

    public void Init(int itemCount)
    {
        this.itemCount = itemCount;
        UpdateCount(0);
    }

    public void UpdateCount(int difference)
    {
        itemCount += difference;
        count.text = itemCount.ToString();
    }

    public void Select()
    {
        gameManager.SelectInventoryItem(this);
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
