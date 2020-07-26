using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
	Texture[,] inventory;
	public Texture slotTexture;
	int iconWidthHeight = 40;

	public void Awake()
	{
		inventory = new Texture[8,10];
	}

	public void OnGUI()
	{
		//Go through each row 
		for (int i = 0; i < 8; i++)
		{
			// and each column 
			for (int k = 0; k < 10; k++)
			{

				//if there is a texture in the i-th row and the k-th column, draw it 
				if (inventory[i,k] != null)
				{
					slotTexture = inventory[i,k];
				}

				GUI.Label(new Rect(k * 50, i * 50, 60, 60), slotTexture);
			}
		}
	}
}



