using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minotaur : MonoBehaviour
{
    public int atkDmg;
    public float windUpMax;
    public bool ramming;
    public int ramSpeed;
    Rigidbody2D rigidbody2d;
    public float distance;
    List<RaycastHit2D> hit = new List<RaycastHit2D>();
    public ContactFilter2D Filter;
    public bool canSee;


    // Start is called before the first frame update, updates basic properties
    void Start()
    {
        atkDmg = gameObject.GetComponent<basicBehave>().getAtkDmg();
        windUpMax = gameObject.GetComponent<basicBehave>().getWindUpMax();
        ramming = false;
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame, checks if it can see the player
    void Update()
    {
        hit.Clear();
        int trash = Physics2D.Raycast(transform.position, transform.right, Filter, hit, distance);

        if (hit[0].transform.gameObject.tag == "Player")
        {
            if (!canSee)
            {
                canSee = true;
                gameObject.SendMessage("canSee", true);
            }
        }
        else if (canSee)
        {
            canSee = false;
            gameObject.SendMessage("canSee", false);
        }
    }

    public void attack()
    {
        StartCoroutine(fire());
    }

    /// <summary>
    /// Starts the boss attack and handles all cooldowns
    /// </summary>
    /// <returns></returns>
    public IEnumerator fire()
    {
        gameObject.GetComponent<basicBehave>().setAtkTime();
        float windUpCur = windUpMax;
        gameObject.GetComponent<basicBehave>().setRotate(false);
        gameObject.SendMessage("windingUp", false);// tells pathfinding to not move 
        yield return new WaitForSeconds(windUpCur);

        ramming = true;
        while (ramming)
        {
            rigidbody2d.AddForce(transform.right * ramSpeed);
            yield return new WaitForSeconds(0.01f);
        }
        rigidbody2d.velocity = Vector3.zero;
        rigidbody2d.angularVelocity = 0;

        gameObject.GetComponent<basicBehave>().setRotate(true);
        gameObject.SendMessage("windingUp", true);//tells pathfinding to move 
        yield break;
    }

    /// <summary>
    /// Handles collision and attacks to the player
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<Player>().changeHealth(atkDmg);
            ramming = false;

        }
        else if (col.gameObject.tag == "wall")
        {
            ramming = false;
        }
    }
}
