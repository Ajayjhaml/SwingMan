using UnityEngine;

public class Coin : MonoBehaviour
{
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f; // coin won’t fall due to gravity
            rb.constraints = RigidbodyConstraints2D.FreezeAll; // no movement or rotation
        }
    }
}
