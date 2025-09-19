using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float minPower = 5f;
    public float maxPower = 20f;
    public float powerChargeSpeed = 15f;
    
    [Header("UI")]
    public RectTransform powerBarFill;
    public GameObject powerBarUI;
    
    private float currentPower = 0f;
    private bool isCharging = false;
    private bool canShoot = true;

    void Start()
    {
        // Esconder a barra de força no início
        if (powerBarUI != null)
            powerBarUI.SetActive(false);
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
        isCharging = true;
        currentPower = minPower;
        
        // Mostrar a barra de força
        if (powerBarUI != null)
            powerBarUI.SetActive(true);
    }

    void ChargePower()
    {
        // Aumentar a força gradualmente
        currentPower += powerChargeSpeed * Time.deltaTime;
        currentPower = Mathf.Clamp(currentPower, minPower, maxPower);
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // Criar o projétil na posição do FirePoint
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            
            // Pegar o Rigidbody2D do projétil
            Rigidbody2D projRig = projectile.GetComponent<Rigidbody2D>();
            
            if (projRig != null)
            {
                // Calcular direção do tiro (baseado na rotação do FirePoint)
                Vector2 shootDirection = firePoint.right;
                
                // Aplicar força no projétil
                projRig.AddForce(shootDirection * currentPower, ForceMode2D.Impulse);
            }

            // Configurar quem atirou (para não colidir consigo mesmo)
            Projectile projScript = projectile.GetComponent<Projectile>();
            if (projScript != null)
            {
                projScript.owner = this.gameObject;
            }
        }

        // Reset do sistema
        StopCharging();
        
        // Cooldown para não atirar muito rápido
        StartCoroutine(ShootCooldown());
    }
    
    void StopCharging()
    {
        isCharging = false;
        currentPower = 0f;
        
        // Esconder barra de força
        if (powerBarUI != null)
            powerBarUI.SetActive(false);
    }

    System.Collections.IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(0.5f); // Meio segundo de cooldown
        canShoot = true;
    }

    void UpdatePowerBar()
    {
        if (powerBarFill != null && isCharging)
        {
            // Calcular porcentagem da força (0 a 1)
            float fillAmount = (currentPower - minPower) / (maxPower - minPower);
            
            // Atualizar escala da barra
            powerBarFill.localScale = new Vector3(fillAmount, 1, 1);
        }
    }
}