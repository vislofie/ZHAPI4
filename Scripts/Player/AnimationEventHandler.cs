using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    private WeaponManager _weaponManager;

    private void Awake()
    {
        _weaponManager = transform.parent.GetComponent<WeaponManager>();
    }

    public void DrawRevoler()
    {
        _weaponManager.Draw();
    }

    public void ActivateRevoler()
    {
        _weaponManager.ActivateWeapon();
    }

    public void SheathRevoler()
    {
        _weaponManager.Sheath();
    }

    public void DeactivateRevolver()
    {
        _weaponManager.DeactivateWeapon();
    }

    public void PutInHand()
    {
        _weaponManager.PutInHand();
    }
}
