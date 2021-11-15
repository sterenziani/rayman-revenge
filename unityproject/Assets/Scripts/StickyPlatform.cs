using System.Collections.Generic;
using UnityEngine;

public class StickyPlatform : Platform
{
    private Dictionary<GameObject, Vector3> targetsAndOffsets = new Dictionary<GameObject, Vector3>();

    private void OnCollisionStay(Collision collision)
    {
        GameObject target = collision.gameObject;
        Vector3 offset = target.transform.position - transform.position;
        targetsAndOffsets[target] = offset;
    }

    protected override void OnCollisionExit(Collision collision)
    {
        base.OnCollisionExit(collision);

		GameObject target = collision.gameObject;
        targetsAndOffsets.Remove(target);
    }

    public void ResetCollisions()
    {
        targetsAndOffsets = new Dictionary<GameObject, Vector3>();
    }

    void LateUpdate()
	{
		RotatingObject rotObj = transform.parent.GetComponent<RotatingObject>();
		foreach (GameObject target in targetsAndOffsets.Keys)
        {
            if(target != null)
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
}
