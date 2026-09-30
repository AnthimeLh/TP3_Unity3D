using UnityEngine;

// A poser sur l'enfant "ZoneDeSurveillance" (celui qui a le Sphere Collider "Is Trigger").
// Ce script relaie les événements de trigger vers le script MonsterAI situé
// sur le GameObject PARENT (racine du monstre), car Unity n'envoie pas
// automatiquement OnTriggerEnter/Exit aux scripts des objets parents.
public class MonsterTriggerZone : MonoBehaviour
{
    private MonsterAI monsterAI;

    void Start()
    {
        monsterAI = GetComponentInParent<MonsterAI>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (monsterAI != null) monsterAI.NotifyPlayerEnter(other.transform);
    }

    void OnTriggerExit(Collider other)
    {
        if (monsterAI != null) monsterAI.NotifyPlayerExit(other.transform);
    }
}
