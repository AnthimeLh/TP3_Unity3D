using UnityEngine;
using System.Collections.Generic;

// EXERCICE 9 (BONUS) : Algorithme de Bézier récursif pour n points de contrôle (n >= 2)
// A poser sur un GameObject vide contenant un composant LineRenderer.
[RequireComponent(typeof(LineRenderer))]
public class RecursiveBezier : MonoBehaviour
{
    [Header("Points de contrôle (n >= 2, ordre important)")]
    public List<Transform> controlPoints = new List<Transform>();

    [Header("Rendu")]
    [Range(2, 300)] public int resolution = 100;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        DrawCurve();
    }

    void DrawCurve()
    {
        if (controlPoints.Count < 2) return;

        lineRenderer.positionCount = resolution;
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            lineRenderer.SetPosition(i, GetPoint(t));
        }
    }

    public Vector3 GetPoint(float t)
    {
        List<Vector3> points = new List<Vector3>(controlPoints.Count);
        foreach (var cp in controlPoints)
            points.Add(cp.position);

        return DeCasteljau(points, t);
    }

    // Algorithme RÉCURSIF de De Casteljau :
    // - Cas de base : il ne reste qu'un seul point -> c'est le point cherché sur la courbe.
    // - Cas général : on construit la liste des points intermédiaires (interpolation linéaire
    //   entre chaque paire de points consécutifs), puis on rappelle la fonction sur cette
    //   nouvelle liste, plus courte d'un élément. Fonctionne pour n'importe quel nombre
    //   de points de contrôle (quadratique, cubique, quintique, etc.)
    private Vector3 DeCasteljau(List<Vector3> points, float t)
    {
        if (points.Count == 1)
            return points[0];

        List<Vector3> next = new List<Vector3>(points.Count - 1);
        for (int i = 0; i < points.Count - 1; i++)
        {
            next.Add(Vector3.Lerp(points[i], points[i + 1], t));
        }

        return DeCasteljau(next, t); // appel récursif
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        foreach (var cp in controlPoints)
        {
            if (cp != null) Gizmos.DrawSphere(cp.position, 0.15f);
        }
    }
}
