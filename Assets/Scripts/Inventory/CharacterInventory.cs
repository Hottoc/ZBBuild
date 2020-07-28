/********************************************************/
/* Script Runs on The Player Parent Object In The Scene */
/********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventory : MonoBehaviour
{
    public List<Item> playerItems = new List<Item>();
    /*IF GETTING "NOT SET TO INSTANCE" ERROR THEN THE DATABASE IS NOT SET IN THE PLAYER OBJECT INSPECTOR*/
    public ItemDatabase itemDB;
    public void AddItem(int id)
    {
        //Fetch Item Object From Item Database
        Item itemToAdd = itemDB.GetItem(id);
        //Add Item To Player Inventory List
        playerItems.Add(itemToAdd);
        Debug.Log(itemToAdd.title + " Has Been Added To Inventory");
    }

    public void AddItem(string itemName)
    {
        //Fetch Item Object From Item Database
        Item itemToAdd = itemDB.GetItem(itemName);
        //Add Item To Player Inventory List
        playerItems.Add(itemToAdd);
        Debug.Log(itemToAdd.title + " Has Been Added To Inventory");
    }

    public Item CheckForItem(int id)
    {
        return playerItems.Find(itemDB => itemDB.id == id);
    }

    public void RemoveItem(int id)
    {
        //Fetch Item Object From Item Database By Checking For Item
        Item itemToRemove = CheckForItem(id);
        //Return If Item Is Null
        if (itemToRemove == null) return;
        //Remove Item From Player Inventory List
        playerItems.Remove(itemToRemove);
        Debug.Log(itemToRemove.title + " Has Been Removed From Inventory");
    }
}

//Add Russian Roullette
