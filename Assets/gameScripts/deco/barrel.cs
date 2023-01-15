using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barrel : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update, sets the player value
    void Start()
    {
        player = GameObject.Find("Player");
    }

    /// <summary>
    /// Destroys the barrel when called and updates pathfinding
    /// </summary>
    public void destroyBarrel()
    {
        gameObject.SetActive(false);

        Bounds b;

        b = new Bounds(gameObject.transform.position, new Vector3(20, 20, 1));

        AstarPath.active.UpdateGraphs(b);

        int temp = UnityEngine.Random.Range(0,100);

        if (temp >= 80)
        {
            player.GetComponent<Player>().addMoney(5.0f);
        }

        Destroy(gameObject);
    }
}
