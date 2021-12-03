using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
	public GameObject cameraLongDistance;
	public GameObject cameraMediumDistance;
	public GameObject cameraShortDistance;
	public GameObject cameraIcons;
	public Transform target1;
	public Transform icon1;
	public Transform target2;
	public Transform icon2;
	private float distance = 50;

    void Update()
    {
        if(target1 != null)
		{
			if (target2 != null)
			{
				float x = (target1.position.x + target2.position.x) / 2;
				float y = distance + (target1.position.y + target2.position.y) / 2;
				float z = (target1.position.z + target2.position.z) / 2;
				float size = Mathf.Sqrt(Mathf.Pow(target1.position.x + target2.position.x, 2) + Mathf.Pow(target1.position.z + target2.position.z, 2));
				float scale = size / distance;

				transform.position = new Vector3(x, y, z);
				cameraLongDistance.GetComponent<Camera>().orthographicSize = size;
				cameraMediumDistance.GetComponent<Camera>().orthographicSize = size;
				cameraShortDistance.GetComponent<Camera>().orthographicSize = size;
				cameraIcons.GetComponent<Camera>().orthographicSize = size;
				if (icon1 != null)
					icon1.localScale = new Vector3(scale, scale, scale);
				if (icon2 != null)
					icon2.localScale = new Vector3(scale, scale, scale);
			}
			else
			{
				transform.position = new Vector3(target1.position.x, distance + target1.position.y, target1.position.z);
			}
		}
    }
}
