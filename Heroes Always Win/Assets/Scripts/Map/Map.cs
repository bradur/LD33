using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapNode
{

    public int x;
    public int y;

    public int movementCost = 0;
    public int parentCost = 0;
    int heuristicCost = 0;
    public int fullCost;
    public bool isWalkable = true;
    MapNode parentNode = null;
    public string list = ".";

    public SpawnedItemBase item;

    public MapNode(int x, int y, bool isWalkable)
    {
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
    }

    public void SetMovementCost(int heuristic)
    {
        heuristicCost = heuristic;
        fullCost = movementCost + heuristicCost + parentCost;
    }

    public void SetHeuristicCost(int heuristic)
    {
        heuristicCost = heuristic;
        fullCost = movementCost + heuristicCost + parentCost;
    }

    public void SetParent(MapNode node){
        parentNode = node;
        fullCost = movementCost + heuristicCost + parentCost;
    }

    public void ClearNode()
    {
        parentNode = null;
        movementCost = 10;
        parentCost = 0;
        heuristicCost = 10;
    }

    public MapNode GetParent()
    {
        return parentNode;
    }
}

public class Map {

    public MapNode[][] nodes;

    int width;
    int height;


    public Map(int width, int height)
    {
        this.width = width;
        this.height = height;
        nodes = new MapNode[height][];
        for (int i = 0; i < nodes.Length; i += 1)
        {
            nodes[i] = new MapNode[width];
        }
    }

    public MapNode GetNodeNormalized(float x, float y){
        return GetNode((int)(-x + 0.5f),(int)(y + 1f));
    }

    public MapNode GetNode(int x, int y)
    {
        try { 
            MapNode node = nodes[y][x];
            if (node != null)
            {
                return nodes[y][x];
            }
        }
        catch (System.IndexOutOfRangeException)
        {
            return null;
        }
        return null;
    }

    public void AddNode(int x, int y, bool isWalkable)
    {
        nodes[y][x] = new MapNode(x, y, isWalkable);
    }


    public void AddNode(int x, int y)
    {
        //if (nodes[y][x] != null) { 
            nodes[y][x] = new MapNode(x, y, true);
        //}
    }

    public string NeighborsToString(MapNode currentNode)
    {
        string content = currentNode.x + "," + currentNode.y + "\n";
        List<MapNode> neighbors = GetNeighbors(currentNode);
        int levels = 0;
        foreach(MapNode neighbor in neighbors){
            if(neighbor.isWalkable){
                content += neighbor.list;
            }
            else
            {
                if (neighbor.x == -1)
                {
                    content += "X";
                }
                else
                {
                    content += "x";
                }
            }
            levels += 1;
            if (levels % 3 == 0)
            {
                content += "\n";
            }
            if (levels == 4)
            {
                content += "p";
                levels += 1;
            }
        }
        return content;
    }

    public List<MapNode> GetNeighbors(MapNode currentNode){
        int x = currentNode.x;
        int y = currentNode.y;
        List<MapNode> neighbors = new List <MapNode>();

        for(int yBounds = -1; yBounds <= 1; yBounds += 1){
            for(int xBounds = -1; xBounds <= 1; xBounds += 1){
                if(xBounds == 0 && yBounds == 0){
                    continue;
                }
                if(IsWithinBounds(x + xBounds, y + yBounds)){
                    MapNode node = nodes[y + yBounds][x + xBounds];
                    if (node != null) { 
                        //node.movementCost = (xBounds == 0 || yBounds == 0) ? 10 : 14;
                        neighbors.Add(node);
                    }
                    else
                    {
                        neighbors.Add(new MapNode(-1, -1, false));
                    }
                }
                else
                {
                    neighbors.Add(new MapNode(-1, -1, false));
                }
            }
        }
        return neighbors;
    }

    bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width) && (y >= 0 && y < height);
    }

    public string PrintableString(){
        string content = "";
        for (int i = 0; i < height; i += 1)
        {
            for (int j = 0; j < width; j += 1)
            {
                if (nodes[i][j] != null) { 
                    if (nodes[i][j].isWalkable)
                    {
                        content += ".";
                    }
                    else
                    {
                        content += "x";
                    }
                }
                else
                {
                    content += " ";
                }
            }
            content += "\n";
        }
        return content;
    }
}
