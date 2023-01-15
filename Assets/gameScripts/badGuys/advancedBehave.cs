using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Uses inheritance to create a new enemy type
/// </summary>
public class advancedBehave : basicBehave
{
    private bool critted;

    /// <summary>
    /// Overides the targetReached function to include a chance for critical hits
    /// </summary>
    public new void targetReached()
    {
        if (atkTime <= 0)
        {

            if (Random.Range(1,3) == 1 && !critted)
            {
                atkDmg *= 2;
                critted = true;
            }
            else if (critted)
            {
                atkDmg /= 2;
                critted = false;
            }

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
}
