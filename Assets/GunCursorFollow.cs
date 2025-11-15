using UnityEngine;

public class GunCursorFollow : MonoBehaviour
{
    public RectTransform cursor;

    void Start()
    {
        Cursor.visible = false; // esconde o cursor normal
    }

    void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform,
            Input.mousePosition,
            null,
            out pos
        );

        cursor.anchoredPosition = pos;
    }
}
