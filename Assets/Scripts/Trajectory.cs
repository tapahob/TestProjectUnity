using UnityEngine;
using System.Collections;

public class Trajectory : MonoBehaviour 
{
	public Vector3 start;
	public Vector3 end;
	public LineRenderer LineRenderer;
	public int SegmentsCount = 20;

	void Start() 
	{
		LineRenderer.SetVertexCount(SegmentsCount);
		start = transform.position;
	}
	
	void LateUpdate() 
	{
		float apexPoint = (start-end).magnitude / 2;
		LineRenderer.SetPosition(0, start);
		for (int i = 1; i<SegmentsCount-1; ++i)
		{
			Vector3 newPos = Vector3.Lerp(start, end, 1.0f / SegmentsCount * i);
			newPos.y += Mathf.Sin(1.0f / SegmentsCount * i * Mathf.PI) * apexPoint;
			LineRenderer.SetPosition(i, newPos);
		}
		LineRenderer.SetPosition(SegmentsCount-1, end);
	}
}
