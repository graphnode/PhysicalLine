using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class LineDrawer : MonoBehaviour
{
    public LineRenderer Line;

    private List<Vector3> _positions = new List<Vector3>();

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

        var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;

        _positions = new List<Vector3> {position, position};

        Line.positionCount = _positions.Count;
        Line.SetPositions(_positions.ToArray());
    }

    private void MoveLine()
    {
        if (_positions.Count > 2)
        {
            var currentSize = _positions.Count;
            for (var i = 0; i < _positions.Count - 2; i++)
            {
                if (Physics2D.Linecast(_positions[i], _positions[i + 2]))
                    continue;

                _positions.RemoveAt(i + 1);
                i--;
            }

            if (currentSize != _positions.Count)
            {
                Line.SetPositions(_positions.ToArray());
                Line.positionCount = _positions.Count;
            }
        }

        var oldPosition = _positions[_positions.Count - 2];

        var newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0;

        var hit = Physics2D.Linecast(oldPosition, newPosition);

        if (hit)
        {
            Vector2 p1, p2;
            if (FindTangents(hit.transform.position, 0.90f, oldPosition, out p1, out p2))
            {
                var hitPosition = Vector2.Distance(hit.point, p1) < Vector2.Distance(hit.point, p2) ? p1 : p2;
                //Debug.DrawLine(oldPosition, hitPosition, Color.red, 5f, false);

                _positions[_positions.Count - 1] = hitPosition;
                _positions.Add(newPosition);
                Line.positionCount = _positions.Count;
            }
            else if (FindTangents(hit.transform.position, 0.90f, newPosition, out p1, out p2))
            {
                var hitPosition = Vector2.Distance(hit.point, p1) < Vector2.Distance(hit.point, p2) ? p1 : p2;
                //Debug.DrawLine(newPosition, hitPosition, Color.blue);

                _positions[_positions.Count - 1] = hitPosition;
                _positions.Add(newPosition);
                Line.positionCount = _positions.Count;
            }
        }
        else
        {
            _positions[_positions.Count - 1] = newPosition;
        }

        Line.SetPositions(_positions.ToArray());
    }

    private void TerminateLine()
    {
        _positions.Clear();
        Line.positionCount = _positions.Count;

        Line.enabled = false;
    }

    private bool FindTangents(Vector2 center, float radius, Vector2 externalPoint, out Vector2 pt1, out Vector2 pt2)
    {
        // Find the distance squared from the
        // external point to the circle's center.
        double dx = center.x - externalPoint.x;
        double dy = center.y - externalPoint.y;
        var dSquared = dx * dx + dy * dy;

        if (dSquared < radius * radius)
        {
            pt1 = pt2 = new Vector2(float.NaN, float.NaN);
            return false;
        }

        // Find the distance from the external point
        // to the tangent points.
        var l = Math.Sqrt(dSquared - radius * radius);

        // Find the points of intersection between
        // the original circle and the circle with
        // center external_point and radius dist.
        FindCircleCircleIntersections(center.x, center.y, radius, externalPoint.x, externalPoint.y, (float)l, out pt1, out pt2);

        return true;
    }

    private int FindCircleCircleIntersections(float cx0, float cy0, float radius0, float cx1, float cy1, float radius1, out Vector2 intersection1, out Vector2 intersection2)
    {
        // Find the distance between the centers.
        var dx = cx0 - cx1;
        var dy = cy0 - cy1;
        var dist = Math.Sqrt(dx * dx + dy * dy);

        // See how many solutions there are.
        if (dist > radius0 + radius1)
        {
            // No solutions, the circles are too far apart.
            intersection1 = intersection2 = new Vector2(float.NaN, float.NaN);
            return 0;
        }

        if (dist < Math.Abs(radius0 - radius1))
        {
            // No solutions, one circle contains the other.
            intersection1 = intersection2 = new Vector2(float.NaN, float.NaN);
            return 0;
        }

        if (dist == 0 && radius0 == radius1)
        {
            // No solutions, the circles coincide.
            intersection1 = intersection2 = new Vector2(float.NaN, float.NaN);
            return 0;
        }
        
        // Find a and h.
        var a = (radius0 * radius0 - radius1 * radius1 + dist * dist) / (2 * dist);
        var h = Math.Sqrt(radius0 * radius0 - a * a);

        // Find P2.
        var cx2 = cx0 + a * (cx1 - cx0) / dist;
        var cy2 = cy0 + a * (cy1 - cy0) / dist;

        // Get the points P3.
        intersection1 = new Vector2((float)(cx2 + h * (cy1 - cy0) / dist), (float)(cy2 - h * (cx1 - cx0) / dist));
        intersection2 = new Vector2((float)(cx2 - h * (cy1 - cy0) / dist), (float)(cy2 + h * (cx1 - cx0) / dist));

        // See if we have 1 or 2 solutions.
        return dist == radius0 + radius1 ? 1 : 2;
    }
}
