using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 10f;
    public Rigidbody2D rig;
    
    [Header("Aiming")]
    public Transform cannon;
    public Transform aimPoint;
    public float minAngle = -45f;
    public float maxAngle = 45f;
    public float aimSpeed = 50f;
    
    private float direction;
    private float aimAngle = 0f;
    private bool facingRight = true;
    Animator anim;

    
    void Start()
    {
        // Pega automaticamente o Rigidbody2D se não foi configurado
        if (rig == null)
            rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }

    void Update()
    {
        // Movimento horizontal
        direction = Input.GetAxis("Horizontal");

        // Pulo
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        
        // NOVO: Sistema de mira
        HandleAiming();

        // NOVO: Virar o personagem
        HandleFlipping();
        
        bool isWalking = Mathf.Abs(direction) > 0.1f;
        anim.SetBool("isWalking", isWalking);
    }
    
    void FixedUpdate()
    {
        rig.linearVelocity = new Vector2(direction * speed, rig.linearVelocity.y);
    }
    
    // NOVO: Função para controlar a mira
    void HandleAiming()
    {
        // Mira com as setas do teclado
        if (Input.GetKey(KeyCode.UpArrow))
        {
            aimAngle += aimSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            aimAngle -= aimSpeed * Time.deltaTime;
        }

        // Limitar o ângulo entre min e max
        aimAngle = Mathf.Clamp(aimAngle, minAngle, maxAngle);

        // Aplicar rotação no canhão
        if (cannon != null)
        {
            float finalAngle = facingRight ? aimAngle : 180f - aimAngle;
            cannon.rotation = Quaternion.Euler(0, 0, finalAngle);
        }

        // Atualizar posição do ponto de mira
        if (aimPoint != null)
        {
            float angleRad = aimAngle * Mathf.Deg2Rad;
            Vector3 aimDirection;
            
            if (facingRight)
                aimDirection = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0);
            else
                aimDirection = new Vector3(-Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0);
            
            aimPoint.position = cannon.position + aimDirection * 2f;
        }
    }
    
    // NOVO: Função para virar o personagem
    void HandleFlipping()
    {
        if (direction > 0 && !facingRight)
        {
            Flip();
        }
        else if (direction < 0 && facingRight)
        {
            Flip();
        }
    }
    
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}