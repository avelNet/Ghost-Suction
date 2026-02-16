using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private FPSController _playerController;

    private void LateUpdate()
    {
        if (_target != null)
        {
            transform.position = _target.position;

            float playerAngleY = _target.eulerAngles.y;
            float cameraAngleX = _playerController.GetRotationX();

            transform.rotation = Quaternion.Euler(cameraAngleX, playerAngleY, 0);
        }
    }
}
