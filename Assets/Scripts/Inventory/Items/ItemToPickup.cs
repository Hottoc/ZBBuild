using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
public class ItemToPickup : MonoBehaviour
{
    public PlayerManager cI;
    public int itemId = 0;
    public int count = 0;
    
    private void Start()
    {
        cI = GameObject.Find("Player").GetComponent<PlayerManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(gameObject);
            cI.AddItem(itemId);
        }
    }


}
