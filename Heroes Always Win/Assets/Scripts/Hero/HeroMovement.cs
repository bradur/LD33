using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HeroMovement : MonoBehaviour {

    Map map;

    Text debugText;

    List<MapNode> openNodes = new List<MapNode>();
    List<MapNode> closedNodes = new List<MapNode>();
    //MapNode currentNode;
    MapNode destination;
    List<MapNode> finalPath = new List<MapNode>();

    // Use this for initialization
    void Start () {

    }

    public void Init(Map map, int x, int y, int endX, int endY)
    {
        this.map = map;
        Debug.Log("Hero start at: ["+ x + ", " + y + "]");
        MapNode currentNode = map.GetNode(x, y);
        debugText = GameObject.FindGameObjectWithTag("DebugText").GetComponent<Text>();
        debugText.text = map.NeighborsToString(currentNode);
        destination = map.GetNode(endX, endY);
        //SearchPath(currentNode);
        StartCoroutine("SearchPath", currentNode);
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

    List<MapNode> GetFinalPath(MapNode node)
    {
        List<MapNode> path = new List<MapNode>();
        while (true)
        {
            MapNode parentNode = node.GetParent();
            if (parentNode != null)
            {
                Debug.Log("[" + parentNode.x + ", " + parentNode.y + "]");
                path.Add(parentNode);
                node = parentNode;
            }
            else
            {
                break;
            }
        }
        return path;
    }

    IEnumerator SearchPath(MapNode currentNode)
    {
        yield return new WaitForSeconds(1f);
        debugText.text = map.NeighborsToString(currentNode);
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
                        Debug.Log("Target found!");
                        finalPath = GetFinalPath(neighbor);
                        StopAllCoroutines();
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
                Debug.Log("Starting another Coroutine!");
                StartCoroutine("SearchPath", lowestCostNode);
            }
        }
    }

    // Update is called once per frame
    void Update () {
    
    }
}
