using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
	public Transform target1;
	public Transform target2;
	
    void Update()
    {
        if(target1 != null)
		{
			if (target2 != null)
			{
				float x = (target1.position.x + target2.position.x) / 2;
				float y = 20 + (target1.position.y + target2.position.y) / 2;
				float z = (target1.position.z + target2.position.z) / 2;
				transform.position = new Vector3(x, transform.position.y, z);
			}
			else
			{
				transform.position = new Vector3(target1.position.x, 20 + target1.position.y, target1.position.z);
			}
		}
    }
}
