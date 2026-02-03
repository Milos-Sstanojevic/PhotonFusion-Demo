using Fusion;
using UnityEngine;

public sealed class Weapon : MonoBehaviour
{
    private Player _player;
    private Transform _tr;
    private float _pitch;
    private float _startRotationX;

    public void Awake()
    {
        _tr = transform;
        _startRotationX = _tr.localRotation.eulerAngles.x;
        _player = GetComponentInParent<Player>();
    }

    public void Update()
    {
        _tr.localRotation=Quaternion.Euler(_startRotationX+_player.Pitch,0,0);
    }
}