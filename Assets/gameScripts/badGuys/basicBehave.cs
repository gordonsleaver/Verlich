using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class basicBehave : MonoBehaviour
{
    public GameObject player;
    public GameObject portal;
    public int atkDmg;
    public float atkCooldown;
    public float atkTime;
    public Transform atkPos;
    public float range;
    public LayerMask atkMask;
    public bool canRotate;
    public float windUpMax;
    public int maxHealth;
    public int health;
    public int xpWorth;
    public float avgMoney;
    public GameObject mapControl;
    public int curLevel;
    public float multi;
    public bool ranged;

    public int weight;
    public int pointCost;
    public int startLevel;

    public bool isBoss;
    public MonoBehaviour bossScript;

    // Start is called before the first frame update, sets properties
    void Start()
    {
        mapControl = GameObject.Find("mapControl");
        curLevel = mapControl.GetComponent<mapControl>().getCurLevel();
        multi = Mathf.Pow(curLevel, 0.6f);
        player = GameObject.Find("Player");
        atkTime = atkCooldown;
        canRotate = true;
        maxHealth = (int)Mathf.Round(maxHealth * multi);
        xpWorth = (int)Mathf.Round(xpWorth * multi);
        atkDmg = (int)Mathf.Round(atkDmg * multi);
        health = maxHealth;

        

        if (ranged)
        {
            gameObject.GetComponent<basicRanged>().setVars(atkDmg, windUpMax);
        }
    }

    // Update is called once per frame, updates timers and sets rotation
    void Update()
    {
        atkTime -= Time.deltaTime;
        rotateToPlr();

    }

    // Returns properties
    public int getWeight()
    {
        return weight;
    }
    public int getStartLevel()
    {
        return startLevel;
    }
    public int getPointCost()
    {
        return pointCost;
    }
    public int getAtkDmg()
    {
        return atkDmg;
    }
    public float getWindUpMax()
    {
        return windUpMax;
    }

    /// <summary>
    /// Attacks player when reached target
    /// </summary>
    public void targetReached()
    {
        if (atkTime <= 0)
        {
            if (isBoss)
            {
                bossScript.Invoke("attack", 0);
            }
            else if (ranged)
            {
                gameObject.GetComponent<basicRanged>().attack();
            }
            else
            {
                StartCoroutine(atkPlayer());
            }
        }
        
    }

    /// <summary>
    /// Handles damage calculation and hitboxes
    /// </summary>
    /// <returns></returns>
    public IEnumerator atkPlayer()
    {
        setAtkTime();
        float windUpCur = windUpMax;
        setRotate(false);
        gameObject.SendMessage("windingUp", false);// tells pathfinding to not move 
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(windUpCur);

        Collider2D[] toAtk = Physics2D.OverlapCircleAll(atkPos.position, range, atkMask);
        for (int i = 0; i < toAtk.Length; i++)
        {
            if (toAtk[i].tag == "Player")
            {
                player.GetComponent<Player>().changeHealth(atkDmg);
            }
        }
        setRotate(true);
        gameObject.SendMessage("windingUp", true);//tells pathfinding to move 
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        yield break;
    }

    // Sets certain properties
    public void setRotate(bool cr)
    {
        canRotate = cr;
    }

    public void setAtkTime()
    {
        atkTime = atkCooldown;
    }

    /// <summary>
    /// Rotates the enemy to face the player
    /// </summary>
    void rotateToPlr()
    {
        if (canRotate && player)
        {
            Vector3 difference = player.transform.position - transform.position;
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
        }
        
    }

    /// <summary>
    /// Sets the enemies health and kills it if it reaches 0
    /// </summary>
    /// <param name="healthChange"></param>
    public void changeHealth(int healthChange)
    {
        health += healthChange;

        if(health > maxHealth)
        {
            health = maxHealth;
        }
        if(health <= 0)
        {
            death();
        }
    }

    /// <summary>
    /// Rewards the player and destroys the enemy
    /// </summary>
    public void death()
    {
        float rndMoney = (Mathf.Round(UnityEngine.Random.Range(avgMoney * 0.80f, avgMoney * 1.20f) * 100.00f)) / 100.00f;
        player.GetComponent<Player>().addMoney(rndMoney);
        player.GetComponent<Player>().addXp(xpWorth);
        if (isBoss)
        {
            Instantiate(portal, gameObject.transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    //draws the circle for where the enemy attacks
    /*
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(atkPos.position, range);
    }
    */



}
