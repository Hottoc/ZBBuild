using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

public class PlayerManager : MonoBehaviour
{
    #region Player Stats
    private int _maxHealth = 100;

    private int _currentHealth = 100;

    private int _currentLevel;

    private int _currentXP;

    public int CurrentHealth
    {
        get { return _currentHealth; }
    }

    public void TakeDamage (int damage)
    {
        _currentHealth -= damage;
    }

    public void AddHealth (int health)
    {
        _currentHealth += health;
        if (_currentHealth >= _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
    }

    public void AddXp(int xp)
    {
        _currentXP += xp;
    }
    #endregion

    #region Player Inventory

    private int _inventorySize = 10;

    private List<Item> _playerItems = new List<Item>();

    public ItemDatabase itemDB;

    public void SetInventorySize(int size)
    {
        _inventorySize = size;
    }
    public void AddItem(int id)
    {
        //Fetch Item Object From Item Database
        Item itemToAdd = itemDB.GetItem(id);
        //Add Item To Player Inventory List
        _playerItems.Add(itemToAdd);
        Debug.Log(itemToAdd.title + " Has Been Added To Inventory");
    }
    public void AddItem(string itemName)
    {
        //Fetch Item Object From Item Database
        Item itemToAdd = itemDB.GetItem(itemName);
        //Add Item To Player Inventory List
        _playerItems.Add(itemToAdd);
        Debug.Log(itemToAdd.title + " Has Been Added To Inventory");
    }
    public Item CheckForItem(int id)
    {
        return _playerItems.Find(itemDB => itemDB.id == id);
    }
    public void RemoveItem(int id)
    {
        //Fetch Item Object From Item Database By Checking For Item
        Item itemToRemove = CheckForItem(id);
        //Return If Item Is Null
        if (itemToRemove == null) return;
        //Remove Item From Player Inventory List
        _playerItems.Remove(itemToRemove);
        Debug.Log(_playerItems[id].title + " Has Been Removed From Inventory");
    }
    #endregion
}
