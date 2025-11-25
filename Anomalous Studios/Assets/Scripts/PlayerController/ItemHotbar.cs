using ItemSystem;
using System;
using System.Collections;
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

    [SerializeField] private GameObject Item1Text;
    [SerializeField] private GameObject Item2Text;
    [SerializeField] private GameObject Item3Text;
    [SerializeField] private GameObject Item4Text;

    private float _fadeDuration = 1.0f;
    private Coroutine _fadeCoroutine;

    [Header("Watch UI Gameobjects")]
    public GameObject WatchUI;
    public GameObject TimeUI;
    public bool _watchActive = false;

    // Getter Methods
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
        switch (_selectedItemIndex)
        {
            case 1:
                Item2Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item2Icon.GetComponent<RawImage>().color = Color.yellow;
                if (_itemHotbar[_selectedItemIndex].GetComponent<Key>())
                {
                    Item2Text.GetComponent<Text>().text = _itemHotbar[_selectedItemIndex].GetComponent<Key>().GetKeyID();
                }
                break;
            case 2:
                Item3Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item3Icon.GetComponent<RawImage>().color = Color.yellow;
                if (_itemHotbar[_selectedItemIndex].GetComponent<Key>()){
                    Item3Text.GetComponent<Text>().text = _itemHotbar[_selectedItemIndex].GetComponent<Key>().GetKeyID();
                }
                break;
            case 3:
                Item4Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item4Icon.GetComponent<RawImage>().color = Color.yellow;
                if (_itemHotbar[_selectedItemIndex].GetComponent<Key>())
                {
                    Item4Text.GetComponent<Text>().text = _itemHotbar[_selectedItemIndex].GetComponent<Key>().GetKeyID();
                }
                break;
            default:
                Item1Icon.GetComponent<RawImage>().texture = _itemHotbar[_selectedItemIndex].GetComponent<ItemInstance>().item.itemIcon.texture;
                Item1Icon.GetComponent<RawImage>().color = Color.yellow;
                if (_itemHotbar[_selectedItemIndex].GetComponent<Key>())
                {
                    Item1Text.GetComponent<Text>().text = _itemHotbar[_selectedItemIndex].GetComponent<Key>().GetKeyID();
                }
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
                    Item2Text.GetComponent<Text>().text = "";
                    break;
                case 2:
                    Item3Icon.GetComponent<RawImage>().color = resetColor;
                    Item3Text.GetComponent<Text>().text = "";
                    break;
                case 3:
                    Item4Icon.GetComponent<RawImage>().color = resetColor;
                    Item4Text.GetComponent<Text>().text = "";
                    break;
                default:
                    Item1Icon.GetComponent<RawImage>().color = resetColor;
                    Item1Text.GetComponent<Text>().text = "";
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
                Item2Text.GetComponent<Text>().text = "";
                break;
            case 2:
                Item3Icon.GetComponent<RawImage>().texture = null;
                Item3Icon.GetComponent<RawImage>().color = resetColor;
                Item3Text.GetComponent<Text>().text = "";
                break;
            case 3:
                Item4Icon.GetComponent<RawImage>().texture = null;
                Item4Icon.GetComponent<RawImage>().color = resetColor;
                Item4Text.GetComponent<Text>().text = "";
                break;
            default:
                Item1Icon.GetComponent<RawImage>().texture = null;
                Item1Icon.GetComponent<RawImage>().color = resetColor;
                Item1Text.GetComponent<Text>().text = "";
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
        /*for (int i = 0; i < 4; i++)
        {
            _itemHotbar[i] = null;
        }*/

        Array.Clear(_itemHotbar, 0, 4);
    }
}
