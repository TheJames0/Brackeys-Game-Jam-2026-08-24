using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float maxLookAngleX = 60f;
    [SerializeField] private float maxLookAngleY = -10f;

    private float yaw;
    private float pitch;
    private void Update()
    {
        float mouseX = Mouse.current.delta.x.ReadValue();
        float mouseY = Mouse.current.delta.y.ReadValue();

        yaw += mouseX * sensitivity;
        yaw = Mathf.Clamp(yaw, -maxLookAngleX, maxLookAngleX);

        pitch += mouseY * sensitivity;
        pitch = Mathf.Clamp(pitch, -maxLookAngleY, 0f);

        transform.localRotation = Quaternion.Euler(-pitch, yaw, 0f);
    }
}
