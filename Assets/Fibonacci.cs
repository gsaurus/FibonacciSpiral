using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fibonacci : MonoBehaviour {

	public FibonacciCell cellObj;
	public TrailRenderer trail;
    public float speed = 0.005f;
    public int cellsAhead = 10;

    private List<FibonacciCell> _cells = new List<FibonacciCell>();
	private int _currentCell;
	private float _curveT = -1f;
	private FibonacciCell _selectedCell;
    private int pointCount;

    private float hue = 0.5f;
    private Color previousColor;
    private Color nextColor;

    float n1 = 0,n2 = 1,n3;

	void Start () 
	{
        FibonacciCell firstCell = Instantiate(cellObj) as FibonacciCell;
        float initSize = 2;//1;
        firstCell.cellDirection = CellDirection.up;

        firstCell.SetUp(0, 0, initSize, -initSize);
        _cells.Add(firstCell);
        _selectedCell = firstCell;

        n3 = 2;
        for (int i = 0; i < cellsAhead; ++i)
        {
            NextCell();
        }
        trail.endWidth = 0.33f;
        trail.endColor = Color.white;
    }

	void Update () 
	{
		Vector3 start = Vector3.zero;
		Vector3 end = Vector3.zero;
		Vector3 middle = Vector3.zero;

        if (_curveT < 0) _curveT = 1.0f;
		while(_curveT >= 1.0f)
		{
            NextCell();
            _selectedCell = _cells[_currentCell++];
            _curveT -= 1.0f;
            previousColor = Color.HSVToRGB(hue, 0.8f, 0.75f);
            hue += 0.05f;
            if (hue > 1) hue -= 1;
            nextColor = Color.HSVToRGB(hue, 0.8f, 0.75f);
        }
		SetInterpPoints (out start, out end, out middle);
        trail.gameObject.transform.position = CurveVelocity(Mathf.Pow(_curveT, 1.2f),start,middle, end);

        float distanceToCenter = trail.gameObject.transform.position.magnitude;

        float camSize = Mathf.Lerp(start.magnitude, end.magnitude, _curveT) * 1.25f;
        if (camSize < 5) camSize = 5;
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, camSize, 0.05f);

        //trail.endWidth = trail.startWidth;
        trail.startWidth = Camera.main.orthographicSize / 10f;
        trail.startColor = Color.Lerp(previousColor, nextColor, _curveT);

        float speedMultiplier = (Mathf.Log10(distanceToCenter) + 1) * 0.5f;
        if (speedMultiplier < 1) speedMultiplier = 1;
        _curveT += speed / speedMultiplier;

    }

    float CalculatePower()
    {
        n3 *= 2;
        return n3;
    }

    float CalculateFibonacciNumber()
	{
		n3 = n1 + n2;

		n1 = n2;
		n2 = n3;

		return n3;
	}

    void NextCell()
    {
        int i = _cells.Count - 1;
        int modulos = i % 4;
		FibonacciCell cell = null;
        float size = CalculatePower(); //CalculateFibonacciNumber();
		FibonacciCell lastCell = null;

		float top = 0;
		float left = 0;
		float right = 0;
		float bottom = 0;
        		
		if(_cells.Count > 0)
			lastCell = _cells[i];

		//x = left, y == top
		switch(modulos)
		{				
		case 0:
			//left
			cell = Instantiate(cellObj) as FibonacciCell;
			cell.cellDirection = CellDirection.left;
            				
			top = lastCell.top;
			left = lastCell.left - size;
			right = lastCell.left;
			bottom = lastCell.top - size;            
            cell.SetUp(top,left,right,bottom);
            break;
		case 1:
			//down
			cell = Instantiate(cellObj) as FibonacciCell;
			cell.cellDirection = CellDirection.down;

			top = lastCell.bottom;
			left = lastCell.left;
			right = lastCell.left + size;
			bottom = top - size;
            			
			cell.SetUp(top,left,right,bottom);
                break;
		case 2:
			//right
			cell = Instantiate(cellObj) as FibonacciCell;
			cell.cellDirection = CellDirection.right;

			bottom = lastCell.bottom;
			left = lastCell.right;
			right = left + size;
			top = bottom + size;

			cell.SetUp(top,left,right,bottom);
                break;

		case 3:
			//up
			cell = Instantiate(cellObj) as FibonacciCell;
			cell.cellDirection = CellDirection.up;
            				
			top = lastCell.top + size;
			left = lastCell.right - size;
			right = lastCell.right;
			bottom = lastCell.top;

			cell.SetUp(top,left,right,bottom);
            break;
		}
        if (cell)
        {
            _cells.Add(cell);
        }
	}

	void SetInterpPoints (out Vector3 start, out Vector3 end, out Vector3 middle)
	{
		switch (_selectedCell.cellDirection) {
		case CellDirection.left:
			start = new Vector3 (_selectedCell.right, _selectedCell.top);
			end = new Vector3 (_selectedCell.left, _selectedCell.bottom);
			middle = new Vector3 (_selectedCell.left, _selectedCell.top);
			break;
		case CellDirection.down:
			start = new Vector3 (_selectedCell.left, _selectedCell.top);
			end = new Vector3 (_selectedCell.right, _selectedCell.bottom);
			middle = new Vector3 (_selectedCell.left, _selectedCell.bottom);
			break;
		case CellDirection.right:
			start = new Vector3 (_selectedCell.left, _selectedCell.bottom);
			end = new Vector3 (_selectedCell.right, _selectedCell.top);
			middle = new Vector3 (_selectedCell.right, _selectedCell.bottom);
			break;
		case CellDirection.up:
			start = new Vector3 (_selectedCell.right, _selectedCell.bottom);
			end = new Vector3 (_selectedCell.left, _selectedCell.top);
			middle = new Vector3 (_selectedCell.right, _selectedCell.top);
			break;
		default:
			start = Vector3.zero;
			end = Vector3.zero;
			middle = Vector3.zero;
			break;
		}
	}

	Vector3 CurveVelocity(float t,Vector3 p0, Vector3 p1, Vector3 p2)
	{
		return (1.0f - t) * (1.0f - t) * p0 
			+ 2.0f * (1.0f - t) * t * p1
				+ t * t * p2;
	}
}