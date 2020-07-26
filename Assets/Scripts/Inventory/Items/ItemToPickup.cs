using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemToPickup : MonoBehaviour
{
    public CharacterInventory cI;
    public int itemId = 0;
    public int count = 0;
    
    private void Start()
    {
        cI = GameObject.Find("Player").GetComponent<CharacterInventory>();
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
