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
    MapNode currentGlobalNode;
    MapNode destination;
    List<MapNode> finalPath = new List<MapNode>();
    List<MapNode> destinations = new List<MapNode>();
    Vector3 targetPosition;
    //float movementInterval = 0.01f;
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
        currentGlobalNode = map.GetNode(x, y);
        //debugText = GameObject.FindGameObjectWithTag("DebugText").GetComponent<Text>();
        //debugText.text = map.NeighborsToString(currentNode);
        thisTransform = GetComponent<Transform>();
        destinations.Add(map.GetNode(endX, endY));

        //SearchPath(currentNode);
        GameObject[] treasures = GameObject.FindGameObjectsWithTag("Gold");

        List<GameObject> sortedTransforms = new List<GameObject>();
        for (int i = 0; i < treasures.Length; i += 1)
        {
            sortedTransforms.Add(treasures[i]);
        }
        sortedTransforms.Sort(delegate(GameObject c1, GameObject c2){
            return Vector3.Distance(this.transform.position, c1.transform.position).CompareTo(
                    Vector3.Distance(this.transform.position, c2.transform.position)
            );
        });
        for (int i = sortedTransforms.Count - 1; i > -1; i -= 1)
        {
            GameObject gold = sortedTransforms[i];
            //Debug.Log(Vector3.Distance(this.transform.position, gold.transform.position));
            MapNode node = map.GetNodeNormalized(gold.transform.position.x, gold.transform.position.z);
            //Debug.Log("NODE: ["+node.x +","+ node.y + "]");
            destinations.Add(node);
        }
        destination = destinations[destinations.Count - 1];
        destinations.RemoveAt(destinations.Count - 1);
        openNodes.Add(currentGlobalNode);
        SearchPath();
        
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

    /*void SetDestination(MapNode destination)
    {
        this.destination = destination;
    }*/

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
                hero.ProcessNodeItem(currentGlobalNode, currentTargetNode);

                //Debug.Log("process!");
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
            if (destinations.Count > 0)
            {
                destination = destinations[destinations.Count - 1];
                destinations.RemoveAt(destinations.Count - 1);
                openNodes.Add(currentGlobalNode);
                SearchPath();
            }
            else
            {
                gameManager.LevelFinished();
            }
            
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
                currentGlobalNode = currentTargetNode;
                movementTimer = 0;
                //moving = false;
                NextNode();

            }
        }
    }

    void SetFinalPath(MapNode node)
    {
        finalPath = new List<MapNode>();
        int i = 300;

        finalPath.Add(node);
        while (true)
        {
            MapNode parentNode = node.GetParent();
            if (parentNode != null)
            {
                node.ClearNode();
                //                Debug.Log("[" + parentNode.x + ", " + parentNode.y + "]");
                finalPath.Add(parentNode);
                node = parentNode;
            }
            else
            {
                node.ClearNode();
                //finalPath.RemoveAt(finalPath.Count-1);
                break;
            }
            if (i < 0)
            {
                Debug.Log("Loop fail?!");
                break;
            }
            i -= 1;
        }
        NextNode();
        StartCoroutine("Move");
        openNodes = new List<MapNode>();
        closedNodes = new List<MapNode>();
    }

    // IEnumerator SearchPath
    void SearchPath()
    {
        //yield return new WaitForSeconds(1f);
        //debugText.text = map.NeighborsToString(currentNode);
        //openNodes.Add(node);                                             // add current node to open list
        //node.list = "o";
        MapNode lowestCostNode = null;
        bool targetFound = false;

        foreach (MapNode openNode in openNodes)
        {
            if (lowestCostNode == null || openNode.fullCost < lowestCostNode.fullCost)
            {
                lowestCostNode = openNode;
            }
        }
        MapNode currentNode = lowestCostNode;
        openNodes.Remove(currentNode);

        if (currentNode == destination)
        {
            targetFound = true;
            SetFinalPath(currentNode);
        }
        else
        {
            closedNodes.Add(currentNode);

            List<MapNode> neighbors = map.GetNeighbors(currentNode);
            foreach (MapNode neighbor in neighbors)             // go through neighbors
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
                currentNode.list = ".";
                currentNode.list = "c";
                if (openNodes.Count > 0)
                {
                    lowestCostNode.list = ".";
                    //SearchPath(lowestCostNode);
                    //Debug.Log("Starting another Coroutine!");
                    SearchPath();
                    //StartCoroutine("SearchPath", currentNode)
                }
                else
                {
                    Debug.Log("No path found!");
                }
            }
        }
    }

}
