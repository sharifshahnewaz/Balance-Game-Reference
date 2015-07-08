using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour
{
	public Material BoneMaterial;
	public GameObject BodySourceManager;
		
	private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject> ();
	private BodySourceManager _BodyManager;
	    
	private static Vector3 transformDistance = new Vector3 (10.1f, 10.1f, 10.1f);
	private SkeletonJointsPositionDoubleExponentialFilter filter = new SkeletonJointsPositionDoubleExponentialFilter ();
	//private StringBuilder jointDataRecorder;
	/*void Start ()
	{	
		jointDataRecorder = new StringBuilder ();
		for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.HandRight; jt++) {
			jointDataRecorder.Append (jt.ToString () + "-X, ");
			jointDataRecorder.Append (jt.ToString () + "-Y, ");
			jointDataRecorder.Append (jt.ToString () + "-Z, ");
		}
		jointDataRecorder.Append ("\n");
	}*/
	    		  
	void Update ()
	{
		if (BodySourceManager == null) {
			return;
		}
        
		_BodyManager = BodySourceManager.GetComponent<BodySourceManager> ();
		if (_BodyManager == null) {
			return;
		}
        
		Kinect.Body[] data = _BodyManager.GetData ();
		if (data == null) {
			return;
		}
        
		List<ulong> trackedIds = new List<ulong> ();
		foreach (var body in data) {
			if (body == null) {
				continue;
			}
                
			if (body.IsTracked) {
				trackedIds.Add (body.TrackingId);
			}
		}
        
		List<ulong> knownIds = new List<ulong> (_Bodies.Keys);
        
		// First delete untracked bodies
		foreach (ulong trackingId in knownIds) {
			if (!trackedIds.Contains (trackingId)) {
				Destroy (_Bodies [trackingId]);
				_Bodies.Remove (trackingId);
			}
		}

		foreach (var body in data) {				
						
			if (body == null) {
				continue;
			}
            
			if (body.IsTracked) {
						
				if (!_Bodies.ContainsKey (body.TrackingId)) {
					_Bodies [body.TrackingId] = CreateBodyObject (body.TrackingId);
				}
                
				RefreshBodyObject (body, _Bodies [body.TrackingId]);
				RotateAvatarJoints (body, _Bodies [body.TrackingId]);
			}
		}
	}
    
	private GameObject CreateBodyObject (ulong id)
	{
		GameObject body = new GameObject ("Body:" + id);
		body.transform.position = Vector3.zero;
				       
		for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.HandRight; jt++) {
				
			GameObject jointObj = GameObject.CreatePrimitive (PrimitiveType.Cube);						
			jointObj.GetComponent<Renderer> ().enabled = false;
			jointObj.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
			jointObj.name = jt.ToString ();
			jointObj.transform.parent = body.transform;
		}
        
		return body;
	}
    
	private void RefreshBodyObject (Kinect.Body body, GameObject bodyObject)
	{
		filter.UpdateFilter (ref body);
		for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.HandRight; jt++) {
			//Kinect.Joint sourceJoint = body.Joints [jt];
			GameObject avatarSpineBase = GameObject.FindGameObjectsWithTag (Kinect.JointType.SpineBase.ToString ()) [0];
			Transform jointObj = bodyObject.transform.FindChild (jt.ToString ());
			Kinect.CameraSpacePoint jointPosition = filter.FilteredJoints [jt].Position;
			jointPosition.Z = -jointPosition.Z;
			
			//jointDataRecorder.Append (string.Format ("{0},{1},{2},{3}", System.DateTime.Now, jointPosition.X, jointPosition.Y, jointPosition.Z));
						
			if (jt == Kinect.JointType.SpineBase) {			
				transformDistance = new Vector3 (jointPosition.X - avatarSpineBase.transform.position.x, 
				                                 jointPosition.Y - avatarSpineBase.transform.position.y, 
				                                 jointPosition.Z - avatarSpineBase.transform.position.z);
								
			}
						
			jointObj.position = new Vector3 ((jointPosition.X - transformDistance.x), 
			                                 (jointPosition.Y - transformDistance.y), 
			                                 (jointPosition.Z - transformDistance.z));
			if (jt <= Kinect.JointType.ShoulderLeft || jt == Kinect.JointType.ShoulderRight) {
				GameObject avatarJoint = GameObject.FindGameObjectsWithTag (jt.ToString ()) [0];         
				avatarJoint.transform.position = jointObj.transform.position;         
			}
						
		}
		//jointDataRecorder.Append ("\n");
	}
		
	private void RotateAvatarJoints (Kinect.Body body, GameObject bodyObject)
	{
		RotateBone (bodyObject, Kinect.JointType.ShoulderRight, Kinect.JointType.ElbowRight);
		RotateBone (bodyObject, Kinect.JointType.ShoulderLeft, Kinect.JointType.ElbowLeft);
		RotateBone (bodyObject, Kinect.JointType.ElbowRight, Kinect.JointType.WristRight);
		RotateBone (bodyObject, Kinect.JointType.ElbowLeft, Kinect.JointType.WristLeft);
				
				
	}
		
	private void RotateBone (GameObject bodyObject, Kinect.JointType startBone, Kinect.JointType endBone)
	{
		GameObject avatarJoint = GameObject.FindGameObjectsWithTag (startBone.ToString ()) [0];
		Transform jointObj = bodyObject.transform.FindChild (startBone.ToString ());
		Transform targetJointObj = bodyObject.transform.FindChild (endBone.ToString ());
		
		Quaternion avatarRotaion = Quaternion.LookRotation ((targetJointObj.transform.position - jointObj.transform.position).normalized);				
		avatarJoint.transform.rotation = Quaternion.Slerp (avatarJoint.transform.rotation, avatarRotaion, Time.deltaTime * 2000);				
		avatarJoint.transform.Rotate (new Vector3 (90, 0, 0));
		
	}
	/*void OnApplicationQuit ()
	{
				
		long fileId = System.DateTime.Now.Ticks;
		//TODO remove comments below
		System.IO.File.AppendAllText ("Joint-data-" + fileId.ToString () + ".csv", jointDataRecorder.ToString ());
	}*/
}
