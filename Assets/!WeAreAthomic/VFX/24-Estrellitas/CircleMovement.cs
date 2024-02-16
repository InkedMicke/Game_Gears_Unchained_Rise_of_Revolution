using UnityEngine;

public class CircleMovement : MonoBehaviour
{
    public float radius = 5f; // Radio de la circunferencia
    public float rotationSpeed = 50f; // Velocidad de rotaci�n en grados por segundo

    private Vector3 center; // Centro de la circunferencia

    void Start()
    {
        center = transform.position; // Posici�n inicial del objeto
    }

    void Update()
    {
        // Calcular la nueva posici�n en la circunferencia
        float angle = Time.time * rotationSpeed; // �ngulo basado en el tiempo y la velocidad
        Vector3 offset = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * radius; // Desplazamiento en el plano XY
        transform.position = center + offset; // Actualizar la posici�n del objeto
    }
}
