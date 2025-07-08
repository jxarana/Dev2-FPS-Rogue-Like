using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openShop : MonoBehaviour
{
    public GameObject shopRogueLike;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shopRogueLike = GameObject.Find("Shop");
        shopRogueLike.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopRogueLike.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopRogueLike.SetActive(false);
        }
    }
}
