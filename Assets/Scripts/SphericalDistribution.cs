using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalDistribution : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float distribution = 1F;
    [SerializeField] private float bias = 0F;

    [SerializeField] private int pointCount;
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private Vector3 direction;

    private int kernelHandle;
    private Vector3[] points;
    private ComputeBuffer computeBuffer;

    private void Awake()
    {
        InitializeCompute();
    }

    private void Update()
    {
        UpdateSphereTargets();
    }

    private void InitializeCompute()
    {
        points = new Vector3[pointCount];
        kernelHandle = computeShader.FindKernel("CSMain");
        computeBuffer = new ComputeBuffer(pointCount, pointCount/10);
        computeShader.SetInt("pointCount", pointCount);
    }

    private void UpdateSphereTargets()
    {
        computeShader.SetVector("direction", Vector3.Normalize(direction));
        computeShader.SetFloat("distribution", distribution);
        computeShader.SetFloat("bias", bias);
        computeBuffer.SetData(points);
        computeShader.SetBuffer(kernelHandle, "Result", computeBuffer);
        computeShader.Dispatch(kernelHandle, pointCount, 1, 1);
        computeBuffer.GetData(points);
    }

    private void OnDestroy()
    {
        computeBuffer.Release();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = new Color(1F, 1F, 0.2F, 0.5F);
            for (int i = 0; i < points.Length; i++)
            {
                Gizmos.DrawSphere(points[i], 0.05F);
            }
        }
    }
}
