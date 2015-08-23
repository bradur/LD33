using UnityEngine;
using System.Collections;

public class Villain : SpawnedItemBase {

    Vector3 targetPos;
    Vector3 cachedPos;
    float moveTimer = 0;
    float moveDuration = 2f;
    bool moving = false;
    int moves = 0;
    Vector3 originalPos;
    public bool moveAgain = false;
    int health = 3;

    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {
        if (moving)
        {
            moveTimer += Time.deltaTime / moveDuration;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveTimer);
            //Debug.Log(transform.position + "->" + targetPos + "(" + moveTimer + ")");
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                if (moves >= 1)
                {
                    moving = false;
                    targetPos = cachedPos;
                    moveTimer = 0f;
                    hero.HitVillain(this);
                    //moves = 0;
                }
                else
                {
                    moves += 1;
                    moveTimer = 0f;
                    targetPos = originalPos;
                }
            }
        }
    }

    public void GetHit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            //Debug.Log("I am dead!");
            StopAllCoroutines();
            hero.OpponentDied();
            Kill();
        }
    }


    IEnumerator CombatHero()
    {
        while (moveAgain == false)
        {
            yield return 5;
        }
        moveAgain = false;
        moves = 0;
        moving = true;
        StartCoroutine("CombatHero");
    }

    public override void InteractWithHero(Hero hero)
    {
        originalPos = transform.position;
        /*Vector3 heroPos = new Vector3(hero.transform.position.x, 0f, hero.transform.position.y);
        Vector3 heading = heroPos - transform.position;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;*/
        targetPos = (transform.position + hero.transform.position) / 2;
        targetPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
        cachedPos = targetPos;
        this.hero = hero;
        moving = true;
        StartCoroutine("CombatHero");
    }
}
