using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float damage = 20f;
    public float lifeTime = 5f;
    public GameObject owner;

    private bool hasExploded = false;

    void Start()
    {
        // Destruir o projétil após um tempo para não acumular na cena
        Destroy(gameObject, lifeTime);

        // Adicionar rastro visual (opcional)
        AddTrailEffect();

        Debug.Log("🚀 Projétil criado!");
    }

    // REMOVIDO - não vamos usar Trigger

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Não colidir com o próprio jogador que atirou
        if (collision.gameObject == owner)
            return;

        Debug.Log("💥 Projétil colidiu com: " + collision.gameObject.name);

        // Explodir quando tocar em qualquer coisa
        Explode();
    }

    void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        Debug.Log("💥 EXPLOSÃO!");

        // Criar efeito visual simples
        CreateExplosionEffect();

        // Destruir o projétil
        Destroy(gameObject);
    }

    void CreateExplosionEffect()
    {
        // Efeito visual simples: criar algumas partículas coloridas
        for (int i = 0; i < 8; i++)
        {
            GameObject particle = new GameObject("ExplosionParticle");
            particle.transform.position = transform.position;

            // Adicionar visual
            SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
            sr.sprite = GetComponent<SpriteRenderer>().sprite; // Usar o mesmo sprite do projétil
            sr.color = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f); // Cores aleatórias
            particle.transform.localScale = Vector3.one * 0.1f;

            // Adicionar física
            Rigidbody2D particleRig = particle.AddComponent<Rigidbody2D>();
            particleRig.gravityScale = 0.5f;

            // Direção aleatória
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            particleRig.AddForce(randomDirection * Random.Range(100f, 300f));

            // Destruir a partícula
            Destroy(particle, 1f);
        }
    }

    void AddTrailEffect()
    {
        // Adicionar rastro atrás do projétil
        TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 0.3f;
        trail.startWidth = 0.05f;
        trail.endWidth = 0.01f;
        trail.material = new Material(Shader.Find("Sprites/Default"));

        trail.startColor = Color.yellow;
        trail.endColor = new Color(1f, 1f, 0f, 0f); // amarelo que vai sumindo

    }
}