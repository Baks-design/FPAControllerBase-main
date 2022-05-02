using UnityEngine;

namespace Baks
{
    public class PlayerComponent : MonoBehaviour
    {
        Player m_player;

        protected Player Player => m_player;

        public void InitPlayerReference(Player player) => m_player = player;
    }
}
