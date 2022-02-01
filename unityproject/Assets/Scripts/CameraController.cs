using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
	// Start is called before the first frame update
	public Transform target;
	public Player playerMovement;
	public float distance = 5.0f;

	[Range(0f, 1f)]
	public float mouseSensitivity = 1;

	public static float xSpeed = 100.0f;
	public static float ySpeed = 100.0f;
	[SerializeField] Slider xSensitivitySlider;
	[SerializeField] Slider ySensitivitySlider;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	public float distanceMin = .5f;
	public float distanceMax = 15f;

	private Rigidbody rigidBody;

	float x = 0.0f;
	float y = 0.0f;

	private float mouseX = 0;
	private float mouseY = 0;

	private float zoomModifier = 0f;

	void Start()
	{
		if (!PlayerPrefs.HasKey("xSensitivity"))
			PlayerPrefs.SetFloat("xSensitivity", 100);
		if (!PlayerPrefs.HasKey("ySensitivity"))
			PlayerPrefs.SetFloat("ySensitivity", 100);
		LoadSensibilityX();
		LoadSensibilityY();

		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;

		rigidBody = GetComponent<Rigidbody>();

		if (rigidBody != null)
		{
			rigidBody.freezeRotation = true;
		}
	}

	void LateUpdate()
	{
		if (target && !PauseMenu.gameIsPaused && !playerMovement.ControlledByCinematic)
		{
			x += mouseX * xSpeed * mouseSensitivity * distance * 0.001f;
			y -= mouseY * ySpeed * mouseSensitivity * 0.002f;

			y = ClampAngle(y, yMinLimit, yMaxLimit);

			Quaternion rotation = Quaternion.Euler(y, x, 0);

			//distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5 * mouseSensitivity, distanceMin, distanceMax);
			distance = Mathf.Clamp(distance - (zoomModifier * 0.2f * mouseSensitivity), distanceMin, distanceMax);
			zoomModifier = 0f;

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

	public void ChangeSensitivityX()
	{
		xSpeed = xSensitivitySlider.value;
		SaveSensibilityX();
	}

	public void ChangeSensitivityY()
	{
		ySpeed = ySensitivitySlider.value;
		SaveSensibilityY();
	}

	private void LoadSensibilityX()
	{
		xSensitivitySlider.value = PlayerPrefs.GetFloat("xSensitivity");
	}
	private void LoadSensibilityY()
	{
		ySensitivitySlider.value = PlayerPrefs.GetFloat("ySensitivity");
	}

	private void SaveSensibilityX()
	{
		PlayerPrefs.SetFloat("xSensitivity", xSensitivitySlider.value);
	}

	private void SaveSensibilityY()
	{
		PlayerPrefs.SetFloat("ySensitivity", ySensitivitySlider.value);
	}

	public void OnCameraMovement(InputValue value)
    {
		Vector2 direction = value.Get<Vector2>();
		mouseX = direction.x;
		mouseY = direction.y;
	}

	public void OnCameraZoomIn()
    {
		zoomModifier = -1;
	}

	public void OnCameraZoomOut()
	{
		zoomModifier = 1;
	}

	void OnCameraZoomInMouseScroll()
	{
		float value = Input.GetAxis("Mouse ScrollWheel");
		if (value < 0)
		{
			OnCameraZoomIn();
		}
		else if (value > 0)
		{
			OnCameraZoomOut();
		}
	}

	void OnCameraLookFront()
    {
		this.transform.forward = playerMovement.transform.forward;
    }
}
