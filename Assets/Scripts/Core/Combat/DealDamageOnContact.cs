using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damge = 5;

    private ulong ownerClientId;

    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SoundManager.instance.PlaySoundExplosion(gameObject.transform.position);

        if (collision.attachedRigidbody == null) return;
        
        if (collision.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject obj))
        {
            if (obj.OwnerClientId == ownerClientId)
            {
                return;
            }
        }

        if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamge(damge);
        }
    }
}
