using System;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class LineDrawer : MonoBehaviour
{
    public LineRenderer Line;

    private Vector3[] _positions = new Vector3[0];

    public void Start()
    {
        Line.enabled = false;
        Line.positionCount = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
        {
            InitializeLine();
        }
        else if (Input.GetMouseButton((int)MouseButton.LeftMouse))
        {
            MoveLine();
        }
        else if (Input.GetMouseButtonUp((int)MouseButton.LeftMouse))
        {
            TerminateLine();
        }
    }


    private void InitializeLine()
    {
        Line.enabled = true;

        _positions = new Vector3[2];

        var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;

        _positions[0] = _positions[1] = position;

        Line.positionCount = 2;
        Line.SetPositions(_positions);
    }

    private void MoveLine()
    {

        var oldPosition = _positions[_positions.Length - 2];

        var newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0;

        var hit = Physics2D.Linecast(oldPosition, newPosition);

        if (hit)
        {
            Line.positionCount += 1;
            Array.Resize(ref _positions, Line.positionCount);
            _positions[_positions.Length - 2] = hit.collider.transform.position;
            _positions[_positions.Length - 1] = newPosition;
        }
        else
        {
            _positions[_positions.Length - 1] = newPosition;
        }

        Line.SetPositions(_positions);
    }

    private void TerminateLine()
    {
        _positions = new Vector3[0];
        Line.positionCount = 0;

        Line.enabled = false;
    }
}
