using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour
{
	[SerializeField] private float parallaxMultiplier;
	private Vector2 screenBounds;
	private Vector3 previousCameraPosition;
	private float spriteWidth, horizontalMovement, startPosition;
	private float actualPos;
	void Start()
	{
		spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
		horizontalMovement = spriteWidth * 2;
		startPosition = transform.position.x;
		screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
	}
	void LateUpdate()
	{
		float deltax = 0.01f * parallaxMultiplier;
		actualPos += deltax;
		transform.Translate(new Vector3(deltax, 0, 0));

		if (transform.position.x > 19)
		{
			transform.position = new Vector3(-19, transform.position.y, transform.position.z);
			actualPos = -19;
		}
	}
}