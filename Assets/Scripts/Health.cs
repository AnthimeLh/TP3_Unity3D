using UnityEngine;
using System;

// EXERCICE 5 : Statistiques (Vie / Force d'attaque)
// A poser à la fois sur le Player ET sur le Monstre.
public class Health : MonoBehaviour
{
    [Header("Statistiques")]
    public int maxHealth = 100;
    public int attackPower = 10;

    [HideInInspector] public int currentHealth;

    // Evénements utilisés par la barre de vie (HealthBarUI) pour se mettre à jour
    public event Action<int, int> OnHealthChanged; // (vie actuelle, vie max)
    public event Action OnDeath;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth == 0)
            OnDeath?.Invoke();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public bool IsDead => currentHealth <= 0;
}
