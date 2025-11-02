using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    [Header("DEVE APARECER ESTES CAMPOS")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public RectTransform powerBarFill;
    public GameObject powerBarUI;

    [Header("Efeito de Disparo")]
    public GameObject MuzzleFlash_Prefab; // Prefab da explosão de fogo ao atirar
    
    [Header("Power Settings")]
    public float minPower = 1f;        
    public float maxPower = 5f;        
    public float powerChargeSpeed = 2f; 
    
    private float currentPower = 0f;
    private bool isCharging = false;
    private bool canShoot = true;
    
    void Start()
    {
        Debug.Log("✅ ShootingSystem iniciado!");
    }

    void Update()
    {
        HandleShooting();
        UpdatePowerBar();
    }

    void HandleShooting()
    {
        // Pressionar X para começar a carregar
        if (Input.GetKeyDown(KeyCode.X) && canShoot)
        {
            StartCharging();
        }

        // Segurar X para carregar força
        if (Input.GetKey(KeyCode.X) && isCharging)
        {
            ChargePower();
        }

        // Soltar X para disparar
        if (Input.GetKeyUp(KeyCode.X) && isCharging)
        {
            Shoot();
        }
    }

    void StartCharging()
    {
        Debug.Log("🔋 Começou a carregar - força inicial: " + minPower);
        isCharging = true;
        currentPower = minPower;
        
        // Mostrar a barra de força
        if (powerBarUI != null)
            powerBarUI.SetActive(true);
    }

    void ChargePower()
    {
        // Aumentar força mais lentamente
        float oldPower = currentPower;
        currentPower += powerChargeSpeed * Time.deltaTime;
        currentPower = Mathf.Clamp(currentPower, minPower, maxPower);
        
        // Debug a cada meio segundo para não spammar
        if (Time.time % 0.5f < 0.1f && oldPower != currentPower)
        {
            Debug.Log("⚡ Carregando força: " + currentPower.ToString("F1"));
        }
    }

    void Shoot()
    {
        Debug.Log("🎯 DISPARANDO com força: " + currentPower.ToString("F1"));
        
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("❌ Faltam componentes para atirar!");
            StopCharging();
            return;
        }

        // Criar projétil
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
        // Aplicar física
        Rigidbody2D projRig = projectile.GetComponent<Rigidbody2D>();
        if (projRig != null)
        {
            // Direção baseada na mira
            Vector2 shootDirection = firePoint.right;

            // Aplicar força mais suave
            projRig.AddForce(shootDirection * currentPower, ForceMode2D.Impulse);

            // Ajustar massa do projétil para voo mais controlado
            projRig.mass = 0.3f; // Mais pesado = voo mais curto
            projRig.linearDamping = 0.1f; // Resistência do ar
        }

        if(MuzzleFlash_Prefab!= null && firePoint != null)
        {
            GameObject flash = Instantiate(MuzzleFlash_Prefab, firePoint.position, firePoint.rotation);
            Destroy(flash, 0.3f);
        }
    

        // Configurar dono
        ProjectileScript projScript = projectile.GetComponent<ProjectileScript>();
        if (projScript != null)
        {
            projScript.owner = this.gameObject;
        }

        StopCharging();
        StartCoroutine(ShootCooldown());
    }

    void StopCharging()
    {
        isCharging = false;
        currentPower = 0f;
        
        if (powerBarUI != null)
            powerBarUI.SetActive(false);
    }

    System.Collections.IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(0.8f); // Cooldown maior
        canShoot = true;
        Debug.Log("✅ Pode atirar novamente");
    }

    void UpdatePowerBar()
    {
        if (powerBarFill != null && isCharging)
        {
            // Cálculo mais suave da barra
            float fillAmount = (currentPower - minPower) / (maxPower - minPower);
            powerBarFill.localScale = new Vector3(fillAmount, 1, 1);
            
            // Mudança de cor baseada na força
            UnityEngine.UI.Image barImage = powerBarFill.GetComponent<UnityEngine.UI.Image>();
            if (barImage != null)
            {
                // Verde no início, amarelo no meio, vermelho no máximo
                if (fillAmount < 0.5f)
                    barImage.color = Color.Lerp(Color.green, Color.yellow, fillAmount * 2f);
                else
                    barImage.color = Color.Lerp(Color.yellow, Color.red, (fillAmount - 0.5f) * 2f);
            }
        }
    }
}