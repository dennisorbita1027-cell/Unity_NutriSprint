using UnityEngine;

public class FollowPlayerFixedZ : MonoBehaviour
{
    //Player Reference
    public Transform player;
    public float offsetZ = 1f;
    public Vector3 fixedXY;

    void LateUpdate()
    {
        if (player == null) return;
        // Keep the Quad at fixed Z offset from the player
        transform.position = new Vector3(fixedXY.x, fixedXY.y, player.position.z + offsetZ
        );
    }
}
