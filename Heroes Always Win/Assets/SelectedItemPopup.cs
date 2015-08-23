using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectedItemPopup : MonoBehaviour {

    public Text txtItemName;
    public GameObject container;
    public Image itemImage;

    // Use this for initialization
    void Start () {
    
    }

    public void Show(string name, Sprite sprite)
    {
        container.SetActive(true);
        txtItemName.text = name;
        itemImage.sprite = sprite;
    }

    public void Hide()
    {
        container.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
    
    }
}
