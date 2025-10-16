using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI Elements")]
    public Slider healthBar;
    public Text healthText;

    [Header("Visual Effects")]
    public Color normalColor = Color.white;
    public Color damagedColor = Color.red;
    public float flashDuration = 0.2f;

    private SpriteRenderer spriteRenderer;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateHealthUI();

        Debug.Log("💚 " + gameObject.name + " iniciado com " + maxHealth + " de vida");
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("💥 " + gameObject.name + " tomou " + damage.ToString("F1") + " de dano! Vida: " + currentHealth.ToString("F1"));

        StartCoroutine(FlashDamage());
        UpdateHealthUI();

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        if (isDead) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("💚 " + gameObject.name + " curou " + healAmount);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;

            Image fillImage = healthBar.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                if (currentHealth > maxHealth * 0.6f)
                    fillImage.color = Color.green;
                else if (currentHealth > maxHealth * 0.3f)
                    fillImage.color = Color.yellow;
                else
                    fillImage.color = Color.red;
            }
        }

        if (healthText != null)
        {
            healthText.text = currentHealth.ToString("F0") + "/" + maxHealth.ToString("F0");
        }
    }

    System.Collections.IEnumerator FlashDamage()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = damagedColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("💀 " + gameObject.name + " Foi com Deus!");
        StartCoroutine(DeathEffect());
    }

    System.Collections.IEnumerator DeathEffect()
    {
        float deathTime = 2f;
        float elapsedTime = 0f;

        Vector3 originalScale = transform.localScale;

        while (elapsedTime < deathTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / deathTime;

            transform.Rotate(0, 0, 360f * Time.deltaTime);
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);

            if (spriteRenderer != null)
            {
                Color newColor = spriteRenderer.color;
                newColor.a = 1f - progress;
                spriteRenderer.color = newColor;
            }

            yield return null;
        }

        GameOver();
    }

    void GameOver()
    {
        Debug.Log("🎮 Acabou pra você meu chapa!");
        gameObject.SetActive(false);
    }

    public bool IsAlive()
    {
        return !isDead;
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    void Update()
{
    // TESTE: Verificando se o código esta funcionando,
    // comando: T para tomar 10 de dano
    if (Input.GetKeyDown(KeyCode.T))
    {
        Debug.Log("🧪 TESTE MANUAL: Aplicando 10 de dano em " + gameObject.name);
        TakeDamage(10);
    }
}
}