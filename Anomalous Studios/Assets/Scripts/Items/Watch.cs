using AudioSystem;
using ItemSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Watch : ItemInstance
{
    private SphereCollider _sphereCollider;
    [SerializeField] GameObject _player;
    private PlayerController _playerController;
    private Text _watchText;
    public Renderer _rendererComponent;

    private float _timer = 0;
    private float _tickInterval = 1.0f;

    [Header("Reaction SFX")]
    [SerializeField] private SoundDataSO _failedSFX;
    [SerializeField] private SoundDataSO _successSFX;
    public override SoundDataSO InitialSFX => null;
    public override SoundDataSO FailedSFX { get => _failedSFX; }
    public override SoundDataSO CancelSFX => null;
    public override SoundDataSO SuccessSFX { get => _successSFX; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();

        _rb = GetComponent<Rigidbody>();
        _sphereCollider = GetComponent<SphereCollider>();
        _rendererComponent = GetComponent<Renderer>();

        _player = GameObject.FindGameObjectWithTag("Player");

        if (_player)
        {
            _playerController = _player.GetComponent<PlayerController>();
            if (_playerController != null)
            {
                _watchText = _playerController.GetItemHotbar().TimeUI.GetComponent<Text>();
            }
        }

        StartCoroutine(UpdateTimer());
    }

    public override void Use(GameObject user)
    {
        TryUse(user);

        if (_isEquipped)
        {
            _playerController.GetItemHotbar().ToggleWatchDisplay(_rendererComponent);
        }
    }

    public IEnumerator UpdateTimer()
    {
        _timer++;

        if(_timer >= 1440)
        {
            _timer = 0;
        }

        if (_watchText)
        {
            int hours = (int)(_timer / 60);
            int minutes = (int)(_timer % 60);
            VariableConditionManager.Instance.Set("watchTimer", _timer.ToString("0.00"));
            _watchText.text = string.Format("{0:00}:{1:00}", hours, minutes);
        }

        yield return new WaitForSeconds(_tickInterval);
        
        StartCoroutine(UpdateTimer());
    }

    public override void Interact()
    {
        if (IInteractable.Instigator != null)
        {
            IInteractable.Instigator.GetComponent<PlayerController>().GetItemHotbar().AddItem(this.gameObject);
            _playerController.GetItemHotbar().ToggleWatchDisplay(_rendererComponent);
        }
    }

    public override void Equip()
    {
        _isEquipped = true;
        _rendererComponent.enabled = true;
        _playerController.GetItemHotbar().ToggleWatchDisplay(_rendererComponent);
    }

    public override void UnEquip()
    {
        _isEquipped = false;
        _rendererComponent.enabled = false;
        _playerController.GetItemHotbar().ToggleWatchDisplay(_rendererComponent);   
    }

    public override void AttachToParent(GameObject parent)
    {
        base.AttachToParent(parent);
        DisableRigidBodyCollisions();
    }

    public override void DetachFromParent(GameObject parent)
    {
        base.DetachFromParent(parent);
    }

    public override void DisableRigidBodyCollisions()
    {
        base.DisableRigidBodyCollisions();
        _sphereCollider.enabled = false;
    }

    public override void EnableRigidBodyCollisions()
    {
        base.EnableRigidBodyCollisions();
        _sphereCollider.enabled = true;
    }
}
