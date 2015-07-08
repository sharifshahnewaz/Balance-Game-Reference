using UnityEngine;
using System.Collections;

public class Drawlines : MonoBehaviour
{

	public Material lineMaterial;
	// Use this for initialization
	void Start ()
	{
		ArrayList linesList = new ArrayList ();
		ArrayList lines = new ArrayList ();
		#region floor
		ArrayList go = new ArrayList ();
		ArrayList goList = new ArrayList ();
		
		for (int i=0; i<12; i++) {
			go.Add (GameObject.CreatePrimitive (PrimitiveType.Sphere));
			GameObject temp = (GameObject)go [i];
			lines.Add (temp.AddComponent<LineRenderer> ());
			LineRenderer lr = (LineRenderer)lines [i];
			temp.transform.localScale = new Vector3 (0.01f, 0.01f, .01f);
			lr.SetVertexCount (2);		
			lr.SetWidth (0.03f, 0.03f);		
			lr.material = lineMaterial;
			//lr [0,i].SetColors (Color.gray, Color.gray);
			lr.SetPosition (0, new Vector3 (-20, 0, (i - 3) * 1.11f));
			lr.SetPosition (1, new Vector3 (9, 0, (i - 3) * 1.11f));
			//startPoint.transform.localScale = new Vector3 (0.3f, 0.3f, 0.3f);
		}
		linesList.Add (lines);
		goList.Add (go);
		
		go = new ArrayList ();
		lines = new ArrayList ();
		for (int i=0; i<20; i++) {
			go.Add (GameObject.CreatePrimitive (PrimitiveType.Sphere));
			GameObject temp = (GameObject)go [i];
			lines.Add (temp.AddComponent<LineRenderer> ());
			LineRenderer lr = (LineRenderer)lines [i];
			temp.transform.localScale = new Vector3 (0.01f, 0.01f, .01f);
			lr.SetVertexCount (2);		
			lr.SetWidth (0.03f, 0.03f);		
			lr.material = lineMaterial;
			//lr [0,i].SetColors (Color.gray, Color.gray);
			lr.SetPosition (0, new Vector3 ((i - 14) * 1.11f - 0.22f, 0, -3));
			lr.SetPosition (1, new Vector3 ((i - 14) * 1.11f - 0.22f, 0, 10));
			//startPoint.transform.localScale = new Vector3 (0.3f, 0.3f, 0.3f);
		}
		linesList.Add (lines);
		goList.Add (go);
		#endregion
		
		#region Right wall
		go = new ArrayList ();
		lines = new ArrayList ();
		for (int i=0; i<10; i++) {
			go.Add (GameObject.CreatePrimitive (PrimitiveType.Sphere));
			GameObject temp = (GameObject)go [i];
			lines.Add (temp.AddComponent<LineRenderer> ());
			LineRenderer lr = (LineRenderer)lines [i];
			temp.transform.localScale = new Vector3 (0.01f, 0.01f, .01f);
			lr.SetVertexCount (2);		
			lr.SetWidth (0.03f, 0.03f);		
			lr.material = lineMaterial;
			//lr [0,i].SetColors (Color.gray, Color.gray);
			lr.SetPosition (0, new Vector3 (5.05f, 0, (i - 2) * 1.11f));
			lr.SetPosition (1, new Vector3 (5.05f, 4.5f, (i - 2) * 1.11f));
			//startPoint.transform.localScale = new Vector3 (0.3f, 0.3f, 0.3f);
		}
		linesList.Add (lines);
		goList.Add (go);	

		go = new ArrayList ();
		lines = new ArrayList ();
		for (int i=0; i<5; i++) {
			go.Add (GameObject.CreatePrimitive (PrimitiveType.Sphere));
			GameObject temp = (GameObject)go [i];
			lines.Add (temp.AddComponent<LineRenderer> ());
			LineRenderer lr = (LineRenderer)lines [i];
			temp.transform.localScale = new Vector3 (0.01f, 0.01f, .01f);
			lr.SetVertexCount (2);		
			lr.SetWidth (0.03f, 0.03f);		
			lr.material = lineMaterial;
			//lr [0,i].SetColors (Color.gray, Color.gray);
			lr.SetPosition (0, new Vector3 (5.05f, (i * 1.11f), -2));
			lr.SetPosition (1, new Vector3 (5.05f, (i * 1.11f), 9));
		}
		linesList.Add (lines);
		goList.Add (go);
		#endregion
			
		#region Frontwall
		go = new ArrayList ();
		lines = new ArrayList ();
		for (int i=0; i<8; i++) {
			go.Add (GameObject.CreatePrimitive (PrimitiveType.Sphere));
			GameObject temp = (GameObject)go [i];
			lines.Add (temp.AddComponent<LineRenderer> ());
			LineRenderer lr = (LineRenderer)lines [i];
			temp.transform.localScale = new Vector3 (0.01f, 0.01f, .01f);
			lr.SetVertexCount (2);		
			lr.SetWidth (0.03f, 0.03f);		
			lr.material = lineMaterial;
			//lr [0,i].SetColors (Color.gray, Color.gray);
			lr.SetPosition (0, new Vector3 (2 + (i - 5) * 1.11f, 0, 8.38f));
			lr.SetPosition (1, new Vector3 (2 + (i - 5) * 1.11f, 5, 8.38f));
		}
		linesList.Add (lines);
		goList.Add (go);	

		go = new ArrayList ();
		lines = new ArrayList ();
		for (int i=0; i<5; i++) {
			go.Add (GameObject.CreatePrimitive (PrimitiveType.Sphere));
			GameObject temp = (GameObject)go [i];
			lines.Add (temp.AddComponent<LineRenderer> ());
			LineRenderer lr = (LineRenderer)lines [i];
			temp.transform.localScale = new Vector3 (0.01f, 0.01f, .01f);
			lr.SetVertexCount (2);		
			lr.SetWidth (0.03f, 0.03f);		
			lr.material = lineMaterial;
			//lr [0,i].SetColors (Color.gray, Color.gray);
			lr.SetPosition (0, new Vector3 (-5, (i * 1.11f), 8.38f));
			lr.SetPosition (1, new Vector3 (5, (i * 1.11f), 8.38f));
		}
		linesList.Add (lines);
		goList.Add (go);
		#endregion
		
		#region Left wall
		go = new ArrayList ();
		lines = new ArrayList ();
		for (int i=0; i<4; i++) {
			go.Add (GameObject.CreatePrimitive (PrimitiveType.Sphere));
			GameObject temp = (GameObject)go [i];
			lines.Add (temp.AddComponent<LineRenderer> ());
			LineRenderer lr = (LineRenderer)lines [i];
			temp.transform.localScale = new Vector3 (0.01f, 0.01f, .01f);
			lr.SetVertexCount (2);		
			lr.SetWidth (0.03f, 0.03f);		
			lr.material = lineMaterial;
			//lr [0,i].SetColors (Color.gray, Color.gray);
			lr.SetPosition (0, new Vector3 (-4.72f, 0, (i + 4) * 1.11f));
			lr.SetPosition (1, new Vector3 (-4.72f, 4.5f, (i + 4) * 1.11f));
			//startPoint.transform.localScale = new Vector3 (0.3f, 0.3f, 0.3f);
		}
		linesList.Add (lines);
		goList.Add (go);	
		
		go = new ArrayList ();
		lines = new ArrayList ();
		for (int i=0; i<5; i++) {
			go.Add (GameObject.CreatePrimitive (PrimitiveType.Sphere));
			GameObject temp = (GameObject)go [i];
			lines.Add (temp.AddComponent<LineRenderer> ());
			LineRenderer lr = (LineRenderer)lines [i];
			temp.transform.localScale = new Vector3 (0.01f, 0.01f, .01f);
			lr.SetVertexCount (2);		
			lr.SetWidth (0.03f, 0.03f);		
			lr.material = lineMaterial;
			//lr [0,i].SetColors (Color.gray, Color.gray);
			lr.SetPosition (0, new Vector3 (-4.72f, (i * 1.11f), 4.45f));
			lr.SetPosition (1, new Vector3 (-4.72f, (i * 1.11f), 9));
		}
		linesList.Add (lines);
		goList.Add (go);
		#endregion
		
		#region Left distant wall
		go = new ArrayList ();
		lines = new ArrayList ();
		for (int i=0; i<6; i++) {
			go.Add (GameObject.CreatePrimitive (PrimitiveType.Sphere));
			GameObject temp = (GameObject)go [i];
			lines.Add (temp.AddComponent<LineRenderer> ());
			LineRenderer lr = (LineRenderer)lines [i];
			temp.transform.localScale = new Vector3 (0.01f, 0.01f, .01f);
			lr.SetVertexCount (2);		
			lr.SetWidth (0.03f, 0.03f);		
			lr.material = lineMaterial;
			//lr [0,i].SetColors (Color.gray, Color.gray);
			lr.SetPosition (0, new Vector3 (-16.05f, 0, (i - 1) * 1.11f));
			lr.SetPosition (1, new Vector3 (-16.05f, 4.5f, (i - 1) * 1.11f));
			//startPoint.transform.localScale = new Vector3 (0.3f, 0.3f, 0.3f);
		}
		linesList.Add (lines);
		goList.Add (go);	
		
		go = new ArrayList ();
		lines = new ArrayList ();
		for (int i=0; i<5; i++) {
			go.Add (GameObject.CreatePrimitive (PrimitiveType.Sphere));
			GameObject temp = (GameObject)go [i];
			lines.Add (temp.AddComponent<LineRenderer> ());
			LineRenderer lr = (LineRenderer)lines [i];
			temp.transform.localScale = new Vector3 (0.01f, 0.01f, .01f);
			lr.SetVertexCount (2);		
			lr.SetWidth (0.03f, 0.03f);		
			lr.material = lineMaterial;
			//lr [0,i].SetColors (Color.gray, Color.gray);
			lr.SetPosition (0, new Vector3 (-16.05f, (i * 1.11f), -2));
			lr.SetPosition (1, new Vector3 (-16.05f, (i * 1.11f), 4.45f));
		}
		linesList.Add (lines);
		goList.Add (go);
		#endregion
		
		#region Left Frontwall
		go = new ArrayList ();
		lines = new ArrayList ();
		for (int i=0; i<10; i++) {
			go.Add (GameObject.CreatePrimitive (PrimitiveType.Sphere));
			GameObject temp = (GameObject)go [i];
			lines.Add (temp.AddComponent<LineRenderer> ());
			LineRenderer lr = (LineRenderer)lines [i];
			temp.transform.localScale = new Vector3 (0.01f, 0.01f, .01f);
			lr.SetVertexCount (2);		
			lr.SetWidth (0.03f, 0.03f);		
			lr.material = lineMaterial;
			//lr [0,i].SetColors (Color.gray, Color.gray);
			lr.SetPosition (0, new Vector3 (2 + (i - 16) * 1.11f, 0, 4.42f));
			lr.SetPosition (1, new Vector3 (2 + (i - 16) * 1.11f, 4.2f, 4.42f));
		}
		linesList.Add (lines);
		goList.Add (go);	
		
		go = new ArrayList ();
		lines = new ArrayList ();
		for (int i=0; i<5; i++) {
			go.Add (GameObject.CreatePrimitive (PrimitiveType.Sphere));
			GameObject temp = (GameObject)go [i];
			lines.Add (temp.AddComponent<LineRenderer> ());
			LineRenderer lr = (LineRenderer)lines [i];
			temp.transform.localScale = new Vector3 (0.01f, 0.01f, .01f);
			lr.SetVertexCount (2);		
			lr.SetWidth (0.03f, 0.03f);		
			lr.material = lineMaterial;
			//lr [0,i].SetColors (Color.gray, Color.gray);
			lr.SetPosition (0, new Vector3 (-4.7f, (i * 1.11f), 4.42f));
			lr.SetPosition (1, new Vector3 (-16.05f, (i * 1.11f), 4.42f));
		}
		linesList.Add (lines);
		goList.Add (go);
		#endregion
	}
	
}
