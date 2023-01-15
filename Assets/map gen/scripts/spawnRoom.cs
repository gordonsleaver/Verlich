using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


/// <summary>
/// script for invisible spawners to create the next room in the right orientation 
/// </summary>
public class spawnRoom : MonoBehaviour
{
    public int openDirection;
    public GameObject mapControl;
    public bool spawned;
    public GameObject walledOff;

    /// <summary>
    /// grabs the main script for finding what room to spawn 
    /// </summary>
    void Start()
    {
        mapControl = GameObject.Find("mapControl");

        Invoke("spawn", 0.1f);
    }

    /// <summary>
    /// grabs a random room then spawns it
    /// </summary>
    void spawn()
    {
        if (!spawned)
        {
            GameObject newRoom = mapControl.GetComponent<mapControl>().getRoom(openDirection);
            GameObject spawnedRoom = Instantiate(newRoom, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            spawned = true;
            mapControl.GetComponent<mapControl>().trackRoom(spawnedRoom);
        }
    }


    /// <summary>
    /// checks to see if another room/roomSpawner is there and if both need to activate than spawns a blank room
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("spawnpoint"))
        {
            if (other.GetComponent<spawnRoom>().spawned == false && spawned == false)
            {
                Invoke("spawnWall", 0.05f);
                
            }
            else
            {
                spawned = true;
            }
            
        }
    }

    /// <summary>
    /// Spawnes a wall where the transform is located
    /// </summary>
    void spawnWall()
    {
        if (!spawned)
        {
            Instantiate(walledOff, transform.position, Quaternion.identity);
            spawned = true;
        }
    }
    /*
    void createGrid()
    {
        // This holds all graph data
        AstarData data = AstarPath.active.data;

        // This creates a Grid Graph
        GridGraph gg = data.AddGraph(typeof(GridGraph)) as GridGraph;

        gg.is2D = true;

        // Setup a grid graph with some values
        int width = 40;
        int depth = 40;
        float nodeSize = 0.5f;

        gg.center = gameObject.transform.position;

        // Updates internal size from the above values
        gg.SetDimensions(width, depth, nodeSize);

        // Scans all graphs
        //AstarPath.active.Scan();
    }
    */
}
    