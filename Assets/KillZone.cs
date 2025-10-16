using UnityEngine;

public class KillZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("☠️ " + other.name + " entrou na Kill Zone!");
        
        // Procurar o HealthSystem
        HealthSystem health = other.GetComponent<HealthSystem>();
        
        if (health != null)
        {
            Debug.Log("💀 MORTE INSTANTÂNEA POR QUEDA!");
            // Matar instantaneamente
            health.TakeDamage(health.maxHealth);
        }
    }
}