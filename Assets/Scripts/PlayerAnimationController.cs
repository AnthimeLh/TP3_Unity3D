using UnityEngine;

// EXERCICE 4 : Animations 3D
// A poser sur le personnage 3D téléchargé (celui qui remplace la capsule).
// Ce script suppose que le personnage a déjà :
//  - un composant Animator avec un Animator Controller assigné
//  - des paramètres nommés "Speed" (Float), "Jump" (Trigger), "IsGrounded" (Bool)
//    créés dans la fenêtre Window > Animation > Animator
[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    private const string PARAM_SPEED = "Speed";
    private const string PARAM_JUMP = "Jump";
    private const string PARAM_GROUNDED = "IsGrounded";
    private const string PARAM_ATTACK = "Attack";

    public float speedSmoothTime = 0.1f;

    private Animator animator;
    private CharacterController controller; // peut être null si vous utilisez un Rigidbody
    private float currentSpeed;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float targetSpeed = Mathf.Clamp01(new Vector2(h, v).magnitude); // 0 à 1

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / speedSmoothTime);
        animator.SetFloat(PARAM_SPEED, currentSpeed);

        bool grounded = controller != null ? controller.isGrounded : true;
        animator.SetBool(PARAM_GROUNDED, grounded);

        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            animator.SetTrigger(PARAM_JUMP);
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger(PARAM_ATTACK);
        }
    }
}