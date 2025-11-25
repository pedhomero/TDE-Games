using UnityEngine;

public class CursorPersistence : MonoBehaviour
{
    private void Awake()
    {
        // Impede que o cursor seja destruído ao trocar de cena
        DontDestroyOnLoad(gameObject);
    }
}
