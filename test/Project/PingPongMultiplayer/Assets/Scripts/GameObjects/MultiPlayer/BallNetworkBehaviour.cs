using Mirror;
using UnityEngine;

namespace GameObjects.MultiPlayer
{
    public class BallNetworkBehaviour : NetworkBehaviour
    {
        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Paddle")) gameObject.GetComponent<Ball>().OnCollisionWithPaddle(other);
        }
    }
}