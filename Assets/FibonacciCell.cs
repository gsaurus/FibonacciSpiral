using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum CellDirection {left, right, up ,down};

public class FibonacciCell : MonoBehaviour {

	public float top, bottom, left, right;
	public CellDirection cellDirection;
    private LineRenderer _line;
    private TextMeshPro text;

    //change size to tlrb
    public void SetUp(float top, float left, float right , float bottom)
	{
        _line = GetComponent<LineRenderer>();
        text = GetComponentInChildren<TextMeshPro>();
        GetComponent<Canvas>().worldCamera = Camera.main;
        _line.positionCount = 4;

        this.top = top;
		this.bottom = bottom;
		this.left = left;
		this.right = right;

        _line.SetPositions(new Vector3[]{
            new Vector3(left, bottom),
            new Vector3(right, bottom),
            new Vector3(right, top),
            new Vector3(left, top)
        });
        text.transform.position = new Vector3(left, bottom);//new Vector3(left + (right - left) * 0.5f, bottom + (top - bottom) * 0.5f);
        text.text = "" + (right - left);
        //text.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(right - left, top - bottom);
        //text.fontSize = (right - left);
        GetComponentInChildren<TextContainer>().width = right - left;
        GetComponentInChildren<TextContainer>().height = top - bottom;
        //text.fontSize = (int)((right - left) * 0.5f);
        //text.transform.position = new Vector3(left + (right - left) * 0.5f, bottom + (top - bottom) * 0.5f);
    }

    public void Update()
    {
        _line.startWidth = Camera.main.orthographicSize / 50f;
        _line.endWidth = _line.startWidth;
    }
}
