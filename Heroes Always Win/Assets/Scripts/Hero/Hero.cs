using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

    public HeroMovement movement;

    public void Init(Map map, int startX, int startY)
    {
        movement.Init(map, startX, startY);
    }
    
    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }
}
