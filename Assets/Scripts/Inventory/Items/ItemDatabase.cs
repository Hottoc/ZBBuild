using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    private void Awake()
    {
        BuildDatabase();
    }

    //Get Item From DataBase By ID value
    public Item GetItem(int id)
    {
        return items.Find(item => item.id == id);
    }

    //Get Item From DataBase By TITLE value
    public Item GetItem(string itemName)
    {
        return items.Find(item => item.title == itemName);
    }

    void BuildDatabase()
    {
        /*TEMPLATE*/
        /** new Item(int [ID], string [TITLE], string [DESCRIPTION]) **/
        items = new List<Item>() {
                new Item(0, "Hammer", "This is the description of a hammer"),
                new Item(1, "Test", "This is a test"),
                new Item(2, "Whatever", "Jereomy is smelly")
            };
    }
}
