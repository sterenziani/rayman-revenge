using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotRotation : MonoBehaviour
{
	public GameObject pivot;
	public float speed = 20f;
	public float xComponent;
	public float yComponent;
	public float zComponent;
	
    void Update()
    {
		if(pivot != null)
			transform.RotateAround(pivot.transform.position, new Vector3(xComponent, yComponent, zComponent), speed * Time.deltaTime);
    }
}
