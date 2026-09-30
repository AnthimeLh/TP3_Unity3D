using UnityEngine;

// EXERCICE 5 : Comportement du monstre
// A poser sur le GameObject racine du monstre.
// IMPORTANT : le monstre doit avoir un enfant avec un Collider configuré en
// "Is Trigger" = true (ex: une Sphere Collider de rayon 5) qui sert de "zone de surveillance".
[RequireComponent(typeof(Health))]
public class MonsterAI : MonoBehaviour
{
    [Header("Cible")]
    public Transform player; // laissé vide : recherché automatiquement via le tag "Player"

    [Header("Comportement d'attaque")]
    public float attackInterval = 1.5f;
    public float attackRange = 2f;
    public float rotationSpeed = 5f;

    private Health monsterHealth;
    private Health playerHealth;
    private Animator animator;
    private bool playerInZone;
    private float attackTimer;

    void Start()
    {
        monsterHealth = GetComponent<Health>();
        // GetComponentInChildren (et pas juste GetComponent) car sur beaucoup de modèles
        // importés, l'Animator est sur un enfant (le modèle 3D) et non sur la racine.
        animator = GetComponentInChildren<Animator>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
        if (player != null)
            playerHealth = player.GetComponent<Health>();
    }

    // Ces deux méthodes sont appelées soit directement par Unity (si le Collider
    // "Is Trigger" est sur CE MEME GameObject), soit par le script relais
    // MonsterTriggerZone.cs posé sur l'enfant "ZoneDeSurveillance".
    public void NotifyPlayerEnter(Transform other)
    {
        if (other == player)
            playerInZone = true;
    }

    public void NotifyPlayerExit(Transform other)
    {
        if (other == player)
            playerInZone = false;
    }

    void OnTriggerEnter(Collider other) => NotifyPlayerEnter(other.transform);
    void OnTriggerExit(Collider other) => NotifyPlayerExit(other.transform);

    void Update()
    {
        if (monsterHealth.IsDead || player == null) return;

        if (playerInZone)
        {
            // Le monstre se tourne vers le Player
            Vector3 lookTarget = new Vector3(player.position.x, transform.position.y, player.position.z);
            Vector3 dir = (lookTarget - transform.position);
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            float dist = Vector3.Distance(transform.position, player.position);
            attackTimer += Time.deltaTime;

            if (dist <= attackRange && attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                Attack();
            }
        }
    }

    [Header("Synchronisation animation / dégâts")]
    [Tooltip("Délai (en secondes) entre le début de l'animation et l'impact réel du coup. " +
             "Réglez-le en regardant l'animation au ralenti pour trouver le bon instant. " +
             "Ignoré si vous utilisez un Animation Event (méthode DealDamageOnAnimationEvent).")]
    public float damageDelay = 0.4f;

    void Attack()
    {
        if (animator != null) animator.SetTrigger("Attack");

        // Les dégâts s'appliquent un peu après le début de l'animation, pour
        // correspondre visuellement au moment où l'arme touche le Player.
        // Si vous ajoutez un Animation Event (recommandé, voir DealDamageOnAnimationEvent),
        // vous pouvez retirer cette ligne et ne garder que la version "événement".
        Invoke(nameof(DealDamageDelayed), damageDelay);
    }

    void DealDamageDelayed()
    {
        if (playerHealth != null && !playerHealth.IsDead)
        {
            playerHealth.TakeDamage(monsterHealth.attackPower);
        }
    }

    // A appeler depuis un Animation Event placé sur la frame exacte de l'impact
    // (voir le guide) : méthode PUBLIQUE et SANS PARAMÈTRE pour apparaître dans
    // la liste déroulante de l'éditeur d'Animation Event.
    public void DealDamageOnAnimationEvent()
    {
        if (playerHealth != null && !playerHealth.IsDead)
        {
            playerHealth.TakeDamage(monsterHealth.attackPower);
        }
    }
}