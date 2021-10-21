using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] GameObject[] waypoints;
    [SerializeField] bool rotateTowardsTarget = false;
    int currentWaypointIndex = 0;

    [SerializeField] float rotationSpeed = 3f;
    [SerializeField] float speed = 1f;

    void Update()
    {
		if(waypoints.Length > 0)
		{
			if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].transform.position) < .1f)
			{
				currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
			}

			transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, speed * Time.deltaTime);

			if (rotateTowardsTarget)
			{
				Vector3 lookAtRotation = Quaternion.LookRotation(waypoints[currentWaypointIndex].transform.position - transform.position).eulerAngles;
				Quaternion rot = Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0, 1, 0)));
				transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
			}
		}
    }
}
