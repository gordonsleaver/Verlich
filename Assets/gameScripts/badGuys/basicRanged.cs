using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class basicRanged : MonoBehaviour
{
    public float distance;
    List<RaycastHit2D> hit = new List<RaycastHit2D>();
    public ContactFilter2D Filter;
    public bool canSee;
    public GameObject projectile;
    public int atkDmg;
    public float projectileSpeed;
    public float windUpMax;

    /// <summary>
    /// Checks if it can see the player
    /// </summary>
    void Update()
    {
        hit.Clear();
        int trash = Physics2D.Raycast(transform.position, transform.right, Filter, hit, distance);

        if (hit.Any())
        {

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
    }

    // Sets variables
    public void setVars(int ad, float wum)
    {
        atkDmg = ad;
        windUpMax = wum;
    }

    // Runs the process of attacking
    public void attack()
    {
        StartCoroutine(fire());
    }

    public IEnumerator fire()
    {
        gameObject.GetComponent<basicBehave>().setAtkTime();
        float windUpCur = windUpMax;
        gameObject.GetComponent<basicBehave>().setRotate(false);
        gameObject.SendMessage("windingUp", false);// tells pathfinding to not move 
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(windUpCur);

        GameObject firedProjct = Instantiate(projectile, gameObject.transform.position, gameObject.transform.rotation);
        firedProjct.GetComponent<basicProjectile>().setVars(atkDmg, projectileSpeed);

        gameObject.GetComponent<basicBehave>().setRotate(true);
        gameObject.SendMessage("windingUp", true);//tells pathfinding to move 
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        yield break;
    }

    /// <summary>
    /// Draws gizmos
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * 20);
    }
}
