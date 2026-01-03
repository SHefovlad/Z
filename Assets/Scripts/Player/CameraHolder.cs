using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    public GameObject player;
    public GameObject target;

    public EnemyFinder enemyFinder;

    public bool targeting = false;

    public float differenceMaximum;
    void Update()
    {
        if (!targeting)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime * 10);
        }
        else
        {
            if (target != null)
            {
                if (target.GetComponent<Enemy>() != null)
                {
                    if (target.GetComponent<Enemy>().healthManager?.HEALTH <= 0)
                    {
                        targeting = false;
                        target = null;
                        enemyFinder.target = null;
                        return;
                    }
                }
                if (Mathf.Sqrt((Mathf.Pow(Mathf.Abs(player.transform.position.x - target.transform.position.x), 2) + Mathf.Pow(Mathf.Abs(player.transform.position.z - target.transform.position.z), 2))) > differenceMaximum)
                {
                    targeting = false;
                    target = null;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3((player.transform.position.x + target.transform.position.x) / 2, player.transform.position.y, (player.transform.position.z + target.transform.position.z) / 2), Time.deltaTime * 10);
                }
            }
            else
            {
                targeting = false;
                target = null;
            }
        }
    }
}
