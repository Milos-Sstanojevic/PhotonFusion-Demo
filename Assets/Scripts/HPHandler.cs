using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class HPHandler : NetworkBehaviour
{
      private const int startHP = 10;
      [SerializeField] private MeshRenderer meshRenderer;
      [SerializeField] private Image gotHitImage;
      [SerializeField] private GameObject playerModel;
      [HideInInspector][Networked,OnChangedRender(nameof(OnHPChange))] public int HP { get; set; }
      [HideInInspector][Networked,OnChangedRender(nameof(OnStateChanged))] public bool IsDead { get; set; }

      private bool isInitialized = false;
      private Color _defaultColor;
      private HitboxRoot hitboxRoot;
      
      private void Start()
      {
            HP = startHP;
            IsDead=false;

            isInitialized = true;
            _defaultColor=meshRenderer.material.color;
      }

      public void TakeDamage()
      {
            if (IsDead) return;

            HP -= 1;
            if(HP<=0) IsDead=true;
      }

      private IEnumerator ShowHit()
      {
            meshRenderer.material.color = Color.white;
            if(Object.HasInputAuthority)
                  gotHitImage.enabled = true;
            yield return new WaitForSeconds(0.2f);
            meshRenderer.material.color = _defaultColor;
            if(Object.HasInputAuthority && !IsDead)
                  gotHitImage.enabled = false;
      }

      public void OnHPChange()
      {
            if (!isInitialized) return;
            StartCoroutine(ShowHit());
      }
      
      public void OnStateChanged()
      {
            if (IsDead)
                  RpcOnDeath();
      }

      [Rpc(RpcSources.InputAuthority,RpcTargets.All)]
      public void RpcOnDeath()
      {
            Destroy(gameObject);
      }
      
}
