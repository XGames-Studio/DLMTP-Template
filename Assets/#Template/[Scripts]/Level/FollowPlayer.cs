using DancingLineFanmade.Level;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Vector3 offset;
    public bool keepOriginY;
    internal Transform player;

    private void Start()
    {
        player = Player.Instance.transform;
    }

    private void LateUpdate()
    {
        if (LevelManager.GameState == GameStatus.Moving || LevelManager.GameState == GameStatus.Playing)
        {
            if (player == null)
                return;
            Vector3 targetPosition = player.position + offset;
            if (keepOriginY)
            {
                targetPosition.y = transform.position.y;
            }
            transform.position = targetPosition;
        }
    }
}
