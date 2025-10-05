using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float damage = 20f;
    public float lifeTime = 8f;
    public GameObject owner;
    
    private bool exploded = false;

    void Start()
    {
        Debug.Log("🚀 Projétil criado!");
        Destroy(gameObject, lifeTime);
        SetupVisuals();
        
        // Ignorar colisão com o dono
        if (owner != null)
        {
            Collider2D ownerCollider = owner.GetComponent<Collider2D>();
            Collider2D myCollider = GetComponent<Collider2D>();
            
            if (ownerCollider != null && myCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, ownerCollider);
            }
        }
    }

    void SetupVisuals()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.yellow;
            sr.sortingOrder = 5;
        }
        transform.localScale = new Vector3(0.3f, 0.3f, 1f);
    }

    void OnCollisionEnter2D(Collision2D hit)
    {
        Debug.Log("💥 Colidiu com: " + hit.gameObject.name);
        
        if (hit.gameObject == owner) return;
        
        // Parar movimento
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        ExplodeNow();
    }

    void ExplodeNow()
    {
        if (exploded) return;
        exploded = true;
        
        Debug.Log("💥 EXPLOSÃO!");
        
        FindAndDamageEnemies();
        MakeExplosionEffect();
        
        Destroy(gameObject, 0.2f);
    }

    void FindAndDamageEnemies()
    {
        float radius = 3f;
        Debug.Log("🔍 Buscando alvos em " + radius + "m de raio");
        
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, radius);
        Debug.Log("🔍 Encontrados: " + nearby.Length + " objetos");
        
        foreach (Collider2D target in nearby)
        {
            if (target.gameObject == owner) continue;
            
            Debug.Log("🎯 Verificando: " + target.gameObject.name);
            
            HealthSystem health = target.GetComponent<HealthSystem>();
            if (health != null)
            {
                Debug.Log("✅ ENCONTROU HealthSystem!");
                
                float dist = Vector2.Distance(transform.position, target.transform.position);
                float damagePercent = 1f - (dist / radius);
                float finalDamage = damage * Mathf.Clamp01(damagePercent);
                
                Debug.Log("⚡ Dano calculado: " + finalDamage.ToString("F1"));
                
                if (finalDamage > 0)
                {
                    health.TakeDamage(finalDamage);
                    Debug.Log("✅ DANO APLICADO!");
                }
            }
            else
            {
                Debug.Log("❌ Sem HealthSystem em " + target.name);
            }
            
            Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = (target.transform.position - transform.position).normalized;
                rb.AddForce(dir * 200f);
            }
        }
    }

    void MakeExplosionEffect()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject spark = new GameObject("Spark");
            spark.transform.position = transform.position;
            
            SpriteRenderer sr = spark.AddComponent<SpriteRenderer>();
            sr.sprite = GetComponent<SpriteRenderer>()?.sprite;
            sr.color = Random.ColorHSV();
            spark.transform.localScale = Vector3.one * 0.1f;
            
            Rigidbody2D rb = spark.AddComponent<Rigidbody2D>();
            rb.AddForce(Random.insideUnitCircle * 300f);
            
            Destroy(spark, 1f);
        }
    }
}