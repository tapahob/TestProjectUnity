using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour 
{
	public Light LightSource;
	public Rigidbody RigidBody;
	public MainPlayerController MainPlayerController;

	public void Fly(Vector3 enemyPosition)
	{
		float gunAngle = MainPlayerController.Gun.transform.localEulerAngles.x;
		RigidBody.velocity = BallisticVelocity(enemyPosition, gunAngle);
	}

	void OnCollisionEnter(Collision collision)
	{
		Detonate();
	}

	Vector3 BallisticVelocity(Vector3 target, float angle)
	{
		var dir = target - transform.position;
		var h = dir.y;
		dir.y = 0;
		var dist = dir.magnitude;
		var a = angle * Mathf.Deg2Rad;
		dir.y = dist * Mathf.Tan(a);
		dist += h / Mathf.Tan(a);
		var vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2*a));
		return vel * dir.normalized;
	}

	void Detonate()
	{
		LightSource.enabled = false;
	}
}
