﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class stores information on obtained guns and the currently equipped gun.
/// It also reads input for firing the guns
/// </summary>
public class Shooter : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The list of guns this shooter can potentially have access to")]
    public List<Gun> guns = new List<Gun>();
    [Tooltip("The index in the available gun list of the gun currently equipped")]
    public int equippedGunIndex = 0;
    [Tooltip("Whether or not this shooter is controlled by the player")]
    public bool isPlayerControlled = false;

    [Header("Input Actions")]
    [Tooltip("The binding for firing the shooter")]
    public InputAction fireInput;
    [Tooltip("The binding to scroll through weapons")]
    public InputAction cycleWeaponInput;
    [Tooltip("The binding for going to next weapon")]
    public InputAction nextWeaponInput;
    [Tooltip("The binding for going to the previous weapon")]
    public InputAction previousWeaponInput;

    /// <summary>
    /// Standard Unity function called whenever the attached gameobject is enabled
    /// </summary>
    private void OnEnable()
    {
        fireInput.Enable();
        cycleWeaponInput.Enable();
        nextWeaponInput.Enable();
        previousWeaponInput.Enable();
    }

    /// <summary>
    /// Standard Unity function called whenever the attached gameobject is disabled
    /// </summary>
    private void OnDisable()
    {
        fireInput.Disable();
        cycleWeaponInput.Disable();
        nextWeaponInput.Disable();
        previousWeaponInput.Disable();
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once before the first update
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void Start()
    {
        SetUpGuns();
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once per frame
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void Update()
    {
        CheckInput();
    }

    /// <summary>
    /// Description:
    /// Checks input and responds accordingly
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void CheckInput()
    {
        if (!isPlayerControlled)
        {
            return;
        }

        // Do nothing when paused
        if (Time.timeScale == 0)
        {
            return;
        }

        if (guns.Count > 0)
        {
            if (guns[equippedGunIndex].fireType == Gun.FireType.semiAutomatic)
            {
                if (fireInput.triggered)
                {
                    FireEquippedGun();
                }
            }
            else if (guns[equippedGunIndex].fireType == Gun.FireType.automatic)
            {
                if (fireInput.triggered || fireInput.ReadValue<float>() >= 1)
                {
                    FireEquippedGun();
                }
            }

            if (cycleWeaponInput.ReadValue<Vector2>().y != 0)
            {
                CycleEquippedGun();
            }

            if (nextWeaponInput.triggered)
            {
                GoToNextWeapon();
            }

            if (previousWeaponInput.triggered)
            {
                GoToPreviousWeapon();
            }
        }
    }

    /// <summary>
    /// Description:
    /// Goes to the next weapon of the available weapons
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    public void GoToNextWeapon()
    {
        List<Gun> availableGuns = guns.Where(item => item.available == true).ToList();
        int maximumAvailableGunIndex = availableGuns.Count - 1;
        int equippedAvailableGunIndex = availableGuns.IndexOf(guns[equippedGunIndex]);

        equippedAvailableGunIndex += 1;
        if (equippedAvailableGunIndex > maximumAvailableGunIndex)
        {
            equippedAvailableGunIndex = 0;
        }

        EquipGun(guns.IndexOf(availableGuns[equippedAvailableGunIndex]));
    }

    /// <summary>
    /// Description:
    /// Goes to the previous weapon of the available weapons
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    public void GoToPreviousWeapon()
    {
        List<Gun> availableGuns = guns.Where(item => item.available == true).ToList();
        int maximumAvailableGunIndex = availableGuns.Count - 1;
        int equippedAvailableGunIndex = availableGuns.IndexOf(guns[equippedGunIndex]);

        equippedAvailableGunIndex -= 1;
        if (equippedAvailableGunIndex < 0)
        {
            equippedAvailableGunIndex = maximumAvailableGunIndex;
        }

        EquipGun(guns.IndexOf(availableGuns[equippedAvailableGunIndex]));
    }

    /// <summary>
    /// Description:
    /// Cycles through the available guns starting from the currently equipped gun
    /// and moving in the direction of the mouse scroll input.
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void CycleEquippedGun()
    {
        float cycleInput = cycleWeaponInput.ReadValue<Vector2>().y;
        List<Gun> availableGuns = guns.Where(item => item.available == true).ToList();
        int maximumAvailableGunIndex = availableGuns.Count - 1;
        int equippedAvailableGunIndex = availableGuns.IndexOf(guns[equippedGunIndex]);
        if (cycleInput < 0)
        {
            equippedAvailableGunIndex += 1;
            if (equippedAvailableGunIndex > maximumAvailableGunIndex)
            {
                equippedAvailableGunIndex = 0;
            }
        }
        else if (cycleInput > 0)
        {
            equippedAvailableGunIndex -= 1;
            if (equippedAvailableGunIndex < 0)
            {
                equippedAvailableGunIndex = maximumAvailableGunIndex;
            }
        }

        EquipGun(guns.IndexOf(availableGuns[equippedAvailableGunIndex]));
    }

    /// <summary>
    /// Description:
    /// Equips the gun from the list of guns at the given index
    /// Input:
    /// int gunIndex
    /// Return:
    /// void (no return)
    /// </summary>
    /// <param name="gunIndex">The index of the gun to make the equipped gun</param>
    public void EquipGun(int gunIndex)
    {
        equippedGunIndex = gunIndex;
        guns[equippedGunIndex].gameObject.SetActive(true);
        for (int i = 0; i < guns.Count; i++)
        {
            if (equippedGunIndex != i)
            {
                guns[i].gameObject.SetActive(false);
            }
        }
        GameManager.UpdateUIElements();
    }

    /// <summary>
    /// Description:
    /// Sets up the available guns for use
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void SetUpGuns()
    {
        foreach(Gun gun in guns)
        {
            if (gun != null)
            {
                if (gun.available && guns[equippedGunIndex] == gun)
                {
                    gun.gameObject.SetActive(true);
                }
                else
                {
                    gun.gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Description:
    /// Attempts to fire the equipped gun. Fails if the gun is not available
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    public void FireEquippedGun()
    {
        if (guns[equippedGunIndex] != null && guns[equippedGunIndex].available)
        {
            guns[equippedGunIndex].Fire();
        }   
    }

    /// <summary>
    /// Description:
    /// Makes a gun available to the player
    /// Input: 
    /// int gunIndex
    /// Return:
    /// void (no return)
    /// </summary>
    /// <param name="gunIndex">The index of the gun to make available</param>
    public void MakeGunAvailable(int gunIndex)
    {
        if (gunIndex < guns.Count && guns[gunIndex] != null && guns[gunIndex].available == false)
        {
            guns[gunIndex].available = true;
            EquipGun(gunIndex);
        }
    }
}
