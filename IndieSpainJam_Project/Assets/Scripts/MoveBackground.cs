using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour
{
	[SerializeField] private float parallaxMultiplier;
	private Vector2 screenBounds;
	private float spriteWidth, doubleSpriteWidth, speed;
	void Start()
	{
		spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
		speed = 0.01f;
		doubleSpriteWidth = spriteWidth * 2;
		screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
	}
	void LateUpdate()
	{
		//Cantidad que se mueve, es decir velocidad
		float xSpeedMovement = speed * parallaxMultiplier * Time.deltaTime;
		transform.Translate(new Vector3(xSpeedMovement, 0, 0));

		//Si la posicion horizontal es mas grande que la pantalla mas el ancho del sprite
		//Mueve el objeto al inicio de la pantalla, es decir el ancho de la pantalla - el ancho del sprite por dos
		if (transform.position.x > screenBounds.x + spriteWidth)
			transform.position = new Vector3(screenBounds.x - doubleSpriteWidth, transform.position.y, transform.position.z);
	}
}