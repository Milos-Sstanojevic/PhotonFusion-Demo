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
    private float _lastTimeFired;
    [HideInInspector][Networked] public NetworkBool IsFiring { get; set; }
    private ChangeDetector _changeDetector;
    

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }
    
    public override void Render()
    {
        if(_changeDetector.DetectChanges(this).Changed("IsFiring"))
            OnFireChanged();
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
        Runner.LagCompensation.Raycast(aimPoint.position, aimPoint.forward, 
            100,Object.InputAuthority, out var hitInfo, playerLayers, HitOptions.IncludePhysX);

        if (hitInfo.Hitbox != null && Object.HasStateAuthority)
        {
            var player=hitInfo.Hitbox.transform.root.GetComponent<HPHandler>();
            if(player!=null) player.TakeDamage();
        }
        
        _lastTimeFired = Time.time;
    }

    private IEnumerator FireEffectCo()
    {
        IsFiring = true;
        firePfx.Play();
        yield return new WaitForSeconds(0.09f);
        IsFiring = false;      
    }
}
