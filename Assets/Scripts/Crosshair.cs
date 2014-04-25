using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour 
{	
	public Texture2D CrosshairNormal;
	public Texture2D CrosshairRed;

	private Texture2D currentTexture;

	public void SetActive(bool isActive)
	{
		currentTexture = isActive ? CrosshairNormal : CrosshairRed;
	}

	void Awake()
	{
		currentTexture = CrosshairNormal;
	}

	void OnGUI()
	{
		Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
		Rect drawingRect = new Rect(screenCenter.x - currentTexture.width/2, screenCenter.y - currentTexture.height/2, currentTexture.width, currentTexture.height);
		GUI.DrawTexture(drawingRect, currentTexture);
	}
}
