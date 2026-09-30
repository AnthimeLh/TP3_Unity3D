using UnityEngine;

// EXERCICE 2 : Caméra orbitale suivant le Player
// EXERCICE 8 (BONUS) : gestion des obstacles entre la caméra et le Player
// A poser sur la caméra principale.
public class OrbitalCamera : MonoBehaviour
{
    [Header("Cible")]
    public Transform target; // Le Player
    public Vector3 targetOffset = new Vector3(0f, 1.6f, 0f); // vise la "tête"

    [Header("Distance / Zoom (molette souris)")]
    public float distance = 5f;
    public float minDistance = 2f;
    public float maxDistance = 10f;
    public float zoomSpeed = 5f;

    [Header("Rotation (souris)")]
    public float rotationSpeedX = 200f;
    public float rotationSpeedY = 150f;

    [Header("Limites verticales (pitch)")]
    public float minPitch = -20f; // évite de regarder sous le sol
    public float maxPitch = 60f;  // évite de regarder au-dessus de la tête

    [Header("Lissage du mouvement")]
    public float smoothTime = 0.05f;

    [Header("EXERCICE 8 (bonus) : anti-obstacle")]
    public bool handleObstacles = true;
    public LayerMask obstacleMask = ~0; // tout, sauf ce que vous excluez au besoin
    public float collisionRadius = 0.3f;
    public float obstacleSmoothSpeed = 10f;

    private float yaw;
    private float pitch = 10f;
    private Vector3 currentVelocity;
    private PlayerMovement playerMovement;
    private float currentDistance;

    void Start()
    {
        if (target != null)
        {
            playerMovement = target.GetComponent<PlayerMovement>();
            yaw = target.eulerAngles.y;
        }
        currentDistance = distance;
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleZoom();
        HandleRotationAndPlayerFacing();

        Vector3 pivot = target.position + targetOffset;
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

        // EXERCICE 8 : on vérifie s'il y a un obstacle entre le pivot (Player) et la caméra
        float desired = handleObstacles ? ResolveObstacles(pivot, rot, distance) : distance;
        currentDistance = Mathf.Lerp(currentDistance, desired, obstacleSmoothSpeed * Time.deltaTime);

        Vector3 finalPos = pivot - rot * Vector3.forward * currentDistance;

        transform.position = Vector3.SmoothDamp(transform.position, finalPos, ref currentVelocity, smoothTime);
        transform.LookAt(pivot);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    void HandleRotationAndPlayerFacing()
    {
        bool rightHeld = Input.GetMouseButton(1);
        bool leftHeld = Input.GetMouseButton(0);

        if (rightHeld || leftHeld)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            yaw += mouseX * rotationSpeedX * Time.deltaTime;
            pitch -= mouseY * rotationSpeedY * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        if (rightHeld)
        {
            // Clic droit : le Player tourne pour faire face à la même direction que la caméra
            // (la caméra reste derrière le dos du Player)
            if (playerMovement != null) playerMovement.AutoRotate = false;
            target.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
        else
        {
            // Clic gauche (ou rien) : seule la caméra bouge, le Player garde son orientation
            if (playerMovement != null) playerMovement.AutoRotate = true;
        }
    }

    // EXERCICE 8 (BONUS) : si un obstacle se trouve entre le Player et la position
    // désirée de la caméra, on rapproche la caméra jusqu'à dégager la vue.
    float ResolveObstacles(Vector3 pivot, Quaternion rot, float desiredDistance)
    {
        Vector3 camDesiredPos = pivot - rot * Vector3.forward * desiredDistance;
        Vector3 dir = (camDesiredPos - pivot).normalized;

        // Le rayon part du Player en direction de la caméra (comme suggéré dans l'énoncé)
        if (Physics.SphereCast(pivot, collisionRadius, dir, out RaycastHit hit, desiredDistance, obstacleMask))
        {
            if (hit.transform != target)
            {
                return Mathf.Clamp(hit.distance - 0.2f, minDistance * 0.3f, desiredDistance);
            }
        }
        return desiredDistance;
    }
}
