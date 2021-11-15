using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    public GameObject[] waypoints;
    [SerializeField] bool rotateTowardsTarget = false;
    int currentWaypointIndex = 0;

    [SerializeField] float rotationSpeed = 3f;
    [SerializeField] float speed = 1f;

	[SerializeField] float acceptableDistance = .1f;

    private void Start()
    {
		acceptableDistance = Mathf.Max(.1f, acceptableDistance);
    }

    void Update()
    {
		if(waypoints.Length > 0)
		{
			if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].transform.position) < acceptableDistance)
			{
				currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
			} else
            {
				transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, speed * Time.deltaTime);
			}

			if (rotateTowardsTarget)
			{
				Vector3 lookAtRotation = Quaternion.LookRotation(waypoints[currentWaypointIndex].transform.position - transform.position).eulerAngles;
				Quaternion rot = Quaternion.Euler(Vector3.Scale(lookAtRotation, new Vector3(0, 1, 0)));
				transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
			}
		}
    }
}
