using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
	public bool weighted = false;
	public float lowestHeight = 0;
	public float explodeTime = 0;
	public float fallSpeed = 0;
	public float respawnWait = 0;
	public float blinkFrequency = 0;

	private float startingHeight = 0;
	private float timer = 0;
	private float blinkTimer = 0;
	private bool activated = false;
	private bool exploded = false;


	void Start()
	{
		startingHeight = transform.position.y;
	}

	private void OnTriggerStay(Collider collision)
	{
		activated = false;
		GameObject target = collision.gameObject;
		if (target.GetComponent<Player>() != null && !exploded)
			activated = true;
	}

	private void OnTriggerExit(Collider collision)
	{
		GameObject target = collision.gameObject;
		if (weighted && target.GetComponent<Player>() != null)
			activated = false;
	}

	void Update()
	{
		if(weighted)
		{
			if (activated)
			{
				if (transform.position.y >= startingHeight - lowestHeight)
					transform.position -= new Vector3(0, 1, 0) * fallSpeed * Time.deltaTime;
			}
			else
			{
				if (transform.position.y < startingHeight)
					transform.position += new Vector3(0, 1, 0) * fallSpeed * Time.deltaTime;
				else
					transform.position = new Vector3(transform.position.x, startingHeight, transform.position.z);
			}
		}
		else
		{
			if (activated)
			{
				if (transform.position.y >= startingHeight - lowestHeight)
					transform.position -= new Vector3(0, 1, 0) * fallSpeed * Time.deltaTime;
				else
				{
					timer += Time.deltaTime;
					blinkTimer += Time.deltaTime;
					if (blinkTimer > blinkFrequency)
                    {
						blinkTimer = 0;
						Blink();
					}
				}
				if (timer > explodeTime)
				{
					foreach (Transform child in transform)
					{
						child.gameObject.GetComponent<Collider>().enabled = false;
						Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
						if (rb != null)
							rb.detectCollisions = false;
						StickyPlatform sp = child.gameObject.GetComponent<StickyPlatform>();
						if(sp != null)
						{
							sp.ResetCollisions();
							sp.enabled = false;
						}
						child.gameObject.GetComponent<Renderer>().enabled = false;
						//child.gameObject.SetActive(false);
					}
					activated = false;
					exploded = true;
					timer = 0;
					blinkTimer = 0;
				}
			}
			if (exploded)
			{
				timer += Time.deltaTime;
				if(timer > respawnWait)
				{
					transform.position = new Vector3(transform.position.x, startingHeight, transform.position.z);
					foreach (Transform child in transform)
					{
						child.gameObject.GetComponent<Collider>().enabled = true;
						Rigidbody rb = child.GetComponent<Rigidbody>();
						if(rb != null)
							rb.detectCollisions = true;
						StickyPlatform sp = child.gameObject.GetComponent<StickyPlatform>();
						if (sp != null)
                        {
							sp.ResetCollisions();
							sp.enabled = true;
						}
							
						child.gameObject.GetComponent<Renderer>().enabled = true;
						//child.gameObject.SetActive(true);
					}
					activated = false;
					exploded = false;
					timer = 0;
					blinkTimer = 0;
				}
			}
		}
	}
	
	IEnumerator Blink()
	{
		foreach (Transform child in transform)
        {
			Renderer r = child.gameObject.GetComponent<Renderer>();
			r.enabled = !r.enabled;
		}
		return null;
	}
}