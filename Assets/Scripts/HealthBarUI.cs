using UnityEngine;
using UnityEngine.UI;

// EXERCICE 5 : Barre de vie
// A poser sur le Canvas (World Space) placé au-dessus du personnage (Player ou Monstre).
public class HealthBarUI : MonoBehaviour
{
    [Tooltip("Référence au composant Health du personnage suivi")]
    public Health health;

    [Tooltip("Image de type Image Type = Filled, Fill Method = Horizontal")]
    public Image fillImage;

    [Tooltip("Décalage au-dessus de la tête du personnage")]
    public Vector3 worldOffset = new Vector3(0f, 2.2f, 0f);

    private Transform target;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        if (health != null)
        {
            target = health.transform;
            health.OnHealthChanged += UpdateBar;
            UpdateBar(health.currentHealth, health.maxHealth);
        }
    }

    void LateUpdate()
    {
        if (target == null || cam == null) return;

        // Le Canvas World Space suit la position du personnage
        transform.position = target.position + worldOffset;

        // Effet "billboard" : la barre fait toujours face à la caméra
        transform.rotation = cam.transform.rotation;
    }

    void UpdateBar(int current, int max)
    {
        if (fillImage != null)
            fillImage.fillAmount = max > 0 ? (float)current / max : 0f;
    }

    void OnDestroy()
    {
        if (health != null) health.OnHealthChanged -= UpdateBar;
    }
}
