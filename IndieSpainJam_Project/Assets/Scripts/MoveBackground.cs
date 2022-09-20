using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour {
	[SerializeField] private float parallaxMultiplier;
	private Transform mainCameraTransform;
	private Vector3 previousCameraPosition;
	private float spriteWidth, horizontalMovement, startPosition;

	void Start()
	{
		mainCameraTransform = Camera.main.transform;
		previousCameraPosition = mainCameraTransform.position;
		spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
		horizontalMovement = spriteWidth * 2;
		startPosition = transform.position.x;
	}
	void LateUpdate()
	{
		float deltax = (mainCameraTransform.position.x - previousCameraPosition.x) * parallaxMultiplier;
		float moveAmount = mainCameraTransform.position.x * (1 - parallaxMultiplier);
		transform.Translate(new Vector3(deltax, 0, 0));
		previousCameraPosition = mainCameraTransform.position;

		if (moveAmount > startPosition + spriteWidth) {
			transform.Translate(new Vector3(horizontalMovement, 0, 0));
			startPosition += horizontalMovement;
		}
		else if (moveAmount < startPosition - spriteWidth) {
			transform.Translate(new Vector3(-horizontalMovement, 0, 0));
			startPosition -= horizontalMovement;
		}

	}
}

