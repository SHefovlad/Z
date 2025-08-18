using UnityEngine;
using System.Collections;

public class PlayerDetector : MonoBehaviour
{
    public PiramidController piramid;
    public HammurController hammur;
    Collider col;
    bool mayRun = true;
    private void Start()
    {
        col = GetComponent<Collider>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (mayRun && piramid != null)
            {
                StopCoroutine(FinderUpdate());
                piramid.PlayerInFinder();
                mayRun = false;
                StartCoroutine(FinderUpdate());
            }
            if (hammur != null) {
                hammur.targetIsPlayer = true;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hammur != null)
            {
                hammur.targetIsPlayer = false;
                if (hammur.targetPoint != null) Destroy(hammur.targetPoint.gameObject);
            }
        }
    }
    IEnumerator FinderUpdate()
    {
        if (piramid != null)
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);

                mayRun = true;
                col.enabled = false;
                col.enabled = true;
            }
        }
    }
}
