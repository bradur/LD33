using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpawnedItemBase : MonoBehaviour {


    public GameManager gameManager;
    public string itemName;
    public SpriteRenderer hoverSprite;
    public Color hoverColor;
    Color originalColor;
    MapNode node;
    public bool allowDeselect = true;

    public void Init(GameManager manager, string name, MapNode node)
    {
        this.gameManager = manager;
        this.itemName = name;
        originalColor = hoverSprite.color;
        node.item = this;
    }

    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {
    
    }

    public void Kill()
    {
        node.item = null;
        Destroy(gameObject);
    }

    public void Select()
    {
        gameManager.HoverOverSpawnedItem(this);
        hoverSprite.color = hoverColor;
    }

    public void Deselect()
    {
        if (allowDeselect)
        {
            gameManager.HoverOverSpawnedItemEnd();
            hoverSprite.color = originalColor;
        }
    }

    void OnMouseUpAsButton()
    {
        gameManager.ClickSpawnedItem(this);
        allowDeselect = false;
    }

    void OnMouseEnter()
    {
        Select();
    }

    void OnMouseExit()
    {
        Deselect();
    }
}
