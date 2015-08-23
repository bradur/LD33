using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

    public HeroMovement movement;
    Map map;
    int startX;
    int startY;

    Vector3 hitTargetPos;
    Vector3 originalPos;
    Vector3 cachedPos;
    bool fighting = false;
    bool fightAgain = false;
    float moveTimer = 0f;
    float moveDuration = 2f;
    int moves = 0;
    public bool fightIsOver = true;
    Villain targetItem;

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

    public void ProcessNodeItem(MapNode heroNode, MapNode node)
    {
        fightIsOver = false;
        node.item.InteractWithHero(this);
        // do smth
        // and finally unset item
    }

    public void HitVillain(Villain spawnedItem)
    {
        if (targetItem == null)
        {
//            Debug.Log("Fight another day!");
            originalPos = transform.position;
            targetItem = spawnedItem;
            hitTargetPos = (transform.position + spawnedItem.transform.position) / 2;
            hitTargetPos = new Vector3(hitTargetPos.x, transform.position.y, hitTargetPos.z);
            cachedPos = hitTargetPos;
            StartCoroutine("CombatVillain");
            fighting = true;
        }
        else
        {
            fightAgain = true;
        }
        
    }

    IEnumerator CombatVillain()
    {
        while (fightAgain == false)
        {
            yield return 5;
        }
        if (targetItem == null)
        {
            StopAllCoroutines();
        }
        fightAgain = false;
        moves = 0;
        moveTimer = 0f;
        fighting = true;
        StartCoroutine("CombatVillain");
    }

    IEnumerator RestAfterCombat()
    {
        yield return new WaitForSeconds(1f);
        fightIsOver = true;
    }

    public void OpponentDied()
    {
//        Debug.Log("opponent died!");
        targetItem = null;
        fightAgain = false;
        fighting = false;
        moves = 0;
        StopAllCoroutines();
        moveTimer = 0f;
        StartCoroutine("RestAfterCombat");
    }

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
        if (fighting)
        {
            moveTimer += Time.deltaTime / moveDuration;
            transform.position = Vector3.MoveTowards(transform.position, hitTargetPos, moveTimer);
            //Debug.Log(transform.position + "->" + hitTargetPos + "(" + moveTimer + ")" + "dist: " + Vector3.Distance(transform.position, hitTargetPos));
            if (Vector3.Distance(transform.position, hitTargetPos) < 0.1f)
            {
                if (moves >= 1)
                {
                    fighting = false;
                    if (targetItem != null)
                    {
                        //Debug.Log("Let him move again...");
                        targetItem.GetHit(1);
                        if (targetItem != null)
                        {
                            targetItem.moveAgain = true;
                        }
                    }
                    hitTargetPos = cachedPos;
                    //moveTimer = 0f;
                }
                else
                {
                    moves += 1;
                    moveTimer = 0f;
                    hitTargetPos = originalPos;
                }
            }
        }
    }
}
