using System.Collections;
using UnityEngine;

public class G_MeshTrail : MonoBehaviour
{
    [SerializeField] private float activeTime = 2f;

    [Header("Mesh Related")]
    [SerializeField] private float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 3f;
    [SerializeField] private Transform positionToSpawn;

    [Header("Shader Related")]
    public Material mat;
    public string shaderVarRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    private bool isTrailActive;
    [SerializeField] private SkinnedMeshRenderer[] skinnedMeshRenderers;

    public void StartTrail()
    {
        if(!isTrailActive)
        {
            isTrailActive = true;
            ActivateTrail(activeTime);
        }
    }

    private void ActivateTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if (skinnedMeshRenderers == null)
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                // Asigna la malla al componente MeshFilter
                mf.mesh = mesh;

                // Asigna el nuevo material a todos los elementos de la matriz de materiales
                Material[] newMaterials = new Material[skinnedMeshRenderers[i].materials.Length];
                for (int j = 0; j < newMaterials.Length; j++)
                {
                    newMaterials[j] = mat;
                }
                mr.materials = newMaterials;

                // Inicia la corrutina para animar la variable del shader en el material
                StartCoroutine(AnimateMaterialFloat(mr.materials, 0, shaderVarRate, shaderVarRefreshRate));

                // Destruye el objeto creado después de un cierto tiempo
                Destroy(gObj, meshDestroyDelay);
            }

            // Espera un cierto tiempo antes de la próxima iteración
            //yield return new WaitForSeconds(meshRefreshRate);
        }
        // Marca que el rastro ya no está activo
        isTrailActive = false;
    }

    IEnumerator AnimateMaterialFloat(Material[] materials, float goal, float rate, float refreshRate)
    {
        foreach (Material mat in materials)
        {
            // Obtiene el valor actual de la variable del shader en el material
            float valueToAnimate = mat.GetFloat(shaderVarRef);

            // Mientras el valor a animar sea mayor que el objetivo
            while (valueToAnimate > goal)
            {
                // Reduce el valor a animar por la tasa de animación
                valueToAnimate -= rate;
                // Actualiza el valor de la variable del shader en el material
                mat.SetFloat(shaderVarRef, valueToAnimate);

                // Espera un cierto tiempo antes de la próxima iteración
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}
