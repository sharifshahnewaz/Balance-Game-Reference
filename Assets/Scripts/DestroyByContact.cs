using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour
{
	//public GameObject explosion;
	//public GameObject playerExplosion;
	public int scoreValue;
	private GameController gameController;

	
	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent <GameController> ();
		}
		if (gameController == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}
	}
	
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Boundary") {
			gameController.AddMiss ();
			AudioSource ballShooting = GameObject.Find ("BallShooting").GetComponent<AudioSource> ();
			ballShooting.pitch = 0.6f;
			ballShooting.volume = 0.03f;
			ballShooting.Play ();
			//return;
		}

				//need to instantiatte explosion
				//Instantiate(explosion, transform.position, transform.rotation);
				else {
			gameController.AddHit ();
			AudioSource ballHit = GameObject.Find ("BallHit").GetComponent<AudioSource> ();
			ballHit.pitch = 0.6f;
			ballHit.Play ();
			//Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
			//gameController.GameOver();
		}
		//gameController.AddScore (scoreValue);
		//Destroy(other.gameObject);
		Destroy (gameObject);
	}
}