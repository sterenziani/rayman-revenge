using UnityEngine;

public class CameraController : MonoBehaviour
{
	// Start is called before the first frame update
	public Transform target;
	public Player playerMovement;
	public float distance = 5.0f;

	[Range(0f, 1f)]
	public float mouseSensitivity = 1;

	private float xSpeed = 120.0f;
	private float ySpeed = 120.0f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	public float distanceMin = .5f;
	public float distanceMax = 15f;

	private Rigidbody rigidBody;

	float x = 0.0f;
	float y = 0.0f;

	// Use this for initialization
	void Start()
	{
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;

		rigidBody = GetComponent<Rigidbody>();

		// Que el cuerpo rigido no rote
		if (rigidBody != null)
		{
			rigidBody.freezeRotation = true;
		}
	}

	void LateUpdate()
	{
		if (target)
		{
			x += Input.GetAxis("Mouse X") * xSpeed * mouseSensitivity * distance * 0.02f;
			y -= Input.GetAxis("Mouse Y") * ySpeed * mouseSensitivity * 0.02f;

			y = ClampAngle(y, yMinLimit, yMaxLimit);

			Quaternion rotation = Quaternion.Euler(y, x, 0);

			distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5 * mouseSensitivity, distanceMin, distanceMax);

			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + target.position;

			transform.rotation = rotation;
			transform.position = position;
			playerMovement.SetRotation(x);
			playerMovement.SetMouseLookingAt(position);
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
}
