using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
using UnityEditor;

public class PlayerManager : MonoBehaviour
{
    Player player = new Player(100);
    #region Player Stats
    GameObject mybone;
    private int _maxHealth = 100;

    private int _currentLevel;

    private int _currentXP;
    GameObject newequip;
    public void Start()
    {
        mybone = GameObject.Find("Player:RightArmBone_end");
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            EquipItem(GameObject.Find("HammerTest"));
            AddHealth(2.0f);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            UnequipItem(GameObject.Find("HammerTest"));
        }
    }
    public float CurrentHealth
    {
        get { return player.currentHealth; }
    }

    public void TakeDamage (float damage)
    {
        player.currentHealth -= damage;
    }

    public void AddHealth (float health)
    {
        player.currentHealth += health;
        if (player.currentHealth >= player.maxHealth)
        {
            player.currentHealth = player.maxHealth;
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

    public void EquipItem(GameObject item)
    {
        newequip = Instantiate(item, new Vector3(mybone.transform.position.x, mybone.transform.position.y, mybone.transform.position.z), mybone.transform.rotation);

        newequip.transform.parent = mybone.transform;
    }

    public void UnequipItem(GameObject item)
    {
        Destroy(newequip);
    }
    #endregion
}
