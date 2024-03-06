using UnityEngine;

namespace Generics
{
    public class GTrajectory : MonoBehaviour
    {
        public Transform origin;
        [SerializeField] private Transform objetivo;

        [SerializeField] private bool seeTrajectory;

        [Range(0.1f, 20f)] public float alturaMaximaFactor = 0.5f; // Factor para ajustar la altura máxima en función de la distancia
        [Range(0.1f, 20f)] public float velocidad = 5f;
        [Range(0.08f, 20f)] public float intervaloTiempo = 0.1f;

        private Vector3[] puntosTrayectoria;

        private void OnDrawGizmos()
        {
            if (objetivo == null)
                return;
            if (origin == null)
                return;
            CalcularPuntosTrayectoria();
            DibujarTrayectoria();

        }

        public Vector3[] CalcularPuntosTrayectoria()
        {
            Vector3 direccion = (objetivo.position - origin.transform.position);
            float distancia = direccion.magnitude;
            float tiempoVuelo = distancia / velocidad;

            // Calcular la altura máxima usando la fórmula del movimiento parabólico
            float alturaMaximaPredicha = origin.transform.position.y + direccion.y;

            // Ajustar la altura máxima si es menor que la altura máxima especificada
            alturaMaximaPredicha = Mathf.Max(alturaMaximaPredicha, alturaMaximaFactor);

            // Calcular la velocidad vertical inicial para llegar a la altura máxima
            float velocidadVerticalInicial = (alturaMaximaPredicha - origin.transform.position.y) / (tiempoVuelo / 2f) + 0.5f * Mathf.Abs(Physics.gravity.y) * tiempoVuelo / 2f;

            // Calcular la velocidad horizontal inicial
            float velocidadHorizontalInicial = distancia / (tiempoVuelo / 2f);

            // Calcular la velocidad inicial total
            Vector3 velocidadInicial = direccion.normalized * velocidadHorizontalInicial;
            velocidadInicial.y = velocidadVerticalInicial;

            // Calcular los puntos de la trayectoria
            int cantidadPuntos = Mathf.CeilToInt(tiempoVuelo / intervaloTiempo) + 1;
            puntosTrayectoria = new Vector3[cantidadPuntos];

            float tiempoActual = 0f;
            for (int i = 0; i < cantidadPuntos; i++)
            {
                puntosTrayectoria[i] = CalcularPosicionEnTiempo(velocidadInicial, tiempoActual);
                tiempoActual += intervaloTiempo;
            }

            return puntosTrayectoria;
        }

        void DibujarTrayectoria()
        {
            if (seeTrajectory)
            {
                foreach (Vector3 punto in puntosTrayectoria)
                {
                    Gizmos.DrawSphere(punto, 0.1f);
                }
            }
        }

        Vector3 CalcularPosicionEnTiempo(Vector3 velocidadInicial, float tiempo)
        {
            // Calcular la posición en el tiempo especificado utilizando la ecuación de movimiento parabólico
            Vector3 posicion = origin.transform.position + velocidadInicial * tiempo +
                               0.5f * Physics.gravity * tiempo * tiempo;

            return posicion;
        }
    }
}