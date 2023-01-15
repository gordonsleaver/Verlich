using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicProjectile : MonoBehaviour
{
    public int atkDmg;
    public float speed;
    Rigidbody2D rigidbody2d;

    // Start is called before the first frame update, sets properties and adds force to the rigidbody
    void Start()
    {
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        rigidbody2d.AddForce(transform.right * speed);
    }

    /// <summary>
    /// Sets variables
    /// </summary>
    /// <param name="ad"></param>
    /// <param name="s"></param>
    public void setVars(int ad, float s)
    {
        atkDmg = ad;
        speed = s;
    }

    /// <summary>
    /// Handles collision and damage
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<Player>().changeHealth(atkDmg);
            Destroy(gameObject);
        }
        else if (col.gameObject.tag == "wall")
        {
            Destroy(gameObject);
        }
    }
}
