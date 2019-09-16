using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fibonacci : MonoBehaviour {

	public FibonacciCell cellObj;
	public TrailRenderer trail;
    public float speed = 0.005f;
    public int cellsAhead = 10;
    public static float ScaleFactor = 0.01f;

    private List<FibonacciCell> _cells = new List<FibonacciCell>();
	private int _currentCell = -1;
	private float _curveT = -1f;
	private FibonacciCell _selectedCell;
    private int pointCount;

    private float hue = 0.5f;
    private Color previousColor;
    private Color nextColor;

	private float[][] highlightTimes = {
		new float[]{1.15f, 1.88f},	// 2
		new float[]{1.88f, 2.58f},	// 4
		new float[]{2.58f, 3.30f},	// 8
		new float[]{3.30f, 3.30f, 4.0f}, // 16
		new float[]{4.00f, 4.00f, 6.9f}, // 32
		new float[]{6.90f, 7.28f, 7.61f}, // 64
		new float[]{7.61f, 8.66f, 8.97f, 9.30f},		// 128
		new float[]{9.30f, 9.58f, 9.58f, 10.16f},		// 256
		new float[]{10.16f, 10.53f, 11.11f, 11.41f},  	// 512
		new float[]{11.41f, 11.72f, 12.31f, 12.31f, 14.05f},  	// 1024
		new float[]{14.05f, 14.36f, 14.69f, 14.98f, 15.27f},  	// 2048
		new float[]{15.27f, 15.59f, 16.14f, 16.14f, 16.73f},  	// 4096
		new float[]{16.73f, 17.02f, 17.71f, 17.71f, 19.35f},  	// 8192
		new float[]{19.35f, 19.66f, 19.95f, 20.24f, 20.55f, 20.85f},  	// 16384
		new float[]{20.85f, 21.39f, 21.70f, 22.27f, 22.57f, 22.80f},  	// 32768
		new float[]{22.80f, 22.80f, 23.12f, 23.41f, 23.69f, 23.99f},  	// 65536
		new float[]{23.99f, 24.54f, 24.80f, 25.13f, 25.68f, 25.68f, 27.62f},  	// 131072
		new float[]{27.62f, 27.90f, 28.19f, 28.46f, 28.74f, 29.05f, 29.63f},  	// 262144
		new float[]{29.63f, 29.86f, 30.18f, 30.18f, 30.46f, 30.79f, 33.43f},  	// 524288
		new float[]{33.43f, 33.43f, 33.43f, 33.43f, 34.22f, 34.92f, 34.92f, 37.06f},  	// 1048576
		new float[]{37.06f, 37.40f, 37.80f, 37.80f, 38.17f, 38.51f, 40.12f, 40.42f},  	// 2097152
		new float[]{40.42f, 40.79f, 41.43f, 41.43f, 42.12f, 42.83f, 42.83f, 43.93f},  	// 4194304
		new float[]{43.93f, 44.24f, 44.24f, 44.56f, 44.94f, 44.94f, 44.94f, 46.91f},  	// 8388608
		new float[]{46.91f, 47.56f, 47.56f, 48.22f, 48.91f, 49.59f, 50.30f, 50.90f, 51.62f},  	// 16777220
		new float[]{51.62f, 52.29f, 52.89f, 53.58f, 54.27f, 54.95f, 55.62f, 55.62f, 57.29f},  	// 33554430
		new float[]{57.29f, 57.99f, 58.64f, 58.64f, 59.86f, 60.87f, 60.87f, 60.87f, 63.00f},  	// 67108860
	};

    float n1 = 0,n2 = 1,n3;

	void Start () 
	{
        FibonacciCell firstCell = Instantiate(cellObj) as FibonacciCell;
        float initSize = 2;//1;
        firstCell.cellDirection = CellDirection.up;

        firstCell.SetUp(0, 0, initSize, -initSize, highlightTimes[0]);
        _cells.Add(firstCell);
        _selectedCell = firstCell;

        n3 = 2;
        for (int i = 0; i < cellsAhead; ++i)
        {
            NextCell();
        }
        trail.endWidth = 0.33f * ScaleFactor;
        trail.endColor = Color.white;
        trail.minVertexDistance *= ScaleFactor;
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
            _selectedCell = _cells[++_currentCell];
            _curveT -= 1.0f;
            previousColor = Color.HSVToRGB(hue, 0.8f, 0.75f);
            hue += 0.05f;
            if (hue > 1) hue -= 1;
            nextColor = Color.HSVToRGB(hue, 0.8f, 0.75f);
        }
		SetInterpPoints (out start, out end, out middle);
        trail.gameObject.transform.localPosition = CurveVelocity(Mathf.Pow(_curveT, 1.2f),start,middle, end);

        float distanceToCenter = trail.gameObject.transform.localPosition.magnitude;
        trail.gameObject.transform.localPosition *= ScaleFactor;

        float camSize = Mathf.Lerp(start.magnitude, end.magnitude, _curveT) * 1.5f; //1.25f;
        if (camSize < 8) camSize = 8;
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, camSize * ScaleFactor, 0.05f);
        //Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, middle * ScaleFactor, 0.05f);

        //trail.endWidth = trail.startWidth;
        trail.startWidth = Camera.main.orthographicSize / 10f;
        trail.startColor = Color.Lerp(previousColor, nextColor, _curveT);

        float speedMultiplier = (Mathf.Log10(distanceToCenter) + 1) * 0.5f;
        if (speedMultiplier < 1) speedMultiplier = 1;
		if (_currentCell < highlightTimes.Length)
		{
			float previousTime = _currentCell > 0 ? highlightTimes[_currentCell-1][highlightTimes[_currentCell-1].Length - 1] : 0;
			_curveT = (Time.timeSinceLevelLoad - previousTime) / (highlightTimes[_currentCell][highlightTimes[_currentCell].Length - 1] - previousTime);
			if (_curveT < 0) _curveT = 0;
		}
		else
		{
        	_curveT += speed / speedMultiplier;
		}

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
        int lastCellIndex = _cells.Count - 1;
        int modulos = lastCellIndex % 4;
		FibonacciCell cell = null;
        float size = CalculatePower(); //CalculateFibonacciNumber();
		FibonacciCell lastCell = null;

		float top = 0;
		float left = 0;
		float right = 0;
		float bottom = 0;
        		
		if(_cells.Count > 0)
			lastCell = _cells[lastCellIndex];

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
            break;
		case 1:
			//down
			cell = Instantiate(cellObj) as FibonacciCell;
			cell.cellDirection = CellDirection.down;

			top = lastCell.bottom;
			left = lastCell.left;
			right = lastCell.left + size;
			bottom = top - size;
			break;
		case 2:
			//right
			cell = Instantiate(cellObj) as FibonacciCell;
			cell.cellDirection = CellDirection.right;

			bottom = lastCell.bottom;
			left = lastCell.right;
			right = left + size;
			top = bottom + size;
			break;

		case 3:
			//up
			cell = Instantiate(cellObj) as FibonacciCell;
			cell.cellDirection = CellDirection.up;
            				
			top = lastCell.top + size;
			left = lastCell.right - size;
			right = lastCell.right;
			bottom = lastCell.top;
            break;
		}
        if (cell)
        {
			if (_cells.Count < highlightTimes.Length)
			{
				cell.SetUp(top,left,right,bottom, highlightTimes[_cells.Count]);
			}else
			{
				cell.SetUp(top,left,right,bottom);
			}
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