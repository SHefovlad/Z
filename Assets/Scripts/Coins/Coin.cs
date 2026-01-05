using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    Transform player;
    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>().transform;
        }
        if (player != null)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, player.position, 100 * Time.deltaTime / Vector3.Distance(transform.position, player.position)));
        }
        if (Vector3.Distance(transform.position, player.position) < 1)
        {
            CoinsManager.Instance.Coins++;
            Debug.Log("Coin Collected");
            Destroy(gameObject);
        }
    }
}
