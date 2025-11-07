using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    [Header("DEVE APARECER ESTES CAMPOS")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public RectTransform powerBarFill;
    public GameObject powerBarUI;

    [Header("Efeito de Disparo")]
    public GameObject MuzzleFlash_Prefab;

    [Header("Power Settings")]
    public float minPower = 1f;
    public float maxPower = 5f;
    public float powerChargeSpeed = 2f;

    private float currentPower = 0f;
    private bool isCharging = false;
    private bool canShoot = true;

    private NewMonoBehaviourScript playerScript;

    void Start()
    {
        playerScript = GetComponentInParent<NewMonoBehaviourScript>();

        if (playerScript == null)
            Debug.LogWarning("⚠️ Nenhum script de Player encontrado como pai deste ShootingSystem!");
    }

    void Update()
    {
        // 🚫 Somente o player da vez pode atirar
        if (playerScript == null || !TurnManager.instance.IsMyTurn(playerScript.playerNumber))
            return;

        HandleShooting();
        UpdatePowerBar();
    }

    void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.X) && canShoot)
            StartCharging();

        if (Input.GetKey(KeyCode.X) && isCharging)
            ChargePower();

        if (Input.GetKeyUp(KeyCode.X) && isCharging)
            Shoot();
    }

    void StartCharging()
    {
        isCharging = true;
        currentPower = minPower;

        if (powerBarUI != null)
            powerBarUI.SetActive(true);
    }

    void ChargePower()
    {
        currentPower += powerChargeSpeed * Time.deltaTime;
        currentPower = Mathf.Clamp(currentPower, minPower, maxPower);
    }

    void Shoot()
    {
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
            Vector2 shootDirection = firePoint.right;

            if (playerScript != null && !playerScript.facingRight)
                shootDirection *= -1f;

            projRig.AddForce(shootDirection * currentPower, ForceMode2D.Impulse);
        }

        // Efeito de fogo
        if (MuzzleFlash_Prefab != null && firePoint != null)
        {
            GameObject flash = Instantiate(MuzzleFlash_Prefab, firePoint.position, firePoint.rotation);
            Destroy(flash, 0.3f);
        }

        // Configurar dono
        ProjectileScript projScript = projectile.GetComponent<ProjectileScript>();
        if (projScript != null)
            projScript.owner = this.gameObject;

        StopCharging();
        StartCoroutine(ShootCooldown());

        // ✅ Passa o turno após atirar
        TurnManager.instance.NextTurn();
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
        yield return new WaitForSeconds(0.8f);
        canShoot = true;
    }

    void UpdatePowerBar()
    {
        if (powerBarFill != null && isCharging)
        {
            float fillAmount = (currentPower - minPower) / (maxPower - minPower);
            powerBarFill.localScale = new Vector3(fillAmount, 1, 1);

            UnityEngine.UI.Image barImage = powerBarFill.GetComponent<UnityEngine.UI.Image>();
            if (barImage != null)
            {
                if (fillAmount < 0.5f)
                    barImage.color = Color.Lerp(Color.green, Color.yellow, fillAmount * 2f);
                else
                    barImage.color = Color.Lerp(Color.yellow, Color.red, (fillAmount - 0.5f) * 2f);
            }
        }
    }
}
