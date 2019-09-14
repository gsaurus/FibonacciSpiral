using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fibonacci : MonoBehaviour {

	public FibonacciCell cellObj;
	public GameObject trail;
    public float speed = 0.025f;
    public int cellsAhead = 10;

    private List<FibonacciCell> _cells = new List<FibonacciCell>();
	private int _currentCell;
	private float _curveT;
	private FibonacciCell _selectedCell;
    private int pointCount;

	float n1 = 0,n2 = 1,n3;

	void Start () 
	{
        FibonacciCell firstCell = Instantiate(cellObj) as FibonacciCell;
        float initSize = CalculateFibonacciNumber();
        firstCell.cellDirection = CellDirection.up;

        firstCell.SetUp(0, 0, initSize, -initSize);
        _cells.Add(firstCell);
        _selectedCell = firstCell;

        for (int i = 0; i < cellsAhead; ++i)
        {
            NextCell();
        }
    }

	void Update () 
	{
		Vector3 start = Vector3.zero;
		Vector3 end = Vector3.zero;
		Vector3 middle = Vector3.zero;

		if(_curveT >= 1.0f)
		{
            NextCell();
            _selectedCell = _cells[++_currentCell];
            _curveT = 0;
        }
		SetInterpPoints (out start, out end, out middle);
        trail.transform.position = CurveVelocity(_curveT,start,middle, end);

        float distanceToCenter = trail.transform.position.magnitude;

        float camSize = Mathf.Lerp(start.magnitude, end.magnitude, _curveT) + 2;
        if (camSize < 10) camSize = 10;
        Camera.main.orthographicSize = camSize;

        _curveT += speed;
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
		float size = CalculateFibonacciNumber();
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