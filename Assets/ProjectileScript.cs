using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float damage = 20f;
    public float lifeTime = 8f;
    public GameObject owner;

    [Header("Explosão")]
    public GameObject Explosão_Prefab;
    public AudioClip explosionSound;


    
    private bool hasExploded = false;

    void Start()
    {
        Debug.Log("🚀 Projétil criado!");
        
        // Destruir após tempo
        Destroy(gameObject, lifeTime);
        
        // Ignorar colisão com dono
        if (owner != null)
        {
            Collider2D ownerCollider = owner.GetComponent<Collider2D>();
            Collider2D myCollider = GetComponent<Collider2D>();
            
            if (ownerCollider != null && myCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, ownerCollider);
                Debug.Log("🔄 Ignorando colisões com " + owner.name);
            }
        }
        
        AddTrailEffect();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("💥 Projétil colidiu com: " + collision.gameObject.name);
        
        // Não colidir com o próprio jogador
        if (collision.gameObject == owner)
        {
            Debug.Log("🔄 Ignorando colisão com owner");
            return;
        }

        // Parar movimento
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Explodir
        Explode();
    }

    void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;
        
         Debug.Log("💥 EXPLOSÃO!");
        // Toca o som da explosão
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, 1f);
        }
        
        if (Explosão_Prefab != null)
        {
            GameObject explosion = Instantiate(Explosão_Prefab, transform.position, Quaternion.identity);
            Destroy(explosion, 0.5f);
        }
        
        Debug.Log("💥 EXPLOSÃO na posição: " + transform.position);
        
        // Causar dano em área
        CauseDamageInArea();
        

        
        // Destruir projétil
        Destroy(gameObject, 0.1f);
    }

    void CauseDamageInArea()
    {
        float explosionRadius = 3f;
        
        Debug.Log("💥 ============ INÍCIO DA ANÁLISE DE DANO ============");
        Debug.Log("🔍 Posição da explosão: " + transform.position);
        Debug.Log("🔍 Owner (quem atirou): " + (owner != null ? owner.name : "NULL"));
        Debug.Log("🔍 Procurando alvos em raio de " + explosionRadius + "m");
        
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        
        Debug.Log("🔍 Encontrados: " + objectsInRange.Length + " objetos próximos");
        
        int playerCount = 0;
        
        foreach (Collider2D obj in objectsInRange)
        {
            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━");
            Debug.Log("🎯 Objeto: " + obj.gameObject.name);
            
            // Verificar se é o owner
            bool isOwner = (obj.gameObject == owner);
            Debug.Log("🎯 É o owner? " + (isOwner ? "SIM - VAI PULAR" : "NÃO"));
            
            if (isOwner)
            {
                Debug.Log("⏭️ PULANDO OWNER: " + owner.name);
                continue;
            }
                
            // Procurar sistema de vida
            HealthSystem healthSystem = obj.GetComponent<HealthSystem>();
            Debug.Log("🎯 Tem HealthSystem? " + (healthSystem != null ? "SIM!" : "NÃO"));
            
            if (healthSystem != null)
            {
                playerCount++;
                Debug.Log("✅✅✅ PLAYER DETECTADO! ✅✅✅");
                Debug.Log("✅ Nome: " + obj.name);
                
                // Calcular dano baseado na distância
                float distance = Vector2.Distance(transform.position, obj.transform.position);
                float damageMultiplier = 1f - (distance / explosionRadius);
                float finalDamage = damage * Mathf.Clamp01(damageMultiplier);
                
                Debug.Log("📏 Distância: " + distance.ToString("F2") + "m");
                Debug.Log("📊 Multiplicador: " + (damageMultiplier * 100f).ToString("F0") + "%");
                Debug.Log("⚡ Dano base: " + damage);
                Debug.Log("⚡ DANO FINAL: " + finalDamage.ToString("F1"));
                
                if (finalDamage > 0)
                {
                    Debug.Log("🎯 APLICANDO DANO AGORA...");
                    healthSystem.TakeDamage(finalDamage);
                    Debug.Log("✅✅✅ DANO APLICADO COM SUCESSO! ✅✅✅");
                }
                else
                {
                    Debug.LogWarning("⚠️ Dano = 0 (muito longe)");
                }
            }
            
            // Aplicar força de repulsão
            Rigidbody2D objRig = obj.GetComponent<Rigidbody2D>();
            if (objRig != null && !isOwner)
            {
                Vector2 direction = (obj.transform.position - transform.position).normalized;
                float distance = Vector2.Distance(transform.position, obj.transform.position);
                float forceMultiplier = 1f - (distance / explosionRadius);
                
                objRig.AddForce(direction * 300f * Mathf.Clamp01(forceMultiplier));
                Debug.Log("💨 Aplicou força de repulsão");
            }
        }
        
        Debug.Log("💥 ============ FIM DA ANÁLISE ============");
        Debug.Log("💥 TOTAL DE PLAYERS ATINGIDOS: " + playerCount);
    }

    void CreateExplosionEffect()
    {
        // Efeito visual
        for (int i = 0; i < 12; i++)
        {
            GameObject particle = new GameObject("ExplosionParticle");
            particle.transform.position = transform.position;
            
            SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
            sr.sprite = GetComponent<SpriteRenderer>()?.sprite;
            sr.color = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);
            particle.transform.localScale = Vector3.one * Random.Range(0.1f, 0.2f);
            
            Rigidbody2D particleRig = particle.AddComponent<Rigidbody2D>();
            particleRig.gravityScale = 0.5f;
            
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            particleRig.AddForce(randomDirection * Random.Range(150f, 400f));
            
            Destroy(particle, 1.5f);
        }
    }
    
    void AddTrailEffect()
    {
        TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 0.3f;
        trail.startWidth = 0.05f;
        trail.endWidth = 0.01f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = Color.yellow;
        trail.endColor = new Color(1f, 1f, 0f, 0f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, 3f);
    }
}
