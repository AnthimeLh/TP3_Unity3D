using UnityEngine;
using System.Collections.Generic;

// EXERCICE 7 : Trajectoire de caméra basée sur une courbe de Bézier
// A poser sur une caméra dédiée (différente de la caméra orbitale du joueur),
// utilisée pour l'enregistrement du vol à travers la forêt.
public class BezierCameraFlight : MonoBehaviour
{
    [Header("Points de contrôle du trajet (2 à n points, dans l'ordre du parcours)")]
    public List<Transform> pathPoints = new List<Transform>();

    [Header("Vitesse")]
    [Tooltip("Valeur de progression 't' ajoutée par seconde (0.02 = ~50 secondes pour tout le trajet)")]
    public float speed = 0.03f;
    public bool loop = false;
    public bool lookAlongPath = true;

    private float t = 0f;

    void Update()
    {
        if (pathPoints.Count < 2) return;

        t += speed * Time.deltaTime;
        t = loop ? t % 1f : Mathf.Clamp01(t);

        Vector3 currentPos = GetPoint(t);
        transform.position = currentPos;

        if (lookAlongPath)
        {
            float lookAheadT = Mathf.Clamp01(t + 0.01f);
            Vector3 nextPos = GetPoint(lookAheadT);
            Vector3 dir = nextPos - currentPos;
            if (dir.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        }
    }

    Vector3 GetPoint(float u)
    {
        List<Vector3> points = new List<Vector3>(pathPoints.Count);
        foreach (var p in pathPoints) points.Add(p.position);
        return DeCasteljau(points, u);
    }

    // Même algorithme récursif que dans RecursiveBezier.cs (Exercice 9),
    // réutilisé ici pour animer la caméra le long de la trajectoire.
    Vector3 DeCasteljau(List<Vector3> points, float u)
    {
        if (points.Count == 1) return points[0];
        List<Vector3> next = new List<Vector3>(points.Count - 1);
        for (int i = 0; i < points.Count - 1; i++)
            next.Add(Vector3.Lerp(points[i], points[i + 1], u));
        return DeCasteljau(next, u);
    }
}
