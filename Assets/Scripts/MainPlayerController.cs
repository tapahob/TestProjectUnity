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
	public GameObject BarrelPivot;
	public Transform RocketPrefab;
	public Crosshair Crosshair;

	private Vector3 movement;
	private float mouseRotationX = 0.0f;
	private float mouseRotationY = 0.0f;
	private Quaternion originalBodyRotation;
	private Quaternion originalCameraAngle;
	private RaycastHit hit;
	private bool allowShooting = false;

	void Awake()
	{
		if (MainCharacterController == null)
			MainCharacterController = GetComponent<CharacterController>();
		movement = Vector3.zero;
	}

	void Start()
	{
		originalBodyRotation = transform.localRotation;
		originalCameraAngle = Camera.main.transform.localRotation;
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
		mouseRotationX = ClampAngle(mouseRotationX, -360.0f, 360.0f);
		mouseRotationY = ClampAngle(mouseRotationY, -20.0f, 20.0f);
		Quaternion quatX = Quaternion.AngleAxis(mouseRotationX, Vector3.up);
		Quaternion quatY = Quaternion.AngleAxis(mouseRotationY, Vector3.left);
		transform.localRotation = originalBodyRotation * quatX;
		Camera.main.transform.localRotation = originalCameraAngle * quatY;

		if (Input.GetMouseButtonDown(1))
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
