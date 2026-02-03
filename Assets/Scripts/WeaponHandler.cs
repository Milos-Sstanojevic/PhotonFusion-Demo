using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponHandler : NetworkBehaviour
{
    [SerializeField] private ParticleSystem firePfx;
    [SerializeField] private LayerMask playerLayers;
    [SerializeField] private Transform aimPoint;
    [SerializeField] private HPHandler hpHandler;
    [HideInInspector][Networked,OnChangedRender(nameof(OnFireChanged))] public bool IsFiring { get; set; }

    private float _lastTimeFired = 0;
    
    public override void FixedUpdateNetwork()
    {
        if(hpHandler.IsDead) return;
        if(!GetInput(out NetworkInputData data)) return;

        if (data.isFirePressed)
            Fire();
    }

    private void Fire()
    {
        if (Time.time - _lastTimeFired < 0.15f)
            return;

        StartCoroutine(FireEffectCo());
        Runner.LagCompensation.Raycast(aimPoint.position, aimPoint.forward, 100,Object.InputAuthority, out var hitInfo, playerLayers, HitOptions.IncludePhysX);

        if (hitInfo.Hitbox != null && Object.HasStateAuthority)
        {
            var player=hitInfo.Hitbox.transform.root.GetComponent<HPHandler>();
            if(player!=null) player.TakeDamage();
        }
        
        _lastTimeFired = Time.time;
    }

    private IEnumerator FireEffectCo()
    {
        IsFiring = true;        // networked property can only be changed by state authority, even if client changes it, server will override it
        firePfx.Play();
        yield return new WaitForSeconds(0.09f);
        IsFiring = false;      
    }

    private void OnFireChanged()
    {
        if (IsFiring)
            OnFireRemote();
    }

    private void OnFireRemote()
    {
        if(!Object.HasInputAuthority)
            firePfx.Play();
    }
}
