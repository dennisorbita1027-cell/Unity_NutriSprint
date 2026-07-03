using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //Character Reference
    public Transform player;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 6, -10);

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = new Vector3(0, player.position.y + offset.y, player.position.z + offset.z );

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(15f, 0f, 0f); // Your current rotation
    }
}
