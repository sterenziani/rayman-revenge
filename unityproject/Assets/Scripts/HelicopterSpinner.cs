using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterSpinner : MonoBehaviour
{
	public Texture[] textures;
	public double speed = 1;
	private double secsPassed = 0;
	private int frame;
	private Material helicopterMaterial;

	void Start()
	{
		helicopterMaterial = GetComponent<Renderer>().materials[1];
	}
	
    void Update()
    {
		secsPassed += Time.deltaTime;
		if (secsPassed > 1.0/(24*speed))
		{
			secsPassed = 0;
			frame++;
			if (frame % textures.Length == 0)
				frame = 0;
		}
		helicopterMaterial.SetTexture("_MainTex", textures[frame]);
    }
}
