using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] waypoints;      //puntos por donde se mueve
    public float speed = 3f;           //velocidad de movimiento
    public bool loop = true;           //Verificar si vuelve a hacer el recorrido

    private int currentIndex = 0; //indice del waypoint actual

    void Update()
    {
        //Si no hay wayopoints, no hacer nada
        if (waypoints.Length == 0) return;

        //Ir al siguiente waypoint
        Transform targetPoint = waypoints[currentIndex];
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            speed * Time.deltaTime
        );

        //Si llego al waypoint, actualizar al siguiente
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.05f)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Length)
            {
                if (loop)
                {
                    currentIndex = 0; //restart
                }
                else
                {
                    enabled = false; //dejar de moverse
                }
            }
        }
    }
}
