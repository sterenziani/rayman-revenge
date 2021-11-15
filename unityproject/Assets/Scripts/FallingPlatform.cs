using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : Platform
{
	public bool weighted = false;
	public float lowestHeight = 0;
	public float explodeTime = 0;
	public float fallSpeed = 0;
	public float respawnWait = 0;
	public float blinkFrequency = 0;
	public bool ignorePlayerWeight = false;

	private float startingHeight = 0;
	private float timer = 0;
	private float blinkTimer = 0;
	private bool activated = false;
	private bool exploded = false;


	protected override void Start()
	{
		base.Start();

		startingHeight = transform.position.y;
	}

	private void OnTriggerStay(Collider collision)
	{
		activated = false;
		GameObject target = collision.gameObject;
		if (((!ignorePlayerWeight && target.GetComponent<Player>() != null) || target.tag == "Crate") && !exploded)
			activated = true;
	}

	protected override void OnTriggerExit(Collider collision)
	{
		base.OnTriggerExit(collision);

		GameObject target = collision.gameObject;
		if (weighted && ((!ignorePlayerWeight && target.GetComponent<Player>() != null) || target.tag == "Crate"))
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
					GetComponent<Collider>().enabled = false;
					foreach (Transform child in transform)
					{
						StickyPlatform sp = child.gameObject.GetComponent<StickyPlatform>();
						if(sp != null)
						{
							sp.ResetCollisions();
							sp.enabled = false;
						}
						child.gameObject.SetActive(false);
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
					GetComponent<Collider>().enabled = true;
					foreach (Transform child in transform)
					{
						StickyPlatform sp = child.gameObject.GetComponent<StickyPlatform>();
						if (sp != null)
                        {
							sp.ResetCollisions();
							sp.enabled = true;
						}
						child.gameObject.SetActive(true);
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