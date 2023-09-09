using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] [Range(.1f, 10f)] private float moveSpeed = 5f;
    [SerializeField] [Range(10f, 100f)] private float rotationSpeed = 50f;
    [SerializeField] [Range(1f, 3f)] private float shiftSpeedBoost = 2f;
    [SerializeField] [Range(.1f, 3f)] private float zoomSpeed = 1f;

    [SerializeField] private CinemachineVirtualCamera playerCamera;
    CinemachineTransposer cinemachineTransposer;
    private const float MIN_ZOOM_Y_OFFSET = .5f;
    private const float MAX_ZOOM_Y_OFFSET = 5f;
    private Vector3 targetFollowOffset;

    private void Start() 
    {
        cinemachineTransposer = playerCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector3 inputMoveDir = new Vector3(0, 0, 0);
        if(Input.GetKey(KeyCode.W))
        {
            inputMoveDir.z = +1f;
        }
        if(Input.GetKey(KeyCode.S))
        {
            inputMoveDir.z = -1f;
        }
        if(Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x = -1f;
        }
        if(Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x = +1f;
        }
        Vector3 moveVector = transform.forward * inputMoveDir.z + transform.right * inputMoveDir.x;
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? this.moveSpeed*shiftSpeedBoost : this.moveSpeed;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);
        if(Input.GetKey(KeyCode.Q))
        {
            rotationVector.y = -1f;
        }
        if(Input.GetKey(KeyCode.E))
        {
            rotationVector.y = +1f;
        }
        float rotationSpeed = Input.GetKey(KeyCode.LeftShift) ? this.rotationSpeed*shiftSpeedBoost : this.rotationSpeed;
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float zoomAmount = .5f;
        if(Input.mouseScrollDelta.y > 0)
        {
            targetFollowOffset.y -= zoomAmount;
        }
        if(Input.mouseScrollDelta.y < 0)
        {
            targetFollowOffset.y += zoomAmount;
        }
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_ZOOM_Y_OFFSET, MAX_ZOOM_Y_OFFSET);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }
}
