using UnityEngine;
using System.Collections;

public enum CellDirection {left, right, up ,down};

public class FibonacciCell : MonoBehaviour {

	public float top, bottom, left, right;
	public CellDirection cellDirection;

	//change size to tlrb
	public void SetUp(float top, float left, float right , float bottom)
	{
		this.top = top;
		this.bottom = bottom;
		this.left = left;
		this.right = right;
		
		//top line
		Debug.DrawLine(new Vector3 (left, top, 0),
		               new Vector3 (right, top, 0),
		               Color.white,float.MaxValue);
		
		//right line
		Debug.DrawLine(new Vector3 (right, top, 0),
		               new Vector3 (right, bottom, 0),
		               Color.white,float.MaxValue);
		//bottom line
		Debug.DrawLine(new Vector3 (right, bottom, 0),
		               new Vector3 (left, bottom, 0),
		               Color.white,float.MaxValue);
		//left line
		Debug.DrawLine(new Vector3 (left, bottom, 0),
		               new Vector3 (left, top, 0),
		               Color.white,float.MaxValue);
	}
}
