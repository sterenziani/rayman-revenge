using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
	[SerializeField] GameObject lifeContainerPrefab;
	[SerializeField] List<GameObject> lifeContainers;
	int totalLives;
	int currentLives;
	LifeContainer currentLife;

	// Start is called before the first frame update
	void Start()
    {
		lifeContainers = new List<GameObject>();
    }

	public void SetupLives(int lives)
	{
		lifeContainers.Clear();
		for(int i=transform.childCount-1; i >= 0; i--)
		{
			Destroy(transform.GetChild(i).gameObject);
		}
		if (lives <= 0)
			return;
		totalLives = lives;
		currentLives = lives;
		for(int i=0; i < lives; i++)
		{
			GameObject newLife = Instantiate(lifeContainerPrefab, transform);
			lifeContainers.Add(newLife);
			if (currentLife != null)
				currentLife.next = newLife.GetComponent<LifeContainer>();
			currentLife = newLife.GetComponent<LifeContainer>();
		}
		currentLife = lifeContainers[0].GetComponent<LifeContainer>();
	}

	public void SetCurrentLives(int lives)
	{
		currentLives = lives;
		currentLife.SetLife(lives);
	}

	public void AddLives(int lives)
	{
		currentLives += lives;
		if (currentLives > totalLives)
			currentLives = totalLives;
		currentLife.SetLife(currentLives);
	}

	public void RemoveLives(int lives)
	{
		currentLives -= lives;
		if (currentLives < 0)
			currentLives = 0;
		currentLife.SetLife(currentLives);
	}

	public void AddContainer()
	{
		GameObject newLife = Instantiate(lifeContainerPrefab, transform);
		currentLife = lifeContainers[lifeContainers.Count - 1].GetComponent<LifeContainer>();
		lifeContainers.Add(newLife);
		if (currentLife != null)
			currentLife.next = newLife.GetComponent<LifeContainer>();
		currentLife = lifeContainers[0].GetComponent<LifeContainer>();
		totalLives++;
		currentLives = totalLives;
		SetCurrentLives(currentLives);
	}
}
