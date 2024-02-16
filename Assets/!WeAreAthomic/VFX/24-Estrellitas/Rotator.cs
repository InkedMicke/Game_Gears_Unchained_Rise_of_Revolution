using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 50f; // Velocidad de rotación en grados por segundo

    void Update()
    {
        // Rotar el objeto en el eje Z (up) usando la velocidad especificada
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
