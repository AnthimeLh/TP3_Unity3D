using UnityEngine;

// EXERCICE 3 : Cycle jour/nuit
// A poser sur le GameObject contenant le composant "Directional Light"
[RequireComponent(typeof(Light))]
public class DayNightCycle : MonoBehaviour
{
    [Header("Durée du cycle")]
    [Tooltip("Durée en secondes RÉELLES d'un cycle complet de 24h virtuelles. Ex: 120 = 2 minutes.")]
    public float dayDurationInSeconds = 120f;

    [Range(0f, 24f)] public float startHour = 6f;

    [Header("Intensité de la lumière")]
    public float maxIntensity = 1.2f;
    public float minIntensity = 0.02f;

    [Header("Couleur au fil de la journée (optionnel)")]
    public Gradient lightColor;

    [Header("Accélération du temps")]
    [Tooltip("Multiplicateur additionnel, modifiable en jeu avec les touches +/-")]
    public float timeMultiplier = 1f;

    private Light directionalLight;
    private float currentHour;

    void Start()
    {
        directionalLight = GetComponent<Light>();
        currentHour = startHour;
        InitDefaultGradientIfNeeded();
    }

    void Update()
    {
        // Vitesse d'écoulement du temps : 24h virtuelles réparties sur "dayDurationInSeconds" secondes réelles
        float hoursPerSecond = 24f / dayDurationInSeconds;
        currentHour += hoursPerSecond * timeMultiplier * Time.deltaTime;
        currentHour %= 24f;

        UpdateSunRotation();
        UpdateLightIntensityAndColor();
        HandleAccelerationInput();
    }

    void UpdateSunRotation()
    {
        // 0h -> soleil sous l'horizon (-90°), 12h -> zénith (90°)
        float sunAngle = (currentHour / 24f) * 360f - 90f;
        transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);
    }

    void UpdateLightIntensityAndColor()
    {
        float t = currentHour / 24f;
        // dayFactor = 1 à midi, 0 à minuit (courbe sinusoïdale)
        float dayFactor = Mathf.Clamp01(Mathf.Sin(t * Mathf.PI * 2f - Mathf.PI / 2f) * 0.5f + 0.5f);

        directionalLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, dayFactor);
        directionalLight.color = lightColor.Evaluate(t);
    }

    void HandleAccelerationInput()
    {
        // Touches pour accélérer/ralentir le temps et bien observer la transition
        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Equals))
            timeMultiplier *= 2f;
        if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
            timeMultiplier = Mathf.Max(0.1f, timeMultiplier / 2f);
    }

    void InitDefaultGradientIfNeeded()
    {
        if (lightColor == null || lightColor.colorKeys == null || lightColor.colorKeys.Length == 0)
        {
            lightColor = new Gradient();
            lightColor.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(new Color(0.05f, 0.05f, 0.15f), 0f),   // nuit
                    new GradientColorKey(new Color(1f, 0.85f, 0.6f), 0.25f),    // lever du soleil
                    new GradientColorKey(Color.white, 0.5f),                   // midi
                    new GradientColorKey(new Color(1f, 0.6f, 0.3f), 0.75f),    // coucher du soleil
                    new GradientColorKey(new Color(0.05f, 0.05f, 0.15f), 1f)   // nuit
                },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
            );
        }
    }
}
