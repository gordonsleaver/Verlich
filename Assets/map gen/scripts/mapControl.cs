using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Pathfinding;

public class mapControl : MonoBehaviour
{
    public List<GameObject> rooms;
    public List<GameObject> activeRooms;
    //change the active enemys to the area where we load the next level--------------------!
    public List<GameObject> allEnemys;

    public GameObject startRoom;
    public float startTime;
    public float activeTime;
    public bool bossSpawn;
    public GameObject bossRoom;
    public GameObject shopRoom;
    public GameObject shop;
    public GraphCollision collision;
    public int curLevel;
    public int enemyMulti;
    public List<GameObject> activeEnemys;
    public List<GameObject> bossAll;
    public List<GameObject> bossActive;
    public List<GameObject> decoItems;
    public GameObject plrPre;
    public GameObject plr;
    public bool firstLoad;
    GridGraph gg;

    //temp vars for testing
    public float loadTimeMax;
    public float loadTimeCur;

    // Start is called before the first frame update, initizalizes script and sets up properties
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (firstLoad)
        {
            firstLoad = false;
            curLevel = 0;
            activeTime = startTime;
            plr = Instantiate(plrPre, new Vector3(0, 0, 0), Quaternion.identity);
            resetLevel();
        }  
        
    }

    // Update is called once per frame, updates timers and finishes spawning
    void Update()
    {
        if(activeTime <= 0 && !bossSpawn && activeRooms[activeRooms.Count - 1])
        {
            bossSpawn = true;
            activeRooms[activeRooms.Count - 1].GetComponent<roomProps>().setBossRoom(true);
            bossRoom = activeRooms[activeRooms.Count - 1];
            shopRoom = activeRooms[UnityEngine.Random.Range(0, activeRooms.Count - 2)];
            if (shopRoom && shopRoom.GetComponent<roomProps>())
            {
                shopRoom.GetComponent<roomProps>().setShopRoom(true);
                shopRoom.GetComponent<roomProps>().setShop(shop);

                createGrid();
                enemySpawns();
                bossSpawning();
            }
        }
        else
        {
            activeTime -= Time.deltaTime;
        }


        if (Input.GetKey("p") && loadTimeCur <= 0)
        {
            loadTimeCur = loadTimeMax;
            resetLevel();
        }
        else
        {
            loadTimeCur -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Sets current depth of level
    /// </summary>
    /// <param name="amt"></param>
    public void setDepth(int amt)
    {
        curLevel = amt-1;
        resetLevel();
    }

    /// <summary>
    /// Goes to the next level
    /// </summary>
    public void resetLevel()
    {
        curLevel++;
        plr.transform.GetChild(0).position = new Vector3(0, 0, 0);
        plr.transform.GetChild(0).GetComponent<Player>().depthChange(curLevel);
        foreach (GameObject dest in activeRooms)
        {
            Destroy(dest);
        }
        activeRooms.Clear();
        //activeEnemys.Clear();

        startRoom = GameObject.Find("startRoom");
        bossSpawn = false;
        activeTime = startTime;
        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].GetComponent<roomProps>().resetWeight();
        }
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// finds a random room that has the right opening
    /// </summary>
    /// <param name="opening">tells what direction the rooms opening needs to be</param>
    /// <returns>returns the room the spawner needs to use</returns>
    public GameObject getRoom(int opening)
    {
        activeTime = startTime;
        List<GameObject> roomsNext = new List<GameObject>();
        int weightSum = 0;
        for (int i = 0; i < rooms.Count; i++)
        {
            bool validOpen;

            if (opening == 1)
            {
                //the room needs to have a door going down

                validOpen = rooms[i].GetComponent<roomProps>().getDown();
            }
            else if (opening == 2)
            {
                //the room needs to have a door going left

                validOpen = rooms[i].GetComponent<roomProps>().getLeft();
            }
            else if (opening == 3)
            {
                //the room needs to have a door going up

                validOpen = rooms[i].GetComponent<roomProps>().getUp();
            }
            else if (opening == 4)
            {
                //the room needs to have a door going right

                validOpen = rooms[i].GetComponent<roomProps>().getRight();
            }
            else
            {
                Debug.Log("invalid spawn opening");
                validOpen = false;
            }

            if (validOpen)
            {
                weightSum += rooms[i].GetComponent<roomProps>().getCurWeight();
                roomsNext.Add(rooms[i]);
            }
        }

        int rndWeight = UnityEngine.Random.Range(0, weightSum);
        int j = -1;
        while(rndWeight > 0)
        {
            j++;
            rndWeight -= roomsNext[j].GetComponent<roomProps>().getCurWeight();
            
        }
        if(j == -1)
        {
            j = 0;
        }

        changeWeight();

        return roomsNext[j];
    }

    /// <summary>
    /// adds the new room that spawned to a list of rooms
    /// </summary>
    /// <param name="newRoom">new room spawned</param>
    public void trackRoom(GameObject newRoom)
    {
        activeRooms.Add(newRoom);
    }

    /// <summary>
    /// tells the rooms weights to change
    /// </summary>
    public void changeWeight()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].GetComponent<roomProps>().changeWeight();
        }
    }

    void createGrid()
    {
        float xMax = 0;
        float yMax = 0;
        float xMin = 0;
        float yMin = 0;
        float xWidth = 0;
        float yWidth = 0;

        for (int i = 0; i < activeRooms.Count; i++)
        {
            if (activeRooms[i])
            {
                if (activeRooms[i].transform.position.x > xMax)
                {
                    xMax = activeRooms[i].transform.position.x;
                }

                if (activeRooms[i].transform.position.y > yMax)
                {
                    yMax = activeRooms[i].transform.position.y;
                }

                if (activeRooms[i].transform.position.x < xMin)
                {
                    xMin = activeRooms[i].transform.position.x;
                }

                if (activeRooms[i].transform.position.y < yMin)
                {
                    yMin = activeRooms[i].transform.position.y;
                }
            }
        }

        // This holds all graph data
        AstarData data = AstarPath.active.data;

        // This creates a Grid Graph
        if (gg == null)
        {
            gg = data.AddGraph(typeof(GridGraph)) as GridGraph;
        }   

        gg.is2D = true;

        if(xMin * -1 > xMax)
        {
            xWidth = xMin;
        }
        else
        {
            xWidth = xMax;
        }
        if (yMin * -1 > yMax)
        {
            yWidth = yMin;
        }
        else
        {
            yWidth = yMax;
        }

        // Setup a grid graph with some values
        int width = (int)(((int)xWidth + 10) * 2.5f);
        int depth = (int)(((int)yWidth + 10) * 2.5f);
        float nodeSize = 1f;

        gg.center = new Vector3(((xMin + xMax)/2), ((yMin + yMax) / 2), 0);

        // Updates internal size from the above values
        gg.SetDimensions(width, depth, nodeSize);

        gg.collision = collision;

        // Scans all graphs
        AstarPath.active.Scan();
    }

    // Handles spawning
    void enemySpawns()
    {
        activeEnemys.Clear();

        for (int i = 0; i < allEnemys.Count; i++)
        {
            if (allEnemys[i].GetComponent<basicBehave>().getStartLevel() <= curLevel)
            {
                activeEnemys.Add(allEnemys[i]);
            }
        }

        int overPoints = curLevel * enemyMulti;
        int perPoints = overPoints / (activeRooms.Count);

        for (int i = 0; i < activeRooms.Count; i++)
        {
            if (activeRooms[i])
            {
                activeRooms[i].GetComponent<roomProps>().setPoints(perPoints);
                activeRooms[i].GetComponent<roomProps>().setEnemyPool(activeEnemys);
                activeRooms[i].GetComponent<roomProps>().setDecoItems(decoItems);
            }
        }

    }

    void bossSpawning()
    {
        bossActive.Clear();

        for (int i = 0; i < bossAll.Count; i++)
        {
            if (bossAll[i].GetComponent<basicBehave>().getStartLevel() <= curLevel)
            {
                bossActive.Add(bossAll[i]);
            }
        }

        for (int i = 0; i < activeRooms.Count; i++)
        {
            if (activeRooms[i])
            {
                activeRooms[i].GetComponent<roomProps>().setBossPool(bossActive);
            }
        }
    }

    /// <summary>
    /// Gets the current depth
    /// </summary>
    /// <returns></returns>
    public int getCurLevel()
    {
        return curLevel;
    }
}
