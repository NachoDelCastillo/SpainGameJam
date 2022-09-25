using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour
{
	[SerializeField] private float parallaxMultiplier;
	private float speed, engineForce;
	private float[] spriteWidths;
	private Transform[] childrenTransforms;
	private Vector2 screenBounds;
	void Start()
	{
		engineForce = 0.1f;
		screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
		childrenTransforms = new Transform[transform.childCount];
		spriteWidths = new float[transform.childCount];
		for (int i = 0; i < transform.childCount; i++)
		{
			childrenTransforms[i] = transform.GetChild(i).gameObject.transform;
			spriteWidths[i] = transform.GetChild(i).GetComponent<SpriteRenderer>().bounds.size.x;
		}
	}
	void LateUpdate()
	{
		float actualTrainSpeed = TrainManager.Instance.GetmainVelocity();
		if(actualTrainSpeed != speed && (speed * -1) < actualTrainSpeed)
			speed -= engineForce;
        else speed = actualTrainSpeed;
		

		if (speed > 0) speed *= -1;
		//Cantidad que se mueve, es decir velocidad
		float xSpeedMovement = speed * parallaxMultiplier * Time.deltaTime;
		for (int i = 0; i < transform.childCount; i++)
		{
			childrenTransforms[i].Translate(new Vector3(xSpeedMovement, 0, 0));
			//Si la posicion horizontal es mas grande que la pantalla mas el ancho del sprite
			//Mueve el objeto al inicio de la pantalla, es decir el ancho de la pantalla - el ancho del sprite por dos
			if (childrenTransforms[i].position.x < screenBounds.x - (spriteWidths[i] * 2))
				childrenTransforms[i].position = new Vector3(screenBounds.x + spriteWidths[i], childrenTransforms[i].position.y, childrenTransforms[i].position.z);
		}
	}
}