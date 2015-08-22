using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

    public HeroMovement movement;
    Map map;
    int startX;
    int startY;
    int endX;
    int endY;

    public void Init(Map map, int startX, int startY)
    {
        this.map = map;
        this.startX = startX;
        this.startY = startY;
        
    }

    public void SetEndSpot(int endX, int endY)
    {
        this.endX = endX;
        this.endY = endY;
        movement.Init(map, startX, startY, endX, endY);
    }
    
    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }
}
