using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HeroMovement : MonoBehaviour {

    Map map;

    Text debugText;

    List<MapNode> openNodes = new List<MapNode>();
    List<MapNode> closedNodes = new List<MapNode>();
    MapNode currentNode;
    MapNode destination;
    List<MapNode> finalPath = new List<MapNode>();
    Vector3 targetPosition;
    float movementInterval = 0.01f;
    public float movementDuration = 0.25f;
    float movementTimer = 0f;
    Transform thisTransform;
    public Hero hero;
    MapNode currentTargetNode;
    GameManager gameManager;
    bool moving;
    // Use this for initialization
    void Start () {

    }

    public void Init(Map map, int x, int y, int endX, int endY)
    {
        this.map = map;
        Debug.Log("Hero start at: ["+ x + ", " + y + "]");
        MapNode currentNode = map.GetNode(x, y);
        //debugText = GameObject.FindGameObjectWithTag("DebugText").GetComponent<Text>();
        //debugText.text = map.NeighborsToString(currentNode);
        destination = map.GetNode(endX, endY);
        //SearchPath(currentNode);
        SearchPath(currentNode);
        thisTransform = GetComponent<Transform>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }


    int CalculateMovementCost(MapNode currentNode, MapNode neighbor)
    {
        return ((currentNode.x == neighbor.x) || (currentNode.y == neighbor.y)) ? 10 : 14;
    }
    int CalculateHeuristicCost(MapNode node, MapNode destination)
    {
        return Mathf.Abs(node.x - destination.x) * 10 + Mathf.Abs(node.y - destination.y) * 10 - 2;
    }

    void SetDestination(MapNode destination)
    {
        this.destination = destination;
    }

    IEnumerator Move()
    {
        while (hero.fightIsOver != true)
        {
            //Debug.Log("Hero target: [" + currentTargetNode.x + ", " + currentTargetNode.y + "]" + (currentTargetNode.item != null ? " + ITEM" : ""));
            yield return 0;
            //Debug.Log("Has item: " + currentTargetNode.item.itemName);
        }

        moving = true;
    }

    void NextNode()
    {
        if (finalPath.Count > 0){

            currentTargetNode = finalPath[finalPath.Count - 1];
            if (currentTargetNode.item != null)
            {
                hero.ProcessNodeItem(currentNode, currentTargetNode);
                moving = false;
                StartCoroutine("Move");
            }
            if (thisTransform == null)
            {
                thisTransform = GetComponent<Transform>();
            }
            targetPosition = new Vector3(-currentTargetNode.x + 0.5f, thisTransform.position.y, currentTargetNode.y - 0.5f);
            finalPath.RemoveAt(finalPath.Count - 1);
        }
        else
        {
            gameManager.LevelFinished();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            movementTimer += Time.fixedDeltaTime / movementDuration;
            thisTransform.position = Vector3.MoveTowards(thisTransform.position, targetPosition, movementTimer);
            if(Vector3.Distance(thisTransform.position, targetPosition) < 0.05f){
                //thisTransform.position = targetPosition;
                currentNode = currentTargetNode;
                movementTimer = 0;
                //moving = false;
                NextNode();

            }
        }
    }

    void SetFinalPath(MapNode node)
    {
        while (true)
        {
            MapNode parentNode = node.GetParent();
            if (parentNode != null)
            {
//                Debug.Log("[" + parentNode.x + ", " + parentNode.y + "]");
                finalPath.Add(parentNode);
                node = parentNode;
            }
            else
            {
                finalPath.RemoveAt(finalPath.Count-1);
                break;
            }
        }
        NextNode();
        StartCoroutine("Move");
    }

    void SearchPath(MapNode currentNode)
    {
        //debugText.text = map.NeighborsToString(currentNode);
        openNodes.Add(currentNode);                                             // add current node to open list
        currentNode.list = "o";
        MapNode lowestCostNode = null;
        bool targetFound = false;
        foreach (MapNode neighbor in map.GetNeighbors(currentNode))             // go through neighbors
        {
            bool considerOpenListNeighbor = false;
            
            if (neighbor.isWalkable && !closedNodes.Contains(neighbor)) {       // if neighbor is reachable and not in closed list
                bool isInOpenList = openNodes.Contains(neighbor);
                if (isInOpenList)                                               // if neighbor is in open list
                {
                    int gCost = neighbor.movementCost + neighbor.parentCost;
                    int newGCost = neighbor.movementCost + currentNode.movementCost;
                    if (gCost > newGCost)
                    {
                        considerOpenListNeighbor = true;
                    }
                }
                if (!isInOpenList || considerOpenListNeighbor)
                {
                    neighbor.SetParent(currentNode);                             // set current node as neighbors parent
                    neighbor.SetMovementCost(CalculateMovementCost(currentNode, neighbor));
                    neighbor.SetHeuristicCost(CalculateHeuristicCost(neighbor, destination));
                }
                if (lowestCostNode == null || neighbor.fullCost < lowestCostNode.fullCost)
                {
                    lowestCostNode = neighbor;
                }
                if (!isInOpenList)
                {
                    if (neighbor == destination)
                    {
                        targetFound = true;
                        //Debug.Log("Target found!");
                        SetFinalPath(neighbor);
                        //StopAllCoroutines();
                    }
                    else
                    {
                        openNodes.Add(neighbor);
                        neighbor.list = "o";
                    }
                }
            }
        }
        if (!targetFound) {
            openNodes.Remove(currentNode);
            currentNode.list = ".";
            closedNodes.Add(currentNode);
            currentNode.list = "c";
            if (lowestCostNode != null)
            {
                openNodes.Remove(lowestCostNode);
                lowestCostNode.list = ".";
                //SearchPath(lowestCostNode);
                //Debug.Log("Starting another Coroutine!");
                SearchPath(lowestCostNode);
            }
        }
    }

}
