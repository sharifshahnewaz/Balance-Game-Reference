using UnityEngine;
using System.Collections;

public class BallRotation : MonoBehaviour
{	
		// Update is called once per frame
		void Update ()
		{
				transform.Rotate (0, 100 * Time.deltaTime, 0);
	
		}
}
