using UnityEngine;

public class EnemyImovile : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if(player != null)
        {
            player.ChangeHealth(-1);
            player.rigidbody2d.AddForce(Vector2.up + Vector2.left * 4f, ForceMode2D.Impulse);

        }
    }
    //void OnTriggerStay2D(Collider2D other)
    //{
    //    PlayerController controller = other.GetComponent<PlayerController>();


    //    if (controller != null)
    //    {
    //        controller.ChangeHealth(-1);
    //    }
    //}
}
