using UnityEngine;
using UnityEngine.UI;

// A poser sur le GameObject parent de la barre (celui qui contient les 2 Images :
// fond + remplissage). Génère le sprite "pilule" une seule fois et l'applique aux deux.
public class HealthBarStyler : MonoBehaviour
{
    public Image backgroundImage;
    public Image fillImage;

    [Header("Couleurs (comme la maquette : fond rose clair, remplissage rouge)")]
    public Color backgroundColor = new Color(1f, 0.65f, 0.65f);
    public Color fillColor = new Color(0.82f, 0.03f, 0.03f);

    void Awake()
    {
        Sprite pill = PillSpriteFactory.CreatePill(256, 64);

        if (backgroundImage != null)
        {
            backgroundImage.sprite = pill;
            backgroundImage.type = Image.Type.Sliced; // garde les bouts ronds quelle que soit la largeur
            backgroundImage.color = backgroundColor;
        }

        if (fillImage != null)
        {
            fillImage.sprite = pill;
            fillImage.type = Image.Type.Filled;       // nécessaire pour l'effet "barre qui se vide"
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            fillImage.color = fillColor;
        }
    }
}
