using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

public class GameController : MonoBehaviour
{
	public GameObject tennisball;
	
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;
	private GameObject head;
	private GameObject OVRPlayerController;
	
	//public GUIText scoreText;
	//public GUIText restartText;
	//public GUIText gameOverText;

	
	private int hit;
	private int miss;
	private bool play;
	
	
	UIVA_Client_WiiFit theClient;
	string ipUIVAServer = "127.0.0.1";
	
	private double gravX = 0.0, gravY = 0.0, weight = 0.0, prevGravX = 0.0, prevGravY = 0.0, pathX = 0.0, pathY = 0.0, path = 0.0;
	private bool isFirstRow = true;
	private double tl = 0.0, tr = 0.0, bl = 0.0, br = 0.0;
	private string fitbutt = "";
	private double elapsedTime = 0.0f;
	public int totalBall = 120;

	private string displayMessage = null;
	private StringBuilder balanceDataRecorder;
	
	
	public int sampleRate = 10;	
	public String studyCondition;
	
	public GameObject hitText;
	public GameObject missText;
	public GameObject messageText;
	
	void Awake ()
	{
		if (Application.platform == RuntimePlatform.WindowsWebPlayer ||
			Application.platform == RuntimePlatform.OSXWebPlayer) {
			if (Security.PrefetchSocketPolicy (ipUIVAServer, 843, 500)) {
				Debug.Log ("Got socket policy");	
			} else {
				Debug.Log ("Cannot get socket policy");	
			}
		}
	}		
	
	void Start ()
	{
		play = false;
		hit = 0;
		miss = 0;
				
		//Time.timeScale = 0;	

		head = GameObject.FindWithTag ("Head");

		if (head == null) {
			Debug.Log ("Cannot find 'Head' of the avatar");
		}
		try {
			theClient = new UIVA_Client_WiiFit (ipUIVAServer);
		} catch (Exception e) {
			Debug.Log (e.ToString ());
		}
		displayMessage = "'P' to \nplay";
		balanceDataRecorder = new StringBuilder ();
		balanceDataRecorder.Append ("System Time,Elapsed Time,Gravity X,Gravity Y, PathX, PathY, Path, Weight, Hit, Miss\n");
		StartCoroutine (SpawnWaves ());
		StartCoroutine (WriteInFile ());	
	}
	
	void Update ()
	{
		
		if (Input.GetKeyDown (KeyCode.P)) {
			if (play) {
				play = false;				
				displayMessage = "'P' to \nplay";
			} else {
				play = true;
				displayMessage = "'P' to \nstop";
			}
			
		} 
		if (Input.GetKeyDown (KeyCode.R)) {
			OVRPlayerController = GameObject.Find ("OVRPlayerController");
			OVRPlayerController.transform.rotation = Quaternion.identity;
		}
		if (hit + miss >= totalBall) {
			play = false;
			displayMessage = "Game\nOver";
		}
		hitText.GetComponent <TextMesh> ().text = "Hit: " + hit;
		missText.GetComponent <TextMesh> ().text = "Miss: " + miss;
		messageText.GetComponent <TextMesh> ().text = displayMessage;
		
	}

	IEnumerator SpawnWaves ()
	{
		yield return new WaitForSeconds (startWait);
	
		while (true) {
			
						
			for (int i = 0; i < hazardCount; i++) {
				GameObject bowlingMachineHead = GameObject.FindGameObjectsWithTag ("BowlingMachineHead") [0];
				Vector3 spawnPosition = new Vector3 (bowlingMachineHead.transform.position.x, 
								bowlingMachineHead.transform.position.y, bowlingMachineHead.transform.position.z);
				Quaternion spawnRotation = Quaternion.identity;
				if (head != null) {
					spawnRotation = Quaternion.LookRotation (spawnPosition - head.transform.position - new Vector3 (0, 0.099f, 0.134f));
				}	
				if (play) {							
					Instantiate (tennisball, spawnPosition, spawnRotation);
					
				}		
				yield return new WaitForSeconds (spawnWait);
			}
			
			yield return new WaitForSeconds (waveWait);
			
							
						
		}
	}
	
	IEnumerator WriteInFile ()
	{
		//yield return new WaitForSeconds (0.001f);
		while (true) {
			//theClient.GetWiiFitRawData (out tl, out tr, out bl, out br, out fitbutt);
			try {
				theClient.GetWiiFitGravityData (out weight, out gravX, out gravY, out fitbutt);
			} catch (Exception ex) {
				Debug.Log (ex.ToString ());
			}
			if (!isFirstRow) {
				pathX = Math.Abs (gravX - prevGravX);
				pathY = Math.Abs (gravY - prevGravY);
				path = Math.Sqrt (pathX * pathX + pathY * pathY);
			}
			if (play) {
			
				balanceDataRecorder.Append (String.Format ("{0},{1},{2},{3},{4},{5},{6},{7},{7},{8}\n", System.DateTime.Now, elapsedTime, gravX, gravY, pathX, pathY, path, weight, hit, miss));
				prevGravX = gravX;
				prevGravY = gravY;
				isFirstRow = false;
				elapsedTime += (1.0f / sampleRate);
				
				
			}
			
			yield return new WaitForSeconds ((1.0f / sampleRate));
		}
		
	}
	
	public void AddHit ()
	{
		hit += 1;
		//UpdateScore ();
	}
	public void AddMiss ()
	{
		miss += 1;
		//UpdateScore ();
	}
		
		
	void OnApplicationQuit ()
	{
		
		long fileId = System.DateTime.Now.Ticks;
		//TODO remove comments below
		System.IO.File.AppendAllText (studyCondition + "-balance-" + fileId.ToString () + ".csv", balanceDataRecorder.ToString ());
		System.IO.File.AppendAllText (studyCondition + "-score-" + fileId.ToString () + ".txt", "Hit: " + hit + ", Miss: " + miss);
	}
}