using ItemSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemHotbar : MonoBehaviour
{
    // Item Hotbar Variables
    private int _selectedItemIndex = 0;
    private GameObject[] _itemHotbar = new GameObject[4];

    [SerializeField] private GameObject HotbarContainer;
    private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject Item1Icon;
    [SerializeField] private GameObject Item2Icon;
    [SerializeField] private GameObject Item3Icon;
    [SerializeField] private GameObject Item4Icon;

    private float _fadeDuration = 1.0f;
    private Coroutine _fadeCoroutine;

    [Header("Watch UI")]
    [SerializeField] public GameObject WatchUI;
    [SerializeField] public GameObject TimeUI;
    public bool _watchActive = false;

    public void Start()
    {
        _canvasGroup = HotbarContainer.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;

        WatchUI.SetActive(_watchActive);
        TimeUI.SetActive(_watchActive);
    }

    /// <summary>
    /// Adds the highlighted interactable object to the player's item hotbar
    /// </summary>
    /// <param name="item">Item Gameobject</param>
    public void AddItem(GameObject item)
    {
        if (_itemHotbar[_selectedItemIndex] != null)
        {
            return;
        }

        _itemHotbar[_selectedItemIndex] = item;
        item.GetComponent<ItemInstance>().AttachToParent(this.gameObject);

        UpdateHotbarItemIcon();

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeSequence());
    }

    /// <summary>
    /// Use currently selected item
    /// </summary>
    public void UseItem()
    {
        if (_itemHotbar[_selectedItemIndex] != null)
        {
            _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().Use(this.gameObject);
        }
    }

    /// <summary>
    /// Drops currently selected item
    /// </summary>
    public void DropItem()
    {
        if (_itemHotbar[_selectedItemIndex] != null)
        {
            _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().DetachFromParent(this.gameObject);
            _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().EnableRigidBodyCollisions();

            if (_itemHotbar[_selectedItemIndex].GetComponent<Watch>())
            {
                ToggleWatchDisplay(_itemHotbar[_selectedItemIndex].GetComponent<Watch>()._rendererComponent);
            }

            _itemHotbar[_selectedItemIndex] = null;

            RemoveHotbarItemIcon();

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            _fadeCoroutine = StartCoroutine(FadeSequence());
        }
    }

    /// <summary>
    /// Updates the item hotbar icon with currently selected item's icon 
    /// </summary>
    private void UpdateHotbarItemIcon()
    {
        switch (_selectedItemIndex)
        {
            case 1:
                Item2Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item2Icon.GetComponent<RawImage>().color = Color.yellow;
                break;
            case 2:
                Item3Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item3Icon.GetComponent<RawImage>().color = Color.yellow;
                break;
            case 3:
                Item4Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item4Icon.GetComponent<RawImage>().color = Color.yellow;
                break;
            default:
                Item1Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item1Icon.GetComponent<RawImage>().color = Color.yellow;
                break;
        }
    }

    /// <summary>
    /// Resets previously selected item hotbar slot only if it does not hold an item
    /// </summary>
    public void ResetPreviousEmptySlot()
    {
        if (_itemHotbar[_selectedItemIndex] == null)
        {
            Color resetColor = new Color(0, 0, 0, 0.5f);

            switch (_selectedItemIndex)
            {
                case 1:
                    Item2Icon.GetComponent<RawImage>().color = resetColor;
                    break;
                case 2:
                    Item3Icon.GetComponent<RawImage>().color = resetColor;
                    break;
                case 3:
                    Item4Icon.GetComponent<RawImage>().color = resetColor;
                    break;
                default:
                    Item1Icon.GetComponent<RawImage>().color = resetColor;
                    break;
            }
        }
    }
    
    /// <summary>
    /// Removes the item hotbar icon (when player drops item)
    /// </summary>
    private void RemoveHotbarItemIcon()
    {
        Color resetColor = new Color(0, 0, 0, 0.5f);

        switch (_selectedItemIndex)
        {
            case 1:
                Item2Icon.GetComponent<RawImage>().texture = null;
                Item2Icon.GetComponent<RawImage>().color = resetColor;
                break;
            case 2:
                Item3Icon.GetComponent<RawImage>().texture = null;
                Item3Icon.GetComponent<RawImage>().color = resetColor;
                break;
            case 3:
                Item4Icon.GetComponent<RawImage>().texture = null;
                Item4Icon.GetComponent<RawImage>().color = resetColor;
                break;
            default:
                Item1Icon.GetComponent<RawImage>().texture = null;
                Item1Icon.GetComponent<RawImage>().color = resetColor;
                break;
        }
    }

    /// <summary>
    /// Fade In and Fade Out animation sequence for item hotbar UI
    /// </summary>
    /// <returns>Coroutine</returns>
    public IEnumerator FadeSequence()
    {
        // Fade In
        yield return StartCoroutine(DoFade(_canvasGroup.alpha, 1));

        // Stay Visible
        yield return new WaitForSeconds(1);

        // Fade Out
        yield return StartCoroutine(DoFade(_canvasGroup.alpha, 0));
    }

    /// <summary>
    /// Linearly interpolate between a start and end alpha value for the fade effect
    /// </summary>
    /// <param name="startAlpha">Start Alpha Value</param>
    /// <param name="endAlpha">End Alpha Value</param>
    /// <returns></returns>
    private IEnumerator DoFade(float startAlpha, float endAlpha)
    {
        float timer = 0;

        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = endAlpha;
    }

    /// <summary>
    /// Checks if currently selected item hotbar slot holds an item
    /// </summary>
    /// <returns></returns>
    public bool SlotHasItem()
    {
        return _itemHotbar[_selectedItemIndex] != null;
    }

    /// <summary>
    /// Unequips current item if we have one, changes the selected item index, and equips the new item if we have one
    /// If we do not have items, reset the UI representations
    /// Also play our fade in/fade out effect on item hotbar
    /// </summary>
    /// <param name="i">Index of Selected Item</param>
    public void SwitchToItem(int i)
    {
        if (_itemHotbar[_selectedItemIndex])
        {
            _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().UnEquip();
        }
        else
        {
            ResetPreviousEmptySlot();
        }

        _selectedItemIndex = i;

        if (_itemHotbar[_selectedItemIndex])
        {
            _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().Equip();
        }
        else
        {
            switch (i)
            {
                case 0:
                    Item1Icon.GetComponent<RawImage>().color = Color.red;
                    break;
                case 1:
                    Item2Icon.GetComponent<RawImage>().color = Color.red;
                    break;
                case 2:
                    Item3Icon.GetComponent<RawImage>().color = Color.red;
                    break;
                default:
                    Item4Icon.GetComponent<RawImage>().color = Color.red;
                    break;
            }
            
        }

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeSequence());
    }

    /// <summary>
    /// Toggles the watch's display with its renderer component
    /// </summary>
    /// <param name="r">Watch's Renderer Component</param>
    public void ToggleWatchDisplay(Renderer r)
    {
        if (r.enabled)
        {
            _watchActive = !_watchActive;
        }
        else
        {
            _watchActive = false;
        }

        WatchUI.SetActive(_watchActive);
        TimeUI.SetActive(_watchActive);
    }

    /// <summary>
    /// Switches to the next item. Called multiple times while scrolling 
    /// </summary>
    public void ScrollUp()
    {
        if (_selectedItemIndex + 1 < 4)
        {
            SwitchToItem(_selectedItemIndex + 1);
        }
        else
        {
            SwitchToItem(0);
        }
    }

    /// <summary>
    /// Switches to the previous item. Called multiple times while scrolling 
    /// </summary>
    public void ScrollDown()
    {
        if (_selectedItemIndex - 1 > -1)
        {
            SwitchToItem(_selectedItemIndex - 1);
        }
        else
        {
            SwitchToItem(_itemHotbar.Length - 1);
        }
    }
}
