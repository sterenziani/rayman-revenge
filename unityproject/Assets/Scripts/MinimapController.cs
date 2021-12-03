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
	public float distance = 50;

	private Camera longDistanceCamera;
	private Camera mediumDistanceCamera;
	private Camera shortDistanceCamera;
	private Camera iconCamera;

	void Start()
	{
		longDistanceCamera = cameraLongDistance.GetComponent<Camera>();
		mediumDistanceCamera = cameraMediumDistance.GetComponent<Camera>();
		shortDistanceCamera = cameraShortDistance.GetComponent<Camera>();
		iconCamera = cameraIcons.GetComponent<Camera>();

		longDistanceCamera.orthographicSize = distance;
		mediumDistanceCamera.orthographicSize = distance;
		shortDistanceCamera.orthographicSize = distance;
		iconCamera.orthographicSize = distance;
	}

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
				longDistanceCamera.orthographicSize = size;
				mediumDistanceCamera.orthographicSize = size;
				shortDistanceCamera.orthographicSize = size;
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
