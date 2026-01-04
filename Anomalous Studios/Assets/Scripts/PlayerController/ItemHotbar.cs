using ItemSystem;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemHotbar : MonoBehaviour
{
    // Item Hotbar Variables
    private int _selectedItemIndex = 0;
    private GameObject[] _itemHotbar = new GameObject[4];

    [Header("Hotbar Container UI Gameobject")]
    [SerializeField] private GameObject HotbarContainer;
    private CanvasGroup _canvasGroup;
    [Header("Item UI Gameobjects")]
    [SerializeField] private GameObject Item1Icon;
    [SerializeField] private GameObject Item2Icon;
    [SerializeField] private GameObject Item3Icon;
    [SerializeField] private GameObject Item4Icon;

    [SerializeField] private GameObject Item1BG;
    [SerializeField] private GameObject Item2BG;
    [SerializeField] private GameObject Item3BG;
    [SerializeField] private GameObject Item4BG;

    [SerializeField] private GameObject Item1Text;
    [SerializeField] private GameObject Item2Text;
    [SerializeField] private GameObject Item3Text;
    [SerializeField] private GameObject Item4Text;

    public GameObject GetItem1Text() { return Item1Text; }
    public GameObject GetItem2Text() { return Item2Text; }
    public GameObject GetItem3Text() { return Item3Text; }
    public GameObject GetItem4Text() { return Item4Text; }


    [SerializeField] private Texture placeholderImage;
    private float _fadeDuration = 1.0f;
    private Coroutine _fadeCoroutine;

    [Header("Watch UI Gameobjects")]
    public GameObject WatchUI;
    public GameObject TimeUI;
    public bool _watchActive = false;

    // Getter Methods
    public GameObject[] GetHotbar() { return  _itemHotbar; }
    public int GetSelectedItemIndex() { return _selectedItemIndex; }

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
        //item.GetComponent<ItemInstance>().CanInteract = false;

        if (_itemHotbar[_selectedItemIndex] != null)
        {
            int i = CheckAvailableItemSlots();

            if (i == _selectedItemIndex)
            {
                return;
            }
            else
            {
                SwitchToItem(i);
            }
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
            //_itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().CanInteract = true;

            if (_itemHotbar[_selectedItemIndex].GetComponent<Watch>())
            {
                if (WatchUI.activeSelf)
                {
                    ToggleWatchDisplay(_itemHotbar[_selectedItemIndex].GetComponent<Watch>()._rendererComponent);
                }
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
    /// Removes one-time item from hotbar and plays fade in/out effect when item is used
    /// </summary>
    public void OnUsed()
    {
        _itemHotbar[_selectedItemIndex] = null;

        RemoveHotbarItemIcon();

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeSequence());
    }

    /// <summary>
    /// Updates the item hotbar icon with currently selected item's icon 
    /// </summary>
    private void UpdateHotbarItemIcon()
    {
        Color bgColor = new Color(255, 0, 0, 1f);
        switch (_selectedItemIndex)
        {
            case 1:
                Item2Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item2Icon.GetComponent<RawImage>().color = Color.white;
                
                Item2BG.GetComponent<RawImage>().color = bgColor;
                if (_itemHotbar[_selectedItemIndex].GetComponent<Key>())
                {
                    Item2Text.GetComponent<TextMeshProUGUI>().text = _itemHotbar[_selectedItemIndex].GetComponent<Key>().GetKeyID();
                    Item2Text.GetComponent<TextMeshProUGUI>().color = Color.white;
                }
                break;
            case 2:
                Item3Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item3Icon.GetComponent<RawImage>().color = Color.white;
                Item3BG.GetComponent<RawImage>().color = bgColor;
                if (_itemHotbar[_selectedItemIndex].GetComponent<Key>()){
                    Item3Text.GetComponent<TextMeshProUGUI>().text = _itemHotbar[_selectedItemIndex].GetComponent<Key>().GetKeyID();
                    Item3Text.GetComponent<TextMeshProUGUI>().color = Color.white;
                }
                break;
            case 3:
                Item4Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item4Icon.GetComponent<RawImage>().color = Color.white;
                Item4BG.GetComponent<RawImage>().color = bgColor;
                if (_itemHotbar[_selectedItemIndex].GetComponent<Key>())
                {
                    Item4Text.GetComponent<TextMeshProUGUI>().text = _itemHotbar[_selectedItemIndex].GetComponent<Key>().GetKeyID();
                    Item4Text.GetComponent<TextMeshProUGUI>().color = Color.white;
                }
                break;
            default:
                Item1Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item1Icon.GetComponent<RawImage>().color = Color.white;
                Item1BG.GetComponent<RawImage>().color = bgColor;
                if (_itemHotbar[_selectedItemIndex].GetComponent<Key>())
                {
                    Item1Text.GetComponent<TextMeshProUGUI>().text = _itemHotbar[_selectedItemIndex].GetComponent<Key>().GetKeyID();
                    Item1Text.GetComponent<TextMeshProUGUI>().color = Color.white;
                }
                break;
        }
    }

    /// <summary>
    /// Resets previously selected item hotbar slot only if it does not hold an item
    /// </summary>
    public void ResetPreviousEmptySlot()
    {
        Color bgColor = new Color(0, 0, 0, 0.5f);

        if (_itemHotbar[_selectedItemIndex] == null)
        {
            Color resetColor = new Color(0, 0, 0, 0.5f);

            switch (_selectedItemIndex)
            {
                case 1:
                    Item2Icon.GetComponent<RawImage>().color = resetColor;
                    Item2Text.GetComponent<TextMeshProUGUI>().text = "";
                    Item2BG.GetComponent<RawImage>().color = bgColor;
                    break;
                case 2:
                    Item3Icon.GetComponent<RawImage>().color = resetColor;
                    Item3Text.GetComponent<TextMeshProUGUI>().text = "";
                    Item3BG.GetComponent<RawImage>().color = bgColor;
                    break;
                case 3:
                    Item4Icon.GetComponent<RawImage>().color = resetColor;
                    Item4Text.GetComponent<TextMeshProUGUI>().text = "";
                    Item4BG.GetComponent<RawImage>().color = bgColor;
                    break;
                default:
                    Item1Icon.GetComponent<RawImage>().color = resetColor;
                    Item1Text.GetComponent<TextMeshProUGUI>().text = "";
                    Item1BG.GetComponent<RawImage>().color = bgColor;
                    break;
            }
        }

        switch (_selectedItemIndex)
        {
            case 1:
                Item2BG.GetComponent<RawImage>().color = bgColor;
                break;
            case 2:
                Item3BG.GetComponent<RawImage>().color = bgColor;
                break;
            case 3:
                Item4BG.GetComponent<RawImage>().color = bgColor;
                break;
            default:
                Item1BG.GetComponent<RawImage>().color = bgColor;
                break;
        }
        
    }
    
    /// <summary>
    /// Removes the item hotbar icon (when player drops item)
    /// </summary>
    private void RemoveHotbarItemIcon()
    {
        Color resetColor = new Color(0, 0, 0, 0.5f);
        Color bgColor = new Color(0, 0, 0, 0.5f);

        switch (_selectedItemIndex)
        {
            case 1:
                Item2Icon.GetComponent<RawImage>().texture = null;
                Item2Icon.GetComponent<RawImage>().color = resetColor;
                Item2Text.GetComponent<TextMeshProUGUI>().text = "";
                Item2BG.GetComponent<RawImage>().color = bgColor;
                break;
            case 2:
                Item3Icon.GetComponent<RawImage>().texture = null;
                Item3Icon.GetComponent<RawImage>().color = resetColor;
                Item3Text.GetComponent<TextMeshProUGUI>().text = "";
                Item3BG.GetComponent<RawImage>().color = bgColor;
                break;
            case 3:
                Item4Icon.GetComponent<RawImage>().texture = null;
                Item4Icon.GetComponent<RawImage>().color = resetColor;
                Item4Text.GetComponent<TextMeshProUGUI>().text = "";
                Item4BG.GetComponent<RawImage>().color = bgColor;
                break;
            default:
                Item1Icon.GetComponent<RawImage>().texture = null;
                Item1Icon.GetComponent<RawImage>().color = resetColor;
                Item1Text.GetComponent<TextMeshProUGUI>().text = "";
                Item1BG.GetComponent<RawImage>().color = bgColor;
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
    public bool SlotHasNoItem()
    {
        return _itemHotbar[_selectedItemIndex] == null;
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

        Color highlightColor = new Color(255, 0, 0, 1f);
        Color unHighlightColor = new Color(0, 0, 0, 1f);

        switch (_selectedItemIndex)
        {
            case 0:
                Item1BG.GetComponent<RawImage>().color = highlightColor;
                Item2BG.GetComponent<RawImage>().color = unHighlightColor;
                Item3BG.GetComponent<RawImage>().color = unHighlightColor;
                Item4BG.GetComponent<RawImage>().color = unHighlightColor;
                break;
            case 1:
                Item2BG.GetComponent<RawImage>().color = highlightColor;
                Item1BG.GetComponent<RawImage>().color = unHighlightColor;
                Item3BG.GetComponent<RawImage>().color = unHighlightColor;
                Item4BG.GetComponent<RawImage>().color = unHighlightColor;
                break;
            case 2:
                Item3BG.GetComponent<RawImage>().color = highlightColor;
                Item1BG.GetComponent<RawImage>().color = unHighlightColor;
                Item2BG.GetComponent<RawImage>().color = unHighlightColor;
                Item4BG.GetComponent<RawImage>().color = unHighlightColor;
                break;
            default:
                Item4BG.GetComponent<RawImage>().color = highlightColor;
                Item1BG.GetComponent<RawImage>().color = unHighlightColor;
                Item2BG.GetComponent<RawImage>().color = unHighlightColor;
                Item3BG.GetComponent<RawImage>().color = unHighlightColor;
                break;
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

    /// <summary>
    /// Checks for the next empty available slot in item hotbar
    /// </summary>
    /// <returns>Returns true or false if an empty slot is available</returns>
    public int CheckAvailableItemSlots()
    {
        int tempIndex = _selectedItemIndex;

        for(int i = 0; i < 4; i++)
        {
            tempIndex = (tempIndex + 1) % 4;
            if (_itemHotbar[tempIndex] == null)
            {
                return tempIndex;
            }
        }

        return _selectedItemIndex;
    }


    /// <summary>
    /// Resets the entire item hotbar. Call this when loading into new level
    /// </summary>
    public void ResetItemHotbar()
    {
        Array.Clear(_itemHotbar, 0, 4);
    }
}
