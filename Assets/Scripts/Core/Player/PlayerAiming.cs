using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private Transform turretTransform;
    [SerializeField] private InputReader inputReader;

    private void LateUpdate()
    {
        if (!IsOwner) return;

        Vector2 aimingScreenPosition = inputReader.AimPosition;
        Vector2 aimingWorldPosition = Camera.main.ScreenToWorldPoint(aimingScreenPosition);

        turretTransform.up = new Vector2(
                aimingWorldPosition.x - turretTransform.position.x,
                aimingWorldPosition.y - turretTransform.position.y
            );
    }
}
