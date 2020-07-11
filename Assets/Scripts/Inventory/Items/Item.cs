using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int id;
    public string title;
    public string description;
    
    public Item(int id, string title, string description)
    {
        this.id = id;
        this.title = title;
        this.description = description;
    }

    public Item(Item item)
    {
        this.id = item.id;
        this.title = item.title;
        this.description = item.description;
    }
}
