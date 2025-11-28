using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("⚙️ Player Info")]
    public int playerNumber = 1;

    [Header("🏃 Movimento")]
    public float speed = 5f;
    public float jumpForce = 10f;
    public Rigidbody2D rig;

    [Header("🎯 Mira")]
    public Transform cannon;
    public Transform aimPoint;
    public float minAngle = -45f;
    public float maxAngle = 45f;
    public float aimSpeed = 50f;
    
    [Header("🔍 Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public bool facingRight = true;
    private float direction;
    private float aimAngle = 0f;
    private Animator anim;
    private bool isGrounded = false; // NOVO

    void Start()
    {
        if (rig == null)
            rig = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        // Detecta orientação inicial
        facingRight = transform.localScale.x >= 0f;
        FixScale();
        
        // Criar GroundCheck se não existir
        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = gc.transform;
        }

         // DEBUG DE LAYERS
    Debug.Log("Ground Layer Mask value: " + groundLayer.value);
    Debug.Log("Ground Layer Mask (binary): " + System.Convert.ToString(groundLayer.value, 2));
    
    // Verificar se Ground existe
    int groundLayerIndex = LayerMask.NameToLayer("Ground");
    Debug.Log("Ground layer index: " + groundLayerIndex);
    
    if (groundLayerIndex == -1)
    {
        Debug.LogError("Layer 'Ground' NAO EXISTE! Crie ela em Layers -> Edit Layers");
    }
    }

    void Update()
    {
        if (!TurnManager.instance.IsMyTurn(playerNumber))
        {
            if (anim != null)
                anim.SetBool("isWalking", false);
            return;
        }
        
        CheckGrounded(); // NOVO - verificar se está no chão
        HandleMovement();
        HandleAiming();
        HandleFlip();
    }
    
    // NOVA FUNÇÃO - Verificar se está no chão
    void CheckGrounded()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
    }

    void HandleMovement()
    {
        direction = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(direction) > 0.1f)
        {
            if (TurnManager.instance.TryUseStep())
                rig.linearVelocity = new Vector2(direction * speed, rig.linearVelocity.y);
        }
        else
        {
            rig.linearVelocity = new Vector2(0, rig.linearVelocity.y);
        }

        // MODIFICADO - Só pula se estiver no chão
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            Debug.Log("Player " + playerNumber + " pulou!");
        }

        if (anim != null)
            anim.SetBool("isWalking", Mathf.Abs(direction) > 0.1f);
    }

    void HandleAiming()
    {
        float aimInput = 0f;
        if (Input.GetKey(KeyCode.UpArrow)) aimInput = 1f;
        else if (Input.GetKey(KeyCode.DownArrow)) aimInput = -1f;

        aimAngle += aimInput * aimSpeed * Time.deltaTime;
        aimAngle = Mathf.Clamp(aimAngle, minAngle, maxAngle);

        if (cannon != null)
        {
            float finalAngle = facingRight ? aimAngle : -aimAngle; 
            cannon.localRotation = Quaternion.Euler(0, 0, finalAngle);
        }

        if (aimPoint != null && cannon != null)
        {
            Vector3 dir = GetAimDirection();
            aimPoint.position = cannon.position + dir.normalized * 2f;
        }
    }

    void HandleFlip()
    {
        if (direction > 0.1f && !facingRight)
            Flip();
        else if (direction < -0.1f && facingRight)
            Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        FixScale();
    }

    void FixScale()
    {
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * (facingRight ? 1f : -1f);
        transform.localScale = s;
    }

    public Vector3 GetAimDirection()
    {
        Vector3 localDir = Quaternion.Euler(0, 0, aimAngle) * Vector3.right;
        
        if (!facingRight)
        {
            localDir.x *= -1f;
        }
        
        return localDir.normalized;
    }
    
    // NOVA FUNÇÃO - Visualizar área de detecção no editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
