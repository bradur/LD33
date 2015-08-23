using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

    public HeroMovement movement;
    Map map;
    int startX;
    int startY;

    public void Init(Map map, int startX, int startY)
    {
        this.map = map;
        this.startX = startX;
        this.startY = startY;
        
    }

    public void SetEndSpot(int endX, int endY)
    {
        movement.Init(map, startX, startY, endX, endY);
    }

    public void ProcessNodeItem(MapNode node)
    {
        SpawnedItemBase item = node.item;

        // do smth
        // and finally unset item
        item.Kill();
    }

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }
}
