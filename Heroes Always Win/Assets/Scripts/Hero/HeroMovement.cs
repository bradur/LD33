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

    // Use this for initialization
    void Start () {

    }

    public void Init(Map map, int x, int y)
    {
        this.map = map;
        Debug.Log("Hero start at: ["+ x + ", " + y + "]");
        currentNode = map.GetNode(x, y);
        debugText = GameObject.FindGameObjectWithTag("DebugText").GetComponent<Text>();
        debugText.text = map.NeighborsToString(currentNode);
    }


    int CalculateHeuristicCost(MapNode node, MapNode destination)
    {
        return Mathf.Abs(node.x - destination.x) * 10 + Mathf.Abs(node.y - destination.y) * 10 - 2;
    }

    void SetDestination(MapNode destination)
    {
        this.destination = destination;
    }

    void SearchPath(MapNode currentNode)
    {
        openNodes.Add(currentNode);
        closedNodes.Add(currentNode);
        MapNode lowestCostNode = null;

        foreach (MapNode neighbor in map.GetNeighbors(currentNode))
        {
            if (neighbor.isWalkable && !closedNodes.Contains(neighbor)) {
                if (openNodes.Contains(neighbor))
                {
                    MapNode oldParent = neighbor.GetParent();
                    int gCost = neighbor.movementCost + neighbor.parentCost;
                    neighbor.SetParent(currentNode);
                    int newGCost = neighbor.movementCost + neighbor.parentCost;
                    if (newGCost >= gCost)
                    {
                        neighbor.SetParent(oldParent);
                    }
                }
                else {
                    neighbor.SetCost(CalculateHeuristicCost(neighbor, destination));
                    neighbor.SetParent(currentNode);
                    if (lowestCostNode == null || neighbor.fullCost < lowestCostNode.fullCost)
                    {
                        lowestCostNode = neighbor;
                    }
                    openNodes.Add(neighbor);
                }
            }
        }
        openNodes.Remove(currentNode);
        openNodes.Remove(lowestCostNode);
        if (lowestCostNode != null)
        {
            SearchPath(lowestCostNode);
        }
    }

    // Update is called once per frame
    void Update () {
    
    }
}
