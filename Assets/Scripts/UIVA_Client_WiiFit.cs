///-----------------------------------------------------------------------------------
///-----------------------------------------------------------------------------------
/// UIVA (Unity Indie VRPN Adapter) Unity
/// 
/// Function: The client side of UIVA living in Unity as a DLL file.
///           Unity creates a UIVA class and calls its GetXXXData(out X, out X) functions
///           to get the latest data from the sensor devices.
/// 
/// About UIVA:
/// 
///   UIVA is a middle-ware between VRPN and Unity. It enables games developed by Unity3D INDIE
///   to be controlled by devices powered by VRPN. It has a client and a server simultaneously.
///   For VRPN, UIVA is its client which implements several callback functions to receive the 
///   latest data from the devices. For Unity, UIVA is a server that stores the latest sensor
///   data which allows it to query. The framework is shown as below:
///   
///        ~~~Sensor~~~      ~~~VRPN~~~      ~~~~~~~~~~~~UIVA~~~~~~~~~~~~~~~    ~~~Unity3D~~~     
///        
///   Kinect-----(FAAST)---->|--------|    |--------|--------|    |---------|
///    Wii ----(VRPN Wii)--->|        |    |        |        |    |         |--->Object transform
///   BPack --(VRPN BPack)-->|  VRPN  |    |  VRPN  | Unity  |    |  Unity  |
///           ...            |        |===>|  .net  | socket |===>|  socket |--->GUI
///           ...            | server |    |        |        |    |         |
///           ...            |        |    | client | server |    |  client |--->etc. of Unity3D
///           ...            |--------|    |--------|--------|    |---------|
///    
/// Special note: 
///
///      The VRPNWrapper implemented by the AR lab of Georgia Institute of Technology offers
///   a easier to use wrapper of VRPN to be used as a plugin in Unity3D Pro. If you can afford 
///   the Pro version of Unity. I suggest you to use VRPNWrapper. Their website is:
///           https://research.cc.gatech.edu/uart/content/about
///   They also implemented a ARToolkit wrapper which enables AR application in Unity. 
///   Check out their UART project, it is awesome!
///    
/// Author: 
/// 
/// Jia Wang (wangjia@wpi.edu)
/// Human Interaction in Virtual Enviroments (HIVE) Lab
/// Department of Computer Science
/// Worcester Polytechnic Institute
/// 
/// History: (1.0) 01/11/2011  by  Jia Wang
///
/// Acknowledge: Thanks to Chris VanderKnyff for the .NET version of VRPN
///                     to UNC for the awesome VRPN
///                     to Unity3D team for the wonderful engine
///              
///              and above all, special thanks to 
///                 Prof. Robert W. Lindeman (http://www.wpi.edu/~gogo) 
///              for the best academic advice.
///              
///-----------------------------------------------------------------------------------
///-----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

/// <summary>
/// UIVA_Client class
/// </summary>
public class UIVA_Client_WiiFit
{
	//Liberal values
	const int X = 0;
	const int Y = 1;
	const int Z = 2;

	Socket socClient;       //Socket deal with communication
	byte[] recBuffer = new byte[256];   //Receive buffer
	string recStr = "";                 //Deciphered receive 

	/// <summary>
	/// Connect and test connection
	/// </summary>
	/// <param name="serverIP">The IP address of the server, 
	/// should be the local IP if used as Unity interface</param>
	public UIVA_Client_WiiFit (string serverIP)
	{
		// If the UIVA server is in the local machine,
		// find its IP address and connect automatically
		if (serverIP == "localhost") {
			IPHostEntry host;
			host = Dns.GetHostEntry (Dns.GetHostName ()); 
			foreach (IPAddress ip in host.AddressList) {
				if (ip.AddressFamily.ToString () == "InterNetwork") {
					serverIP = ip.ToString ();
				}
			}
		}

		try {
			//Create a client socket
			socClient = new Socket (AddressFamily.InterNetwork,
                                SocketType.Stream, ProtocolType.IP);
			//Parse the IP address string into an IPAddress object
			IPAddress serverAddr = IPAddress.Parse (serverIP);
			//Port: 8881
			IPEndPoint serverMachine = new IPEndPoint (serverAddr, 8881);
			//Connect
			socClient.Connect (serverMachine);
			//Send a confirmation message
			SendMessage ("Ready?\n");
			ReceiveMessage ();
			if (recStr != "Ready!") {
				throw new Exception ("Not ready?");
			}
		} catch (Exception e) {
			Exception initError = new Exception (e.ToString ()
				+ "\nClient failed to connect to server. Is your IP correct?"
				+ "Is your UIVA working\n");
			throw initError;
		}
	}

	/// <summary>
	/// Send a message to the server
	/// </summary>
	/// <param name="msg">message content, end with a '\n'</param>
	private void SendMessage (string msg)
	{
		try {
			//Encode a message
			byte[] sendBuffer = Encoding.ASCII.GetBytes (msg);
			socClient.Send (sendBuffer);
		} catch (Exception e) {
			Exception sendError = new Exception (e.ToString () + "Client failed to send message.\n");
			throw sendError;
		}
	}

	/// <summary>
	/// Receive message from the server, decode and store in "recStr" variable
	/// </summary>
	private void ReceiveMessage ()
	{
		try {
			socClient.Receive (recBuffer);
			recStr = Encoding.Default.GetString (recBuffer);
			//Remove the tailing '\0's after the '\n' token, caused by the buffer size
			int ixEnd = recStr.IndexOf ('\n');
			recStr = recStr.Remove (ixEnd);
		} catch (Exception e) {
			Exception recError = new Exception (e.ToString ()
				+ "Client failed to receive message.\n");
			throw recError;
		}
	}
	/// <summary>
	/// Get WiiFit Raw data
	/// </summary>
	/// <param name="topLeft"> Pressure sensor data on the top left corner</param>
	/// <param name="topRight"> Pressure sensor data on the top right corner</param>
	/// <param name="bottomLeft"> Pressure sensor data on the bottom left corner</param>
	/// <param name="bottomRight"> Pressure sensor data on the bottom right corner</param>
	/// <param name="buttons"> Button A: "A" for press, "a" for release</param>
	/// <returns></returns>
	public void GetWiiFitRawData (int which, out double topLeft, out double topRight,
        out double bottomLeft, out double bottomRight, out String buttons)
	{
		SendMessage (String.Format ("WiiFit?{0}?\n", which));
		ReceiveMessage ();
		try {
			string[] sections = recStr.Split (new char[] { ',' });
			//Debug.Log (recStr);
			//Skip "WIIFIT,,"
			topLeft = System.Convert.ToDouble (sections [2]);
			topRight = System.Convert.ToDouble (sections [3]);
			bottomLeft = System.Convert.ToDouble (sections [4]);
			bottomRight = System.Convert.ToDouble (sections [5]);
			//Skip the analog timestamp, uncomment this line and add a corresponding out DateTime variable to test time delay
			//anaTS = DateTime.Parse(sections[6]);
			buttons = sections [5];
			//Skip the button timestamp, uncomment this line and add a corresponding out DateTime variable to test time delay
			//buttTS = DateTime.Parse(sections[8]);
		} catch (Exception e) {
			throw new Exception (e.ToString () + "\n\nRECEIVED FROM UIVA_SERVER: " + recStr);
		}
	}

	/* An overload which requires no 'which' parameter, used when there is only one wiifit */
	public void GetWiiFitRawData (out double topLeft, out double topRight,
        out double bottomLeft, out double bottomRight, out String buttons)
	{
		GetWiiFitRawData (1, out topLeft, out topRight, out bottomLeft, out bottomRight, out buttons);
	}

	/// <summary>
	/// Get WiiFit Center of Gravity data
	/// </summary>
	/// <param name="weight"> Weight of the person in Kg</param>
	/// <param name="gravX"> Gravity along the X axis (the longer one), extends to the right</param>
	/// <param name="gravY"> Gravity along the Y axis (the shorter one), extends upwards</param>
	/// <param name="buttons"> Button A: uppercase for press, lowercase for release</param>
	public void GetWiiFitGravityData (int which, out double weight, out double gravX, out double gravY, out String buttons)
	{
		// These computation formulas are borrowed from the WiimoteLib by Brian Peek
		float BSL = 43.0f;   // Length between board censors
		float BSW = 24.0f;   // Width between board censors
		double tl = 0.0, tr = 0.0, bl = 0.0, br = 0.0;
		GetWiiFitRawData (which, out tl, out tr, out bl, out br, out buttons);
		float kx = (float)((tl + bl) / (tr + br));
		float ky = (float)((tl + tr) / (bl + br));
		gravX = (kx - 1.0f) / (kx + 1.0f) * (BSL / 2.0f); //- is removed to make x positive right when wii board is reversed
		gravY = (ky - 1.0f) / (ky + 1.0f) * (-BSW / 2.0f); 
		weight = (float)(tl + tr + bl + br) / 4.0;
	}

	/* An overload which requires no 'which' parameter, used when there is only one wiifit */
	public void GetWiiFitGravityData (out double weight, out double gravX, out double gravY, out String buttons)
	{
		GetWiiFitGravityData (1, out weight, out gravX, out gravY, out buttons);
	}

	/// <summary>
	/// Disconnect from UIVA
	/// </summary>
	public void Disconnect ()
	{
		SendMessage ("Bye?\n");      //Send disconnect request
	}
}



