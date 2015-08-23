using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("MusicPlayer").Length == 1)
        {
            Object.DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }
}
