using Interfaces;
using UnityEngine;

namespace GameObjects.SinglePlayer
{
    public class PaddleSingleBehaviour : MonoBehaviour, IPaddleBehaviour
    {
        public bool IsLocalPlayer()
        {
            return true;
        }
    }
}