using UnityEngine;

// EXERCICE 6 : Courbes de Bézier quadratique (3 points) et cubique (4 points)
// A poser sur un GameObject vide contenant un composant LineRenderer.
[RequireComponent(typeof(LineRenderer))]
public class BezierCurve : MonoBehaviour
{
    public enum CurveType { Quadratic, Cubic }

    [Header("Type de courbe")]
    public CurveType curveType = CurveType.Cubic;

    [Header("Points de contrôle (glissez des Transforms de la scène ici)")]
    public Transform p0;
    public Transform p1;
    public Transform p2;
    public Transform p3; // utilisé uniquement en mode Cubic

    [Header("Rendu")]
    [Range(2, 200)] public int resolution = 50;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // Recalcule la courbe à chaque frame : permet de déplacer les points de contrôle
        // dans la scène (à la souris, ou via un script) et de voir la courbe se mettre à jour en direct.
        DrawCurve();
    }

    void DrawCurve()
    {
        if (curveType == CurveType.Quadratic && (p0 == null || p1 == null || p2 == null)) return;
        if (curveType == CurveType.Cubic && (p0 == null || p1 == null || p2 == null || p3 == null)) return;

        lineRenderer.positionCount = resolution;
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            Vector3 point = curveType == CurveType.Quadratic ? GetQuadraticPoint(t) : GetCubicPoint(t);
            lineRenderer.SetPosition(i, point);
        }
    }

    // Courbe de Bézier QUADRATIQUE : B(t) = lerp(lerp(P0,P1,t), lerp(P1,P2,t), t)
    public Vector3 GetQuadraticPoint(float t)
    {
        Vector3 a = p0.position, b = p1.position, c = p2.position;
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, t);
    }

    // Courbe de Bézier CUBIQUE : interpolations successives entre 4 points de contrôle
    public Vector3 GetCubicPoint(float t)
    {
        Vector3 a = p0.position, b = p1.position, c = p2.position, d = p3.position;
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        Vector3 cd = Vector3.Lerp(c, d, t);
        Vector3 abc = Vector3.Lerp(ab, bc, t);
        Vector3 bcd = Vector3.Lerp(bc, cd, t);
        return Vector3.Lerp(abc, bcd, t);
    }

    // Aide visuelle dans l'éditeur pour repérer les points de contrôle
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (p0) Gizmos.DrawSphere(p0.position, 0.15f);
        if (p1) Gizmos.DrawSphere(p1.position, 0.15f);
        if (p2) Gizmos.DrawSphere(p2.position, 0.15f);
        if (p3 && curveType == CurveType.Cubic) Gizmos.DrawSphere(p3.position, 0.15f);
    }
}
