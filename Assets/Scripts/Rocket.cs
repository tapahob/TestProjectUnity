using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour 
{
	public float flightSpeed = 3.0f;
	public Light lightSource;

	private Vector3 startPos;
	private Vector3 enemyPos;
	private float timer = 0.0f;
	private float apexPoint;

	public void Fly(Vector3 enemyPosition)
	{
		enemyPos = enemyPosition;
		startPos = transform.position;
		apexPoint = (startPos-enemyPos).magnitude / 2;
	}

	void Update () 
	{
		timer += Time.deltaTime;

		if (timer > flightSpeed)
		{
			Detonate();
			return;
		}
		Vector3 newPosition = Vector3.Lerp(startPos, enemyPos, timer / flightSpeed);
		newPosition.y += Mathf.Sin(timer / flightSpeed * Mathf.PI) * apexPoint;
		transform.position = newPosition;
	}

	void Detonate()
	{
		lightSource.enabled = false;

		//Destroy(this.gameObject);
	}
}
