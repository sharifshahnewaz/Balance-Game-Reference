using UnityEngine;
using System.Collections;

public class DestroyByBounday : MonoBehaviour 
{
	void OnTriggerExit(Collider other)
	{
		Destroy(other.gameObject);
	}
}
