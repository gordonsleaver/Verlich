using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Xml;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

// Makes sure it can get the PlayerInput script
[RequireComponent(typeof(PlayerInput))]

public class Player : MonoBehaviour
{
    public float baseMoveSpeed;
    public float moveSpeed;
    public bool canMove;
    public float collisionHitBox;
    public ContactFilter2D movementFilter;
    public string currentCharacter;
    public float dashTime;
    public float dashSpeed;
    public float dashCooldown;
    public int baseHealthMax;
    public int healthMax;
    public int health;
    public GameObject sword;
    public GameObject pressInp;
    public GameObject damageIndicator;
    public bool canAtk;
    public int swingSpeed;
    public int baseDmg;
    public int incDmg;
    public int dmg;
    public int xpTotal;
    public int level;
    public int xpNextLevel;
    private int skillPoints;
    public float money;
    public GameObject hitEffect;
    public GameObject swingEffect;
    public GameObject buyEffect;

    public GameObject stats;

    public float loadTimeMax;
    public float loadTimeCur;

    private Rigidbody2D rb;
    private TrailRenderer tr;
    private Transform tf;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private List<Collider2D> curColliders = new List<Collider2D>();
    private bool checkedColliders = false;
    private PlayerInput playerInput;
    [SerializeField] private float currentDashTime = -5f;
    private Dictionary<string, Dictionary<string, string>> playerSkills = new Dictionary<string, Dictionary<string, string>>();

    // Awake is called when script is initialized, updates input property
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update, updates properties
    private void Start()
    {
        DontDestroyOnLoad(gameObject.transform.parent);
        xpTotal = 0;
        level = 0;
        xpNextLevel = 10;
        healthMax = baseHealthMax;
        health = healthMax;
        dmg = baseDmg;
        moveSpeed = baseMoveSpeed;
        sword.GetComponent<plrSword>().setDmg(dmg);
        rb = GetComponent<Rigidbody2D>();
        tf = GetComponent<Transform>();
        tr = GetComponentsInChildren<TrailRenderer>()[0];
    }

    /// <summary>
    /// When called moves the player forward to make them dash
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    IEnumerator Dash(Vector2 direction)
    {
        float dsBuff = 0;
        List<string> buffs = checkForSkillType("buffDashSpeed");
        foreach (var v in buffs)
        {
            dsBuff += float.Parse(v);
        }

        if (currentDashTime + (dashCooldown - dsBuff) <= 0f)
        {
            currentDashTime = dashTime; // Reset the dash timer.
            rb.freezeRotation = true;
            tr.emitting = true;
            canMove = false;
 
            float dlBuff = 0;
            List<string> buffs2 = checkForSkillType("buffDashLength");
            foreach (var v in buffs2)
            {
                dlBuff += Convert.ToInt32(v);
            }

            while (currentDashTime + (dashCooldown - dsBuff) > 0f)
            {
                currentDashTime -= Time.fixedDeltaTime; // Lower the dash timer each frame.
                if (currentDashTime > 0.1f)
                {
                    // collision check here
                    rb.AddForce(direction * (dashSpeed + dlBuff)); // Dash in the direction that was held down.
                                                        // No need to multiply by Time.DeltaTime here, physics are already consistent across different FPS.
                }
                else if (tr.emitting)
                {
                    tr.emitting = false;
                    rb.freezeRotation = false;
                    rb.velocity = new Vector2(0f, 0f); // Stop dashing.
                    canMove = true;
                }
                if (currentDashTime + (dashCooldown - dsBuff) <= 0f)
                {
                    break;
                }
                    yield return null; // Returns out of the coroutine this frame so we don't hit an infinite loop.
            }
        }
    }

    /// <summary>
    /// Moves the player in a certain direction while checking collision
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private bool moveTo(Vector2 direction)
    {
        // Collision checking
        int count = rb.Cast(
            direction,
            movementFilter,
            castCollisions,
            moveSpeed * Time.fixedDeltaTime + collisionHitBox);

        if (count == 0)
        {
            // No collision
            Vector2 moveVector = direction * moveSpeed * Time.fixedDeltaTime;

            rb.MovePosition(rb.position + moveVector);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Forces the player to look at the mouse
    /// </summary>
    private void lookAtMouse()
    {
        Vector2 mouseVector = playerInput.mouseInput;
        Vector2 mDirection = Camera.main.ScreenToWorldPoint(mouseVector);
        if (!rb.OverlapPoint(mDirection))
        {
            float angle = Mathf.Atan2(mDirection.y - tf.position.y, mDirection.x - tf.position.x);
            float mRotation = angle * Mathf.Rad2Deg - 90;
            rb.MoveRotation(mRotation + Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Updates movement
    /// </summary>
    private void handleMovement()
    {
        if (canMove)
        {
            Vector2 movementVector = playerInput.movementInput;

            // Attempt to move player in input direction
            bool success = moveTo(movementVector);

            if (!success)
            {
                // Try to move player in other directions
                success = moveTo(new Vector2(movementVector.x, 0));

                if (!success)
                {
                    success = moveTo(new Vector2(0, movementVector.y));
                }
            }
        }
    }

    // FixedUpdate is called once per frame, calls methods and updates timers
    void FixedUpdate()
    {
        // Move the player
        handleMovement();
        // Set the players rotation
        lookAtMouse();
        // Handle dashing
        if (playerInput.jumpInput)
        {
            StartCoroutine(Dash(tf.up));
        }
        if (Input.GetMouseButtonDown(0) && canAtk)
        {
            StartCoroutine(attack());
        }
        loadTimeCur -= Time.deltaTime;
    }

    /// <summary>
    /// When the players collider enters another one, adds it to a list
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerStay2D(Collider2D col)
    {
        curColliders.Add(col);
        checkedColliders = true;
    }

    /// <summary>
    /// Goes through all active colliders player is in and adds inputLabels to the closest one if applicable
    /// </summary>
    private void LateUpdate()
    {
        if (checkedColliders)
        {
            checkedColliders = false;

            float bestDistance = 99999.0f;
            Collider2D closestCollider = null;
            foreach (Collider2D col in curColliders)
            {
                if (col && (col.gameObject.CompareTag("item") || col.gameObject.CompareTag("portal")))
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);

                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        closestCollider = col;
                    }
                }
            }

            GameObject pressInput = GameObject.Find("pressInput");

            if (closestCollider != null)
            {
                if (pressInput)
                {
                    pressInput.transform.position = new Vector3(closestCollider.transform.position.x, closestCollider.transform.position.y - 2, 0);
                }
                else
                {
                    GameObject newPI = Instantiate(pressInp, new Vector3(closestCollider.transform.position.x, closestCollider.transform.position.y - 2, 0), Quaternion.identity);
                    newPI.name = pressInp.name;
                }

                if (Input.GetKeyDown("f") && closestCollider.gameObject.CompareTag("item"))
                {
                    GameObject item = closestCollider.gameObject.GetComponent<shopStats>().buy(money);
                    if (item != null)
                    {
                        money -= item.GetComponent<shopStats>().getCost();
                        Instantiate(buyEffect, transform.position, Quaternion.identity);
                        stats.GetComponent<statsUi>().updateGold(money);
                        changeHealth(item.GetComponent<shopStats>().getHealing());

                        Dictionary<string, Dictionary<string, string>> itemSkill = item.GetComponent<shopStats>().getSkills();

                        List<string> itemSkillNames = item.GetComponent<shopStats>().getSkillNames();

                        for (int i = 0; i < itemSkillNames.Count; i++)
                        {
                            Dictionary<string, string> itemSkillInfo = itemSkill[itemSkillNames[i]];
                            addPlayerSkill(itemSkillNames[i], itemSkillInfo, true);
                        }

                        Destroy(item);
                    }

                }
                else if (Input.GetKeyDown("f") && closestCollider.gameObject.CompareTag("portal") && loadTimeCur <= 0)
                {

                    loadTimeCur = loadTimeMax;
                    GameObject.Find("mapControl").gameObject.GetComponent<mapControl>().resetLevel();
                }
            }
            else
            {
                if (pressInput)
                {
                    Destroy(pressInput);
                }
            }

            curColliders.Clear();
        }
    }

    /// <summary>
    /// Attacks with the players sword and handles damage calculation
    /// </summary>
    /// <returns></returns>
    IEnumerator attack()
    {
        bool atking = true;
        updateDamage();
        sword.GetComponent<plrSword>().setDmg(dmg);
        sword.GetComponent<plrSword>().setAtking(atking);
        Instantiate(swingEffect, transform.position, Quaternion.identity);
        canAtk = false;
        int dpsBuff = 1;
        List<string> buffs = checkForSkillType("buffDPS");
        foreach (var v in buffs)
        {
            dpsBuff *= Convert.ToInt32(v);
        }

        while (sword.transform.localEulerAngles.z >= 260 || sword.transform.localEulerAngles.z <= 90)
        {
            sword.transform.RotateAround(transform.position, Vector3.forward, swingSpeed * Time.fixedDeltaTime * dpsBuff);
            yield return null;
        }

        atking = false;
        sword.GetComponent<plrSword>().setAtking(atking);

        sword.transform.RotateAround(transform.position, Vector3.forward, -185);

        canAtk = true;
        yield break;
    }

    /// <summary>
    /// Sets the players max health
    /// </summary>
    /// <param name="maxHealthChange"></param>
    /// <param name="pct"></param>
    public void changeMaxHealth(int maxHealthChange, bool pct)
    {
        if (!pct)
        {
            healthMax += maxHealthChange;
            health += maxHealthChange;
        }
        else
        {
            float pctInc = maxHealthChange * 0.01f;
            health += (int)Mathf.Round(maxHealthChange * pctInc);
            maxHealthChange += (int)Mathf.Round(maxHealthChange * pctInc);
        }
    }

    /// <summary>
    /// Changes the players health with a indicator and restarts the game if they die
    /// </summary>
    /// <param name="healthChange"></param>
    public void changeHealth(int healthChange)
    {

        GameObject popUp = Instantiate(damageIndicator, transform.position, Quaternion.identity);
        popUp.transform.GetComponentInChildren<TextMeshProUGUI>().text = healthChange.ToString();

        if (healthChange > 0)
        {
            popUp.transform.GetComponentInChildren<TextMeshProUGUI>().faceColor = Color.green;
        }
        else
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        health += healthChange;

        if(health > healthMax)
        {
            health = healthMax;
        }
        else if(health <= 0)
        {
            GameObject MC = GameObject.Find("mapControl");
            SceneManager.LoadScene(sceneName: "Scene1");
            Destroy(transform.parent.gameObject);
            Destroy(MC);
        }
    }

    /// <summary>
    /// Levels the player up a certain amount
    /// </summary>
    /// <param name="amt"></param>
    public void levelUp(int amt)
    {
        level += amt;
        skillPoints += amt;
        stats.GetComponent<statsUi>().updateLevel(level);
        xpNextLevel += (level + 1) * 10;
        stats.GetComponent<statsUi>().updateXp((level + 1) * 10, (level + 1) * 10 - xpNextLevel);
    }

    /// <summary>
    /// Adds xp to the player and levels them up if reached the goal
    /// </summary>
    /// <param name="x"></param>
    public void addXp(int x)
    {
        xpTotal += x;

        xpNextLevel -= x;
        if (xpNextLevel <= 0)
        {
            skillPoints++;
            level++;
            stats.GetComponent<statsUi>().updateLevel(level);
            xpNextLevel += (level + 1) * 10;
        }
        stats.GetComponent<statsUi>().updateXp((level + 1) * 10, (level + 1) * 10 - xpNextLevel);
    }

    /// <summary>
    /// Gets skill points
    /// </summary>
    /// <returns></returns>
    public int getSkillPoints()
    {
        return skillPoints;
    }

    /// <summary>
    /// Sets skill points
    /// </summary>
    /// <param name="s"></param>
    public void setSkillPoints(int s)
    {
        skillPoints = s;
    }

    /// <summary>
    /// Updates player money
    /// </summary>
    /// <param name="m"></param>
    public void addMoney(float m)
    {
        money += m;
        stats.GetComponent<statsUi>().updateGold(money);
    }

    /// <summary>
    /// Adds temporary damage buffs to the player
    /// </summary>
    /// <param name="inc"></param>
    /// <param name="pct"></param>
    public void addDmg(int inc, bool pct)
    {
        if (!pct)
        {
            incDmg += inc;
            dmg += inc;
        }
        else
        {
            float pctInc = inc * 0.01f;
            incDmg += (int)Mathf.Round(dmg * pctInc);
            dmg += (int)Mathf.Round(dmg * pctInc);
        }
    }

    /// <summary>
    /// Sets the players damage to a certain amount based on buffs
    /// </summary>
    private void updateDamage()
    {
        int tempDmg = baseDmg;
        List<string> buffs = checkForSkillType("buffDamage");
        foreach (var v in buffs)
        {
            tempDmg += Convert.ToInt32(v);
        }
        dmg = tempDmg;
    }

    /// <summary>
    /// Sets the players speed based on buffs
    /// </summary>
    private void updateSpeed()
    {
        float tempSpd = baseMoveSpeed;
        List<string> buffs = checkForSkillType("buffSpeed");
        foreach (var v in buffs)
        {
            tempSpd += float.Parse(v);
        }
        moveSpeed = tempSpd;
    }

    /// <summary>
    /// Sets the players max health based on buffs
    /// </summary>
    private void updateHealthMax()
    {
        int tempMhp = baseHealthMax;
        List<string> buffs = checkForSkillType("buffHealth");
        foreach (var v in buffs)
        {
            tempMhp += int.Parse(v);
        }
        health += tempMhp - healthMax;
        healthMax = tempMhp;
    }

    /// <summary>
    /// Adds a player skill to their list so that they can use it
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="details"></param>
    /// <param name="dupes"></param>
    public void addPlayerSkill(string skill, Dictionary<string,string> details, bool dupes)
    {

        if (!playerSkills.ContainsKey(skill))
        {
            playerSkills.Add(skill, details);
        }
        else if (dupes)
        {
            bool added = false;
            int i = 0;
            while (!added)
            {
                if(!playerSkills.ContainsKey(skill + i))
                {
                    playerSkills.Add(skill + i, details);
                    added = true;
                }
                i++;
            }
            
        }

        if (details["Effect"] == "buffSpeed")
        {
            updateSpeed();
        }
        else if (details["Effect"] == "buffHealth")
        {
            updateHealthMax();
        }
    }

    /// <summary>
    /// Returns the player skills
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, Dictionary<string, string>> getPlayerSkills()
    {
        return playerSkills;
    }

    /// <summary>
    /// Checks if the player has a certain buff
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public List<string> checkForSkillType(string type)
    {
        List<string> list = new List<string>();
        if (playerSkills.Count > 0)
        {
            foreach (Dictionary<string, string> entry in new List<Dictionary<string, string>>(playerSkills.Values))
            {
                string locked = entry["Locked"];
                string cost = entry["Cost"];
                string effect = entry["Effect"];
                string effectValue = entry["effectValue"];
                if (effect == type)
                {
                    list.Add(effectValue);
                }
            }
        }
        return list;
    }

    /// <summary>
    /// Updates the depth ui indicator
    /// </summary>
    /// <param name="depth"></param>
    public void depthChange(int depth)
    {
        stats.GetComponent<statsUi>().updateDepth(depth);
    }
}
