using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CoinWallet coinWallet;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playreCollider;

    [Header("Setting")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private int costToFire;
    [SerializeField] private float muzzleFlashDuration;

    //public event Action<Vector3> OnSpawnProjectile;

    private bool shouldFire;
    private float timer;
    private float muzzleFlashTimer;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.PrimaryFireEvent += InputReader_PrimaryFireEvent;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        inputReader.PrimaryFireEvent -= InputReader_PrimaryFireEvent;
    }

    private void Update()
    {
        if (muzzleFlashTimer > 0)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner) return;

        //cooldown shoot
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (!shouldFire) return;

        if (timer > 0) return;

        if (coinWallet.TotalCoins.Value < costToFire) return;

        //Spawn Bullet when cooldown done and have enough coin
        ProjectileFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        timer = 1/fireRate;
    }

    [ServerRpc]
    private void ProjectileFireServerRpc(Vector3 positionSpawn, Vector3 direction)
    {

        if (coinWallet.TotalCoins.Value < costToFire) return;

        coinWallet.SpendCoin(costToFire);

        GameObject projectileInstance = Instantiate(serverProjectilePrefab, positionSpawn, Quaternion.identity);
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playreCollider, projectileInstance.GetComponent<Collider2D>());


        if(projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact damageOnContact))
        {
            damageOnContact.SetOwner(OwnerClientId);
        }

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }
        SpawnDummyProjectileClientRpc(positionSpawn, direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 positionSpawn, Vector3 direction)
    {
        if (IsOwner) return;

        SpawnDummyProjectile(positionSpawn, direction);
    }

    private void SpawnDummyProjectile(Vector3 positionSpawn, Vector3 direction)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        GameObject projectileInstance = Instantiate(clientProjectilePrefab, positionSpawn, Quaternion.identity);
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playreCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }

        SoundManager.instance.PlaySoundShoot(positionSpawn);
    }

    private void InputReader_PrimaryFireEvent(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }
}
