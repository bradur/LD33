using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpawnedItemBase : MonoBehaviour {


    public GameManager gameManager;
    public string itemName;
    public SpriteRenderer hoverSprite;
    public Color hoverColor;
    public float dieDuration = 1f;
    Color originalColor;
    public MapNode node;
    public Hero hero;
    public bool allowDeselect = true;

    public void Init(GameManager manager, string name, MapNode node)
    {
        this.gameManager = manager;
        this.itemName = name;
        originalColor = hoverSprite.color;
        node.item = this;
        this.node = node;
    }

    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {
    
    }

    public virtual void InteractWithHero(Hero hero)
    {
        this.hero = hero;
        StartCoroutine("DieLater");
    }

    IEnumerator DieLater()
    {
        yield return new WaitForSeconds(dieDuration);
        hero.OpponentDied();
        Kill();
    }

    public void Kill()
    {
        node.item = null;
        allowDeselect = true;
        Deselect();
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

    /*void OnMouseUpAsButton()
    {
        if (gameManager.noSelection){



        }
    }*/

    void OnMouseOver()
    {
        if (gameManager.noSelection)
        {
            if (Input.GetMouseButtonUp(0))
            {

                gameManager.ClickSpawnedItem(this);
                allowDeselect = false;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                if (gameManager.RemoveItemThroughHoverPopup(itemName, this))
                {
                    Kill();
                }
            }
            else if (Input.GetMouseButtonUp(2))
            {
                if (gameManager.SelectInventoryItemThroughHoverPopup(itemName, this))
                {
                    Kill();
                }
            }
        }
    }

    void OnMouseEnter()
    {
        if (gameManager.noSelection)
        {
            Select();
        }
    }

    void OnMouseExit()
    {
        if (gameManager.noSelection){
            Deselect();
        }
    }
}
