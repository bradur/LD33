using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpawnedItemHoverPopup : MonoBehaviour {

    public Text title;
    public Animator animator;
    public SpawnedItemBase itemOnGround;
    public GameManager gameManager;
    public bool isActivated = false;

    public void Activate(string itemName, SpawnedItemBase item)
    {
        isActivated = true;
        title.text = itemName;
        itemOnGround = item;
        transform.position = Camera.main.WorldToScreenPoint(item.transform.position);
        animator.SetTrigger("Show");
        gameManager.noSelection = false;
    }

    public void DeActivate()
    {
        isActivated = false;
        title.text = "";
        animator.SetTrigger("Hide");
        if (itemOnGround != null)
        {
            itemOnGround.allowDeselect = true;
            itemOnGround.Deselect();
        }
        gameManager.noSelection = true;
    }

    public void RemoveItem()
    {
        if (gameManager.RemoveItemThroughHoverPopup(title.text, itemOnGround))
        {
            DeActivate();
            itemOnGround.Kill();
        }

    }

    public void MoveItem()
    {
        if (gameManager.SelectInventoryItemThroughHoverPopup(title.text, itemOnGround))
        {
            itemOnGround.allowDeselect = true;
            itemOnGround.Deselect();
            DeActivate();
            itemOnGround.Kill();
        }

    }

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }


}
