using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 60f;

    private float yaw;

    private void Update()
    {
        float mouseX = Mouse.current.delta.x.ReadValue();

        yaw += mouseX * sensitivity;
        yaw = Mathf.Clamp(yaw, -maxLookAngle, maxLookAngle);

        transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
    }
}
