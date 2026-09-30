using UnityEngine;

// EXERCICE 1 : Mouvement du Player
// A poser sur la capsule "Player" (qui doit avoir un composant CharacterController)
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Vitesse de déplacement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 10f;

    [Header("Saut")]
    public float jumpForce = 8f;
    public float gravity = -20f;

    [Header("Référence caméra (déplacement relatif à la vue)")]
    public Transform cameraTransform;

    // Contrôlé par le script de caméra orbitale (Exercice 2) :
    // quand on tient le clic droit, la caméra prend le contrôle de la rotation.
    [HideInInspector] public bool AutoRotate = true;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f; // évite que le personnage "flotte" légèrement au sol

        // W/S -> axe "Vertical", A/D -> axe "Horizontal" (configurés par défaut dans Unity Input Manager)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Le déplacement est relatif à la direction de la caméra, pas aux axes du monde,
        // pour que "avancer" avance toujours "vers l'écran"
        Vector3 camForward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
        Vector3 camRight   = cameraTransform != null ? cameraTransform.right   : Vector3.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * v + camRight * h;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        // Le Player s'oriente automatiquement vers la direction de déplacement,
        // SAUF si la caméra orbitale a pris le contrôle de sa rotation (clic droit, cf Exercice 2)
        if (AutoRotate && moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Saut : uniquement si le Player touche le sol
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = jumpForce;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public bool IsGrounded => isGrounded;
}
