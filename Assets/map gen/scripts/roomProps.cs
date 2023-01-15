using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// store the info on type of room and how its weight/spawn chance changes
/// </summary>
public class roomProps : MonoBehaviour
{
    public bool up;
    public bool left;
    public bool down;
    public bool right;

    public int startWeight;
    public int increment;
    public int maxWeight;
    public int decrement;
    public int curWeight;
    public bool hitMax;

    public int enemyPoints;
    public List<GameObject> enemyPool;
    public List<GameObject> bossPool;
    public bool spawned;

    public List<GameObject> decoItems;

    public GameObject shop;

    public bool bossRoom;
    public bool shopRoom;

    // Gets and sets values of properties
    public bool getUp()
    {
        return up;
    }

    public bool getLeft()
    {
        return left;
    }

    public bool getDown()
    {
        return down;
    }

    public bool getRight()
    {
        return right;
    }

    public int getStartWeight()
    {
        return startWeight;
    }

    public int getIncrement()
    {
        return increment;
    }

    public int getMaxWeight()
    {
        return maxWeight;
    }

    public int getDecrement()
    {
        return decrement;
    }

    public int getCurWeight()
    {
        return curWeight;
    }

    public void setBossRoom(bool boss)
    {
        bossRoom = boss; 
    }

    public bool getBossRoom()
    {
        return bossRoom;
    }

    public void setShopRoom(bool s)
    {
        shopRoom = s;
    }

    public void setShop(GameObject s)
    {
        shop = s;
    }

    public void setPoints(int p)
    {
        enemyPoints = p;
    }

    public void setEnemyPool(List<GameObject> activeEnemys)
    {
        for (int i = 0; i < activeEnemys.Count; i++)
        {
            enemyPool.Add(activeEnemys[i]);
        }
    }

    public void setBossPool(List<GameObject> bp)
    {
        for (int i = 0; i < bp.Count; i++)
        {
            bossPool.Add(bp[i]);
        }
    }

    public void setDecoItems(List<GameObject> di)
    {
        decoItems = di;
    }
    ////////////////////////////////////////////////////////////
    /// <summary>
    /// sets the weights when changing floors and on strart up
    /// </summary>
    public void resetWeight()
    {
        curWeight = startWeight;
        hitMax = false;
    }

    /// <summary>
    /// changes the weights up or down depending on the what type of room it is
    /// </summary>
    public void changeWeight()
    {
        if (curWeight >= maxWeight)
        {
            hitMax = true;
        }

        if (hitMax)
        {
            curWeight -= decrement;
        }
        else
        {
            curWeight += increment;
        }
        if(curWeight <= 0)
        {
            curWeight = 1;
        }
    }

    /// <summary>
    /// starts the spawn of enmeies when the player enters the room 
    /// </summary>
    /// <param name="col">checks to make sure it is the player</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player" && !spawned)
        {
            if (!bossRoom && !shopRoom)
            {
                int weightSum = 1;

                for (int i = 0; i < enemyPool.Count; i++)
                {
                    weightSum += enemyPool[i].GetComponent<basicBehave>().getWeight();
                }

                while (enemyPoints > 0 && enemyPool.Count > 0)
                {
                    int rndWeight = UnityEngine.Random.Range(0, weightSum);

                    int j = -1;
                    while (rndWeight > 0)
                    {
                        j++;
                        rndWeight -= enemyPool[j].GetComponent<basicBehave>().getWeight();
                    }
                    if (j == -1)
                    {
                        j = 0;
                    }

                    enemyPoints -= enemyPool[j].GetComponent<basicBehave>().getPointCost();
                    if (enemyPoints < 0)
                    {
                        enemyPoints += enemyPool[j].GetComponent<basicBehave>().getPointCost();
                        weightSum -= enemyPool[j].GetComponent<basicBehave>().getWeight();
                        enemyPool.RemoveAt(j);
                    }
                    else
                    {
                        spawnEnemy(enemyPool[j]);
                    }
                }
            }
            else if(bossRoom)
            {
                int weightSum = 1;

                for (int i = 0; i < bossPool.Count; i++)
                {
                    weightSum += bossPool[i].GetComponent<basicBehave>().getWeight();
                }

                int rndWeight = UnityEngine.Random.Range(0, weightSum);

                int j = -1;
                while (rndWeight > 0)
                {
                    j++;
                    rndWeight -= bossPool[j].GetComponent<basicBehave>().getWeight();
                }
                if (j == -1)
                {
                    j = 0;
                }

                spawnEnemy(bossPool[j]);
            }
            else if (shopRoom)
            {
                Instantiate(shop, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            }

            for (int i = 0; i < 4; i++)
            {
                spawnDeco();
            }
            

            Bounds b;

            b = new Bounds(gameObject.transform.position, new Vector3(20, 20, 1));

            AstarPath.active.UpdateGraphs(b);

            spawned = true;
            
        }

    }

    /// <summary>
    /// Spawns a enemy
    /// </summary>
    /// <param name="enemy"></param>
    void spawnEnemy(GameObject enemy)
    {
        float rndPosX = UnityEngine.Random.Range(transform.position.x - 3, transform.position.x + 3);
        float rndPosY = UnityEngine.Random.Range(transform.position.y - 3, transform.position.y + 3);

        Instantiate(enemy, new Vector3(rndPosX, rndPosY, 0), Quaternion.identity);
    }

    /// <summary>
    /// Spawns decoration
    /// </summary>
    void spawnDeco()
    {
        float rndPosX = UnityEngine.Random.Range(transform.position.x - 7f, transform.position.x + 7f);
        float rndPosY = UnityEngine.Random.Range(transform.position.y - 7f, transform.position.y + 7f);

        while (rndPosX >= transform.position.x - 4 && rndPosX <= transform.position.x + 4)
        {
            rndPosX = UnityEngine.Random.Range(transform.position.x - 7f, transform.position.x + 7f);
        }

        while (rndPosY >= transform.position.y - 4 && rndPosY <= transform.position.y + 4)
        {
            rndPosY = UnityEngine.Random.Range(transform.position.y - 7f, transform.position.y + 7f);
        }
        if (decoItems.Any())
        {
            Instantiate(decoItems[0], new Vector3(rndPosX, rndPosY, 0), Quaternion.identity);
        }
    }

}
