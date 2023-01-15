using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shop : MonoBehaviour
{
    public List<GameObject> shopItems;
    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;

    // Start is called before the first frame update, spawns in 3 items
    void Start()
    {
        spawnItem(slot1);
        spawnItem(slot2);
        spawnItem(slot3);
    }

    /// <summary>
    /// Spawns in a random item
    /// </summary>
    /// <param name="slot"></param>
    public void spawnItem(GameObject slot)
    {
        int weightSum = 1;

        for (int i = 0; i < shopItems.Count; i++)
        {
            weightSum += shopItems[i].GetComponent<shopStats>().getWeight();
        }

        int rndWeight = UnityEngine.Random.Range(0, weightSum);

        int j = -1;
        while (rndWeight > 0)
        {
            j++;
            rndWeight -= shopItems[j].GetComponent<shopStats>().getWeight();
        }
        if (j == -1)
        {
            j = 0;
        }

        Instantiate(shopItems[j], slot.transform.position, Quaternion.identity);
    }
}
