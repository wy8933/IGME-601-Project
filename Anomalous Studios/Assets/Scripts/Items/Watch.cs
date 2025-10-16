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
    private Transform _cameraTransform;
    private Vector3 _itemCamPosOffset = new Vector3(0.3f, -0.3f, 0.3f);
    private float _dropDistanceOffset = 1.5f;
    private Rigidbody _rb;
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

        _playerController.GetItemHotbar().ToggleWatchDisplay(_rendererComponent);
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
        _cameraTransform = parent.transform.GetChild(1).transform.GetChild(0).transform;

        PickUp();
        DisableRigidBodyCollisions();

        this.gameObject.transform.SetParent(_cameraTransform, false);
        transform.localPosition = _itemCamPosOffset;
        transform.localRotation = Quaternion.Euler(90, 0, 0);
    }

    public override void DetachFromParent(GameObject parent)
    {
        Vector3 newPos = parent.transform.position + parent.transform.forward * _dropDistanceOffset;
        transform.position = newPos;
        this.gameObject.transform.parent = null;
        CanInteract = true;
        _pickedUp = false;
    }

    public override void DisableRigidBodyCollisions()
    {
        _sphereCollider.enabled = false;
        _rb.isKinematic = true;
        _rb.detectCollisions = false;
        _rb.useGravity = false;
    }

    public override void EnableRigidBodyCollisions()
    {
        _sphereCollider.enabled = true;
        _rb.isKinematic = false;
        _rb.detectCollisions = true;
        _rb.useGravity = true;
    }
}
