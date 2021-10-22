using System.Collections.Generic;
using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    private Dictionary<GameObject, Vector3> targetsAndOffsets = new Dictionary<GameObject, Vector3>();

    private void OnCollisionStay(Collision collision)
    {
        GameObject target = collision.gameObject;
        Vector3 offset = target.transform.position - transform.position;
        targetsAndOffsets[target] = offset;
    }

    private void OnCollisionExit(Collision collision)
    {
		Debug.Log("Bye");
		GameObject target = collision.gameObject;
        targetsAndOffsets.Remove(target);
    }

    void LateUpdate()
	{
		RotatingObject rotObj = transform.parent.GetComponent<RotatingObject>();
		foreach (GameObject target in targetsAndOffsets.Keys)
        {
			if (rotObj != null && rotObj.degreesPerSecondY != 0)
			{
				target.transform.RotateAround(transform.position, new Vector3(0, 1, 0), rotObj.degreesPerSecondY * Time.deltaTime);
			}
			else
			{
				target.transform.position = transform.position + targetsAndOffsets[target];
			}
        }
    }
}
