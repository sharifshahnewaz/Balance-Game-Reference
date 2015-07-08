using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
public class SkeletonJointsPositionDoubleExponentialFilter
    {
    
	public Dictionary<Kinect.JointType, Kinect.Joint> FilteredJoints = new Dictionary<Kinect.JointType, Kinect.Joint> ();
	/*{
		{ Kinect.JointType.FootLeft, new Kinect.Joint() },
		{ Kinect.JointType.AnkleLeft, new Kinect.Joint() },
		{ Kinect.JointType.KneeLeft, new Kinect.Joint() },
		{ Kinect.JointType.HipLeft,new Kinect.Joint()},
		
		{ Kinect.JointType.FootRight, new Kinect.Joint() },
		{ Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
		{ Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
		{ Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
		
		{ Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
		{ Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
		{ Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
		{ Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
		{ Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
		{ Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
		
		{ Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
		{ Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
		{ Kinect.JointType.HandRight, Kinect.JointType.WristRight },
		{ Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
		{ Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
		{ Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
		
		{ Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
		{ Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
		{ Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
		{ Kinect.JointType.Neck, Kinect.JointType.Head },
	};*/
	/// <summary>
        /// The previous data.
        /// </summary>

        private FilterDoubleExponentialData[] history; 


        /// <summary>
        /// The transform smoothing parameters for this filter.
        /// </summary>
        private TransformSmoothParameters smoothParameters;

        /// <summary>
        /// True when the filter parameters are initialized.
        /// </summary>
        private bool init;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkeletonJointsPositionDoubleExponentialFilter"/> class.
        /// </summary>
        public SkeletonJointsPositionDoubleExponentialFilter()
        {
            this.init = false;
        }

        /// <summary>
        /// Initialize the filter with a default set of TransformSmoothParameters.
        /// </summary>
        public void Init()
        {
            // Specify some defaults
            this.Init(.7f, .25f, .25f, .05f, .23f);
        }

        /// <summary>
        /// Initialize the filter with a set of manually specified TransformSmoothParameters.
        /// </summary>
        /// <param name="smoothingValue">Smoothing = [0..1], lower values is closer to the raw data and more noisy.</param>
        /// <param name="correctionValue">Correction = [0..1], higher values correct faster and feel more responsive.</param>
        /// <param name="predictionValue">Prediction = [0..n], how many frames into the future we want to predict.</param>
        /// <param name="jitterRadiusValue">JitterRadius = The deviation distance in m that defines jitter.</param>
        /// <param name="maxDeviationRadiusValue">MaxDeviation = The maximum distance in m that filtered positions are allowed to deviate from raw data.</param>
        public void Init(float smoothingValue, float correctionValue, float predictionValue, float jitterRadiusValue, float maxDeviationRadiusValue)
        {
            this.smoothParameters = new TransformSmoothParameters(); 

            this.smoothParameters.MaxDeviationRadius = maxDeviationRadiusValue; // Size of the max prediction radius Can snap back to noisy data when too high
            this.smoothParameters.Smoothing = smoothingValue;                   // How much soothing will occur.  Will lag when too high
            this.smoothParameters.Correction = correctionValue;                 // How much to correct back from prediction.  Can make things springy
            this.smoothParameters.Prediction = predictionValue;                 // Amount of prediction into the future to use. Can over shoot when too high
            this.smoothParameters.JitterRadius = jitterRadiusValue;             // Size of the radius where jitter is removed. Can do too much smoothing when too high
            this.Reset();
            this.init = true;
        }

        /// <summary>
        /// Initialize the filter with a set of TransformSmoothParameters.
        /// </summary>
        /// <param name="smoothingParameters">The smoothing parameters to filter with.</param>
        public void Init(TransformSmoothParameters smoothingParameters)
        {
            this.smoothParameters = smoothingParameters;
            this.Reset();
            this.init = true;
        }

        /// <summary>
        /// Resets the filter to default values.
        /// </summary>
        public void Reset()
        {
            this.history = new FilterDoubleExponentialData[(int)Kinect.JointType.ThumbRight + 1];
        }

        /// <summary>
        /// Update the filter with a new frame of data and smooth.
        /// </summary>
        /// <param name="skeleton">The Skeleton to filter.</param>
        public void UpdateFilter(ref Kinect.Body skeleton)
        {
            if (null == skeleton)
            {
                return;
            }

            if (!skeleton.IsTracked)
            {
                return;
            }

            if (this.init == false)
            {
                this.Init();    // initialize with default parameters                
            }

         //   Array jointTypeValues = Enum.GetValues(typeof(JointType));

            TransformSmoothParameters tempSmoothingParams = new TransformSmoothParameters();

            // Check for divide by zero. Use an epsilon of a 10th of a millimeter
            this.smoothParameters.JitterRadius = Mathf.Max(0.0001f, this.smoothParameters.JitterRadius);

            tempSmoothingParams.Smoothing = this.smoothParameters.Smoothing;
            tempSmoothingParams.Correction = this.smoothParameters.Correction;
            tempSmoothingParams.Prediction = this.smoothParameters.Prediction;

		for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
		
            { 
                // If not tracked, we smooth a bit more by using a bigger jitter radius
                // Always filter feet highly as they are so noisy
                if (skeleton.Joints[jt].TrackingState != Kinect.TrackingState.Tracked)
                {
                    tempSmoothingParams.JitterRadius *= 2.0f;
                    tempSmoothingParams.MaxDeviationRadius *= 2.0f;
                }
                else
                {
                    tempSmoothingParams.JitterRadius = this.smoothParameters.JitterRadius;
                    tempSmoothingParams.MaxDeviationRadius = this.smoothParameters.MaxDeviationRadius;
                }

                this.FilterJoint(ref skeleton, jt, tempSmoothingParams);
            }
        }

        /// <summary>
        /// Update the filter for one joint.  
        /// </summary>
        /// <param name="skeleton">The Skeleton to filter.</param>
        /// <param name="jt">The Skeleton Joint index to filter.</param>
        /// <param name="smoothingParameters">The Smoothing parameters to apply.</param>
		protected void FilterJoint(ref Kinect.Body skeleton, Kinect.JointType jt, TransformSmoothParameters smoothingParameters)
        {

            if (null == skeleton)
            {
                return;
            }


	//	Debug.Log("here");
            int jointIndex = (int)jt;

            Vector3 filteredPosition;
            Vector3 diffvec;
            Vector3 trend;
            float diffVal;

            Vector3 rawPosition = GetVector3FromJoint(skeleton.Joints[jt]);
            Vector3 prevFilteredPosition = this.history[jointIndex].FilteredPosition;
            Vector3 prevTrend = this.history[jointIndex].Trend;
            Vector3 prevRawPosition = this.history[jointIndex].RawPosition;
            bool jointIsValid = JointPositionIsValid(rawPosition);
		/*if(jt == Kinect.JointType.HandLeft)
			Debug.Log ("raw "  +":"+ rawPosition.x + ", " + rawPosition.y + ", " + rawPosition.z);
*/
            // If joint is invalid, reset the filter
            if (!jointIsValid)
            {
                this.history[jointIndex].FrameCount = 0;
            }

            // Initial start values
            if (this.history[jointIndex].FrameCount == 0)
            {
                filteredPosition = rawPosition;
                trend = Vector3.zero;
//			if(jt == Kinect.JointType.HandLeft)
//				Debug.Log ("filtered pos" +":"+ filteredPosition.x + ", " + filteredPosition.z + ", " + filteredPosition.z);

            }
            else if (this.history[jointIndex].FrameCount == 1)
            {
                filteredPosition = (rawPosition+ prevRawPosition) * 0.5f;
                diffvec = filteredPosition - prevFilteredPosition;
                trend = (diffvec * smoothingParameters.Correction) + (prevTrend * (1.0f - smoothingParameters.Correction));
            }
            else
            {              
                // First apply jitter filter
                diffvec = rawPosition - prevFilteredPosition;
                diffVal = Mathf.Abs(diffvec.magnitude);

                if (diffVal <= smoothingParameters.JitterRadius)
                {
                    filteredPosition = (rawPosition * (diffVal / smoothingParameters.JitterRadius)) + (prevFilteredPosition * ( 1.0f - (diffVal / smoothingParameters.JitterRadius)));
                }
                else
                {
                    filteredPosition = rawPosition;
                }

                // Now the double exponential smoothing filter
                filteredPosition = (filteredPosition *(1.0f - smoothingParameters.Smoothing)) +  ((prevFilteredPosition + prevTrend) * smoothingParameters.Smoothing);
		//	if(jt == Kinect.JointType.HandLeft)
			//					Debug.Log ("filtered pos" +":"+ filteredPosition.x + ", " + filteredPosition.z + ", " + filteredPosition.z);
                diffvec = filteredPosition - prevFilteredPosition;
                trend = (diffvec * smoothingParameters.Correction) + (prevTrend *( 1.0f - smoothingParameters.Correction));

            }      

            // Predict into the future to reduce latency
            Vector3 predictedPosition = filteredPosition + (trend *smoothingParameters.Prediction);

            // Check that we are not too far away from raw data
            diffvec = predictedPosition- rawPosition;
            diffVal = Mathf.Abs(diffvec.magnitude);

            if (diffVal > smoothingParameters.MaxDeviationRadius)
            {
                predictedPosition = (predictedPosition * (smoothingParameters.MaxDeviationRadius / diffVal)) + (rawPosition* ( 1.0f - (smoothingParameters.MaxDeviationRadius / diffVal)));
            }

            // Save the data from this frame
            this.history[jointIndex].RawPosition = rawPosition;
            this.history[jointIndex].FilteredPosition = filteredPosition;
            this.history[jointIndex].Trend = trend;
            this.history[jointIndex].FrameCount++;
		//Debug.Log (this.history[jointIndex].FrameCount);
            // Set the filtered data back into the joint
            Kinect.Joint j = new Kinect.Joint();
		j.Position = Vector3ToSkeletonPoint(predictedPosition);
		//if(skeleton.Joints.ContainsKey(jt))
			//skeleton.Joints.Remove(jt);
	 	FilteredJoints[jt] = j;// =Vector3ToSkeletonPoint(predictedPosition);// j;

	/*	if (jt == Kinect.JointType.HandLeft) {
						Debug.Log ("predicted " + ":" + j.Position.X + ", " + j.Position.Y + ", " + j.Position.Z);
						//Debug.Log ("new " + ":" + skeleton.Joints[jt].Position.X + ", " + skeleton.Joints[jt].Position.Y + ", " + skeleton.Joints[jt].Position.Z);
				}*/



        }

        /// <summary>
        /// Historical Filter Data.  
        /// </summary>
        private struct FilterDoubleExponentialData
        {
            /// <summary>
            /// Gets or sets Historical Position.  
            /// </summary>
            public Vector3 RawPosition { get; set; }

            /// <summary>
            /// Gets or sets Historical Filtered Position.  
            /// </summary>
            public Vector3 FilteredPosition { get; set; }

            /// <summary>
            /// Gets or sets Historical Trend.  
            /// </summary>
            public Vector3 Trend { get; set; }

            /// <summary>
            /// Gets or sets Historical FrameCount.  
            /// </summary>
            public uint FrameCount { get; set; }
        }

		public struct TransformSmoothParameters{
			public float Smoothing;
				public float Correction; 
				public float Prediction; 
				public float JitterRadius; 
				public float MaxDeviationRadius;
		}

	private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
	{
		return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
	}
	private static Kinect.CameraSpacePoint Vector3ToSkeletonPoint(Vector3 position)
	{
		//Kinect.Joint joint = new Kinect.Joint();
		Kinect.CameraSpacePoint Position = new Kinect.CameraSpacePoint ();
		Position.X = position.x / 10f;
		Position.Y = position.y / 10f;
		Position.Z = position.z/ 10f;
		return Position;
	}

	public static bool JointPositionIsValid(Vector3 jointPosition)
	{
		return jointPosition.x != 0.0f || jointPosition.y != 0.0f || jointPosition.z != 0.0f;
	}
    }