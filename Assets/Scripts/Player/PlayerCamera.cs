using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] CinemachineCamera playerCam; // Cinemachine camera component reference

    // Event Subscription / Unsubscription
    void OnEnable()
    {
        EventManager.onPlayerSpawned += SetCameraTarget;
    }
    void OnDisable()
    {
        EventManager.onPlayerSpawned -= SetCameraTarget;
    }

    // Camera target setting method
    public void SetCameraTarget(Transform newTarget)
    {
        if (playerCam != null && playerCam.Follow != newTarget) playerCam.Follow = newTarget;
    }
}