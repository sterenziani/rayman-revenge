using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeContainer : MonoBehaviour
{
	public LifeContainer next;
	[SerializeField] Image filledImage;
	
    public void SetLife(int lives)
	{
		filledImage.fillAmount = lives;
		lives--;
		if(next != null)
			next.SetLife(lives);
	}
}
