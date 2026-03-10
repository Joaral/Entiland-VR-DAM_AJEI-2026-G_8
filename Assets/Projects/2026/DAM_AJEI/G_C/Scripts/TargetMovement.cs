using NUnit;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    [Serializable]
    public class BPoin
    {
        public Vector3 Position = Vector3.zero;
        public Vector3 CurveDirection = Vector3.up;
    }

    public GameObject target;
    public GameObject pivot;

    public List<BPoin> points = new();
    private List<Vector3> becierPoints = new();

    [Header("Spawn Settings")]
    public float timeHidden = 2f;
    public float timeVisible = 4f;

    public bool wasShoot = false;

    [Header("Path Settings")]
    public Transform[] waypoints;      //puntos por donde se mueve
    public float speed = 3f;           //velocidad de movimiento
    public bool loop = true;           //Verificar si vuelve a hacer el recorrido

    private bool lastPoint = false;

    private int currentIndex = 0; //indice del waypoint actual

    private bool isActive = false;

    Vector3 hiddenRotation;
    Vector3 visibleRotation;

    private void Start()
    {
        if (points.Count >= 2)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                //Gizmos.DrawLine(points[i] + transform.position, points[i + 1] + transform.position);

                Vector3 start = points[i].Position;
                Vector3 end = points[i + 1].Position;

                Vector3 control = (start + end) / 2 + points[i].CurveDirection * 2f;

                becierPoints.AddRange(GenerateBezierPoints(start, control, end, 10));
            }
        }

        hiddenRotation = new Vector3(pivot.transform.rotation.x - 90f, pivot.transform.rotation.y, pivot.transform.rotation.z);
        visibleRotation = new Vector3(pivot.transform.rotation.x, pivot.transform.rotation.y, pivot.transform.rotation.z);

        pivot.transform.rotation = Quaternion.Euler(hiddenRotation.x, hiddenRotation.y, hiddenRotation.z);
        StartCoroutine(TargetCycle());
    }

    void Update()
    {
        //Si no hay wayopoints, no hacer nada
        if (becierPoints.Count < 2) return;

        //Ir al siguiente waypoint
        Vector3 targetPoint = becierPoints[currentIndex];
        target.transform.localPosition = Vector3.MoveTowards(
            target.transform.localPosition,
            targetPoint,
            speed * Time.deltaTime
        );

        //Si llego al waypoint, actualizar al siguiente
        if (Vector3.Distance(target.transform.localPosition, targetPoint) < 0.05f)
        {
            if(currentIndex == becierPoints.Count - 1)
            {
                currentIndex = 0;
                becierPoints.Reverse();
            }
            else
            {
                currentIndex++;
            }
            
            targetPoint = becierPoints[currentIndex];
            /*if (currentIndex >= becierPoints.Count - 1)
            {
                lastPoint = true;
            }
            if (currentIndex < becierPoints.Count)
            {
                lastPoint = false;
            }
            if(lastPoint)
            {
                currentIndex--;
            }
            else
            {
                currentIndex++;
            }

            if (currentIndex >= becierPoints.Count)
            {
                if (loop)
                {
                    currentIndex = 0; //restart
                }
                else
                {
                    enabled = false; //dejar de moverse
                }
            }*/
        }
    }

    IEnumerator TargetCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeHidden);

            yield return StartCoroutine(RotateTarget(-90));

            float timer = 0;

            while (timer < timeVisible)
            {
                if (wasShoot)
                {
                    break;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            yield return StartCoroutine(RotateTarget(0));
            wasShoot = false;
        }
    }

    IEnumerator RotateTarget(float targetAngle)
    {
        Quaternion startRot = pivot.transform.localRotation;
        Quaternion endRot = Quaternion.Euler(targetAngle, 0, 0);

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * speed;
            pivot.transform.localRotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Color color = Gizmos.color;
        Gizmos.color = Color.yellow;
        foreach (BPoin point in points)
        {
            Gizmos.DrawSphere(point.Position + transform.position, 0.1f);
        }

        if(points.Count >= 2)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                //Gizmos.DrawLine(points[i] + transform.position, points[i + 1] + transform.position);

                Vector3 start = points[i].Position;
                Vector3 end = points[i + 1].Position;

                Vector3 control = (start + end) / 2 + points[i].CurveDirection * 2f;

                List<Vector3> curvePoints = GenerateBezierPoints(start, control, end, 10);

                for (int x = 0; x < curvePoints.Count - 1; x++)
                {
                    Gizmos.DrawLine(curvePoints[x] + transform.position, curvePoints[x + 1] + transform.position);

                }
            }
        }
        

        Gizmos.color = color;
    }

    List<Vector3> GenerateBezierPoints(Vector3 start, Vector3 control, Vector3 end, int resolution)
    {
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            points.Add(CalculateBezierPoint(t, start, control, end));
        }

        return points;
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;

        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * p0;
        point += 2 * u * t * p1;
        point += tt * p2;

        return point;
    }
}
