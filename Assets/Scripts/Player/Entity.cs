using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity
{
    public float maxHealth;
    public float currentHealth;
    public bool canMove = true;
}

public class Player : Entity
{
    public Player(float health)
    {
        this.maxHealth = health;
        this.currentHealth = health;
    }

    public Player(Player player)
    {
        this.maxHealth = player.maxHealth;
        this.currentHealth = player.currentHealth;
    }
}

public class Enemy : Entity
{
    public Enemy(float health)
    {
        this.maxHealth = health;
    }

    public Enemy(Enemy enemy)
    {
        this.maxHealth = enemy.maxHealth;
    }
}
