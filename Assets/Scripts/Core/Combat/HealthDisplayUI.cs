using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplayUI : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private Image imageHealth;

    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;

        health.CurrentHealth.OnValueChanged += HandleHealthChanged;
        HandleHealthChanged(0, health.CurrentHealth.Value);
        
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;
        health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int previousValue, int newValue)
    {
        imageHealth.fillAmount = (float)newValue/health.MaxHealth;
    }
}
