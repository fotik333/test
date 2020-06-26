using UnityEngine;

namespace GameObjects.SinglePlayer
{
    public class BallSingleBehaviour : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Paddle")) gameObject.GetComponent<Ball>().OnCollisionWithPaddle(other);
        }
    }
}