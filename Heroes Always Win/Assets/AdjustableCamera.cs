﻿using UnityEngine;
using System.Collections;

public class AdjustableCamera : MonoBehaviour {

    [Range(1, 6)]
    public float orthographicSizeMin = 1f;
    [Range(6, 16)]
    public float orthographicSizeMax = 6f;
    [Range(0.5f, 5f)]
    public float panSpeed = 1f;
    float originalSize;
    Vector3 originalpos;
    bool isMoving = false;
    Vector3 dragOrigin;
    Vector3 prevPos;
    Vector3 mouseDelta;


    [Range(0, 3)]
    public float cameraMinPosZ = 0f;

    [Range(17, 20)]
    public float cameraMaxPosZ = 0f;

    [Range(-25, -32)]
    public float cameraMinPosX = 0f;

    [Range(-5, -15)]
    public float cameraMaxPosX = 0f;
    

    // Use this for initialization
    void Start () {
        originalpos = transform.position;
        originalSize = Camera.main.orthographicSize;
    }
    
    // Update is called once per frame
    void Update () {

        // ZOOM IN / OUT
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize++;
        }
        if(Input.GetAxis("Mouse ScrollWheel") > 0){
            Camera.main.orthographicSize--;
        }
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, orthographicSizeMin, orthographicSizeMax );

        // PANNING START
        if (Input.GetMouseButtonDown(0))
        {
            isMoving = true;
            //dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragOrigin = Input.mousePosition;
        }

        // PANNING
        if (Input.GetMouseButton(0) && isMoving)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(pos.x * panSpeed, transform.position.y, pos.y * panSpeed);
            transform.Translate(move, Space.World);
            float xPos = transform.position.x;
            if (xPos > cameraMaxPosX)
            {
                xPos = cameraMaxPosX;
            }
            else if (xPos < cameraMinPosX)
            {
                xPos = cameraMinPosX;
            }
            float zPos = transform.position.z;
            if(zPos > cameraMaxPosZ){
                zPos = cameraMaxPosZ;
            } else if(zPos < cameraMinPosZ){
                zPos = cameraMinPosZ;
            }
            transform.position = new Vector3(xPos, originalpos.y, zPos);
        }

        // PANNING STOP
        if (Input.GetMouseButtonUp(0))
        {
            isMoving = false;
        }


        // RESET POS
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Camera.main.orthographicSize = originalSize;
            Camera.main.transform.position = originalpos;
        }
        
    }
}
