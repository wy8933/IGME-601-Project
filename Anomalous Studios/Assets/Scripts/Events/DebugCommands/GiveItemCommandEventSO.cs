using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GiveItemCommandEventSO", menuName = "Events/Commands/GiveItemCommandEventSO")]
public sealed class GiveItemCommandEventSO : BaseEventSO<CommandRequested>
{
    public List<GameObject> itemList = new List<GameObject>();

    protected override void OnEvent(CommandRequested e)
    {
        GameObject item = itemList[int.Parse(e.Args[0])];

        if (item == null) 
        {
            return;
        }

        // find the player contoller and add the item to the item hotbar
        FindAnyObjectByType<PlayerController>().GetItemHotbar().AddItem(item);
    }
}