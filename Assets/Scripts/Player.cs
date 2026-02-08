using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed=5;
    [SerializeField] private float jumpForce = 10;
    [SerializeField] private Camera playerLocalCamera;
    [SerializeField] private Weapon weapon;
    [Networked] public float Pitch { get; private set;}
    private NetworkCharacterControllerFPS _characterController;
    private Transform _tr;
    private Transform _playerLocalCameraTr;
    private float _yaw;
    private bool _jumpWasPressed;
    private bool _fireWasPressed;

    public override void Spawned()
    {
        Cursor.lockState=CursorLockMode.Locked;
        Cursor.visible=false;
        
        _tr = transform;
        _characterController = GetComponent<NetworkCharacterControllerFPS>();
        
        if (Object.HasInputAuthority)
            return;
        playerLocalCamera.enabled = false;
        var audioListener=GetComponentInChildren<AudioListener>();
        audioListener.enabled=false;
    }

    public override void FixedUpdateNetwork()
    {
        if (_tr == null) return;
        if (!GetInput(out NetworkInputData data)) return;
        Vector3 moveDirection = _tr.forward * data.movementInput.y + _tr.right * data.movementInput.x;
        moveDirection.Normalize();
        _characterController.Move(moveDirection);
        
        if(Object.HasInputAuthority)
            _playerLocalCameraTr.localRotation=Quaternion.Euler(Pitch,0,0);
        Pitch+= data.rotationInput.y * _characterController.rotationSpeed * Runner.DeltaTime;
        Pitch = Mathf.Clamp(Pitch, -60, 60); 
        
        if (!Object.HasStateAuthority) return;
        
        _yaw += data.rotationInput.x * _characterController.rotationSpeed * Runner.DeltaTime;
        _tr.rotation = Quaternion.Euler(0, _yaw, 0);
        
        bool jumpPressed = data.isJumpPressed && !_jumpWasPressed;
        _jumpWasPressed = data.isJumpPressed;
        if (jumpPressed)
            _characterController.Jump();
    }
    
    private void Awake()
    {
        _playerLocalCameraTr=playerLocalCamera.transform;
    }
    
    
}