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

    public bool facingRight = true;
    private float direction;
    private float aimAngle = 0f;
    private Animator anim;

    void Start()
    {
        if (rig == null)
            rig = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        // Detecta orientação inicial com base na escala
        facingRight = transform.localScale.x >= 0f;
        FixScale();
    }

    void Update()
    {
        if (!TurnManager.instance.IsMyTurn(playerNumber))
        {
            if (anim != null)
                anim.SetBool("isWalking", false);
            return;
        }

        HandleMovement();
        HandleAiming();
        HandleFlip();
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

        if (Input.GetKeyDown(KeyCode.Space))
            rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

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
            Vector3 dir = facingRight
                ? Quaternion.Euler(0, 0, aimAngle) * Vector3.right
                : Quaternion.Euler(0, 0, -aimAngle) * Vector3.left;

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
}
