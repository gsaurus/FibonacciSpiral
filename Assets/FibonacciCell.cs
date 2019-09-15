using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public enum CellDirection {left, right, up ,down};

public class FibonacciCell : MonoBehaviour {

	public float top, bottom, left, right;
	public CellDirection cellDirection;
    private LineRenderer _line;
    private TextMeshPro text;

    static float hue = 0.5f;

    //change size to tlrb
    public void SetUp(float top, float left, float right , float bottom)
	{
        _line = GetComponent<LineRenderer>();
        text = GetComponentInChildren<TextMeshPro>();
        //GetComponent<Canvas>().worldCamera = Camera.main;
        _line.positionCount = 5;

        this.top = top;
		this.bottom = bottom;
		this.left = left;
		this.right = right;
        Vector3[] vertices = new Vector3[]{
            new Vector3(left, bottom),
            new Vector3(right, bottom),
            new Vector3(right, top),
            new Vector3(left, top),
            new Vector3(left, bottom)
        };
        _line.SetPositions(vertices);


        vertices = new Vector3[]{
            new Vector3(right, top, 0.1f),
            new Vector3(right, bottom,  0.1f),
            new Vector3(left, top,  0.1f),
            new Vector3(left, bottom,  0.1f)
        };

        Vector2[] uv = new Vector2[]
         {
             new Vector2(1, 1),
             new Vector2(1, 0),
             new Vector2(0, 1),
             new Vector2(0, 0),
         };

        int[] triangles = new int[]
        {
             0, 1, 2,
             2, 1, 3,
        };

        GetComponent<MeshFilter>().mesh.uv = uv;
        GetComponent<MeshFilter>().mesh.triangles = triangles;
        GetComponent<MeshFilter>().mesh.vertices = vertices;
        GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(hue, 0.2f, 1.61f);
        hue += 0.05f;
        if (hue > 1) hue -= 1;

        text.transform.position = new Vector3(left + (right - left) * 0.5f, bottom + (top - bottom) * 0.5f);
        text.text = (right - left).ToString("F0");
        text.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2((right - left) * 0.65f, (top - bottom) * 0.7f);
    }

    public void Update()
    {
        _line.startWidth = Camera.main.orthographicSize / 50f;
        _line.endWidth = _line.startWidth;
    }

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        text.enableAutoSizing = false;
    }
}
