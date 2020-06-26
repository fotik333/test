using Interfaces;
using Mirror;

namespace GameObjects.MultiPlayer
{
    public class PaddleNetworkBehaviour : NetworkBehaviour, IPaddleBehaviour
    {
        public bool IsLocalPlayer()
        {
            return isLocalPlayer;
        }
    }
}
