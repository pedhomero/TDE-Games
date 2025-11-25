using UnityEngine;
using UnityEngine.UI;

public class GunCursorUI : MonoBehaviour
{
    public Animator animator;
    public RectTransform cursorTransform;
    public float followSpeed = 15f;

    void Start()
    {
        Cursor.visible = false; // esconde o cursor original
    }

    void Update()
    {
        // Atualizar posição da arma (cursor)
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)cursorTransform.parent,
            Input.mousePosition,
            null,
            out pos
        );

        cursorTransform.anchoredPosition = pos;

        // Quando clicar com o botão esquerdo → animação
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Shoot");
        }
    }
}
