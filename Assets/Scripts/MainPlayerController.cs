using UnityEngine;
using System.Collections;

public class MainPlayerController : MonoBehaviour 
{
	public CharacterController MainCharacterController;
	public float MovementSpeed = 10.0f;
	public float MouseSensitivity = 15.0f;
	public Trajectory Trajectory;
	public GameObject Gun;
	public GameObject Barrel;
	public Transform RocketPrefab;
	public Crosshair Crosshair;
	public float BodyRotationSpeed = 80f;

	private Vector3 movement;
	private float mouseRotationX = 0.0f;
	private float mouseRotationY = 0.0f;
	private Quaternion originalGunRotation;
	private Quaternion originalCameraAngle;
	private RaycastHit hit;
	private bool allowShooting = false;
	private float prevRotationX = 0;
	private const float MAX_BARREL_ANGLE_X = 20f;
	private const float MIN_BARREL_ANGLE_X = -20f;
	private const float MAX_BARREL_ANGLE_Y = 80f;
	private const float MIN_BARREL_ANGLE_Y = 0f;

	void Awake()
	{
		if (MainCharacterController == null)
			MainCharacterController = GetComponent<CharacterController>();
		movement = Vector3.zero;
	}

	void Start()
	{
		originalGunRotation = Gun.transform.localRotation;
		originalCameraAngle = Camera.main.transform.localRotation;
		Screen.lockCursor = true;
	}

	void Update() 
	{
		updateInput();
		updateTrajectory();
	}

	void updateInput()
	{
		// WASD Movement
		float side = Input.GetAxis("Horizontal");
		float forward = Input.GetAxis("Vertical");
		
		if (side == 0 && forward == 0)
			movement = Vector3.zero;
		
		Vector3 forwardVector = transform.TransformDirection(Vector3.forward);
		Vector3 rightVector = transform.TransformDirection(Vector3.right);
		movement = (side * rightVector + forward * forwardVector) * Time.deltaTime * MovementSpeed;
		if (MainCharacterController == null)
		{
			Debug.LogError("Main character controller was not set!");
			return;
		}
		MainCharacterController.Move(movement);
		
		// Mouse Rotation
		mouseRotationX += Input.GetAxis("Mouse X") * MouseSensitivity;
		mouseRotationY += Input.GetAxis("Mouse Y") * MouseSensitivity;
		mouseRotationX = ClampAngle(mouseRotationX, MIN_BARREL_ANGLE_X, MAX_BARREL_ANGLE_X);
		mouseRotationY = ClampAngle(mouseRotationY, MIN_BARREL_ANGLE_X, MAX_BARREL_ANGLE_X);
		if (prevRotationX == MIN_BARREL_ANGLE_X || prevRotationX == MAX_BARREL_ANGLE_X && prevRotationX == mouseRotationX)
		{
			var speed = prevRotationX == MIN_BARREL_ANGLE_X ? -1 * BodyRotationSpeed : BodyRotationSpeed;
			transform.RotateAround(transform.position, Vector3.up, speed * Time.deltaTime);
		}
		prevRotationX = mouseRotationX;
		Quaternion quatX = Quaternion.AngleAxis(mouseRotationX, Vector3.back);
		Quaternion quatY = Quaternion.AngleAxis(mouseRotationY, Vector3.left);
		Gun.transform.localRotation = originalGunRotation * quatX * quatY;
		Camera.main.transform.localRotation = originalCameraAngle * quatY;
		if (Input.GetMouseButtonDown(0))
			Shoot();
	}

	void updateTrajectory()
	{
		if (Trajectory == null)
			return;

		Trajectory.start = Trajectory.transform.position;
		Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward));
		if (Physics.Raycast(ray, out hit) && hit.collider.name == "Terrain")
		{
			allowShooting = true;
			Trajectory.end = hit.point;
		}
		else
			allowShooting = false;
		Crosshair.SetActive(allowShooting);
	}

	void Shoot()
	{
		if (!allowShooting)
			return;
		Rocket rocket = (GameObject.Instantiate(RocketPrefab, Barrel.transform.position, Quaternion.identity) as Transform).GetComponent<Rocket>();
		rocket.MainPlayerController = this;
		rocket.Fly(hit.point);
	}

	float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
			angle += 360f;
		if (angle > 360f)
			angle -= 360f;
		return Mathf.Clamp (angle, min, max);
	}
}
