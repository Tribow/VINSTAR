using UnityEngine;
using System;

[Serializable]
public static class DrawThis
{
    public static void Polygon(GameObject container, int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth)
    {
        LineRenderer lineRenderer;
        if (container.GetComponent<LineRenderer>() == null)
            lineRenderer = container.AddComponent<LineRenderer>();
        else
            lineRenderer = container.GetComponent<LineRenderer>();


        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.loop = true;
        float angle = 2 * Mathf.PI / vertexNumber;
        lineRenderer.positionCount = vertexNumber;

        for (int i = 0; i < vertexNumber; i++)
        {
            Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
                                                     new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
                                       new Vector4(0, 0, 1, 0),
                                       new Vector4(0, 0, 0, 1));
            Vector3 initialRelativePosition = new Vector3(0, radius, 0);
            lineRenderer.SetPosition(i, centerPos + rotationMatrix.MultiplyPoint(initialRelativePosition));

        }
    }
}
