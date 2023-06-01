using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.TryGetComponent(out PlayerController player))
        {
            player.AddCoin();
            Destroy(gameObject);
        }
    }
}
