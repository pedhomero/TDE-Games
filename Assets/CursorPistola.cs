using UnityEngine;

public class CursorGun : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        Cursor.visible = false; 
    }

    void Update()
    {
        // Segue o mouse
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        transform.position = pos;

        // Atira ao clicar
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Shoot");
            Shoot();
        }
    }

    void Shoot()
    {
        Debug.Log("Atirou!");
        // aqui você coloca seu sistema de tiro de verdade
    }
}
