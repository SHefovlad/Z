using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class CameraHolder : MonoBehaviour
{
    public GameObject player;
    public Transform playerModelTransform;
    public GameObject target;
    public Transform cameraTransform;

    [SerializeField] float cameraDist = 3.8f;
    float maxCameraDist;

    public EnemyFinder enemyFinder;

    public bool targeting = false;

    public float differenceMaximum;
    private void Awake()
    {
        maxCameraDist = cameraDist;
    }
    void Update()
    {
        Vector3 newPos = new();

        if (!targeting)
        {
            newPos = player.transform.position;
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

        Debug.DrawLine(player.transform.position, cameraTransform.position, Color.red);
        if (Physics.Raycast(player.transform.position, new Vector3(0, 1, -1), out RaycastHit hit, maxCameraDist)
            && hit.transform.gameObject.layer == 10)
        {
            cameraDist = hit.distance;
        }
        else
        {
            cameraDist = maxCameraDist;
        }

        float catet = cameraDist / Mathf.Sqrt(2) - 0.3f;

        if (catet < 0.75f)
        {
            SetChildrenRenderersActivity(playerModelTransform, false);
        }
        else
        {
            SetChildrenRenderersActivity(playerModelTransform, true);
        }

        Vector3 delta = new Vector3(0, -(maxCameraDist - catet), (maxCameraDist - catet));
        newPos += delta;

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 10);
    }

    List<Transform> GetAllChildren(Transform transform)
    {
        var children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
            children.AddRange(GetAllChildren(child));
        }
        return children;
    }
    void SetChildrenRenderersActivity(Transform parent, bool activity)
    {
        List<Transform> children = GetAllChildren(parent);
        foreach (Transform child in children)
        {
            if (child.GetComponent<Renderer>() != null)
            {
                child.GetComponent<Renderer>().enabled = activity;
            }
        }
    }
}
