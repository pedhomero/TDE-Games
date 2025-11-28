using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    [Header("DEVE APARECER ESTES CAMPOS")]
    public GameObject projectilePrefab;
    public Transform firePoint; // Mantemos a referência, mas não usaremos o .position
    public RectTransform powerBarFill;
    public GameObject powerBarUI;

    [Header("Muzzle Settings")]
    public float muzzleOffset = 1.0f; // <--- NOVO CAMPO: Distância da ponta do cano
    public GameObject MuzzleFlash_Prefab;

    [Header("Power Settings")]
    public float minPower = 1f;
    public float maxPower = 100f;
    public float powerChargeSpeed = 0.5f;

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
        if (projectilePrefab == null || playerScript.cannon == null) // Checa o canhão em vez do firePoint
        {
            Debug.LogError("❌ Faltam componentes para atirar!");
            StopCharging();
            return;
        }

        // 1. Pega a direção correta (que já está funcionando)
        Vector2 shootDirection = playerScript.GetAimDirection(); 

        // 🎯 CORREÇÃO DEFINITIVA DA POSIÇÃO DE SPAWN:
        // Usa a posição do PIVÔ do canhão (que é fixa) e move ao longo do vetor de direção corrigido.
        Vector3 spawnPosition = playerScript.cannon.position + (Vector3)shootDirection * muzzleOffset;
        
        // 2. Instancia o projétil na posição corrigida
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        // Garante que a escala seja positiva e no tamanho correto do prefab
        Vector3 originalScale = projectilePrefab.transform.localScale;
        projectile.transform.localScale = new Vector3(Mathf.Abs(originalScale.x), Mathf.Abs(originalScale.y), Mathf.Abs(originalScale.z));

        // 3. Aplica força
        Rigidbody2D projRig = projectile.GetComponent<Rigidbody2D>();
        if (projRig != null)
        {
            projRig.AddForce(shootDirection * currentPower, ForceMode2D.Impulse);

            // Rotaciona a bala visualmente para a direção do tiro
            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

                 // DEBUG COMPLETO
        Debug.Log("========== DEBUG TIRO ==========");
        Debug.Log("Current Power: " + currentPower);
        Debug.Log("Min Power: " + minPower);
        Debug.Log("Max Power: " + maxPower);
        Debug.Log("Shoot Direction: " + shootDirection);
        Debug.Log("Massa ANTES: " + projRig.mass);
        Debug.Log("Gravity ANTES: " + projRig.gravityScale);
        Debug.Log("Drag ANTES: " + projRig.linearDamping);
        }

        // Efeito de fogo
        if (MuzzleFlash_Prefab != null)
        {
            // Instancia o flash na mesma posição e rotação corrigidas
            GameObject flash = Instantiate(MuzzleFlash_Prefab, spawnPosition, Quaternion.Euler(0, 0, projectile.transform.eulerAngles.z));
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