using UnityEngine;
public class FollowPlayer : MonoBehaviour
{
    private Transform player;
    private void FixedUpdate()
    {
        transform.position = GameManager.instance.PlayerTran.position;
    }
}
