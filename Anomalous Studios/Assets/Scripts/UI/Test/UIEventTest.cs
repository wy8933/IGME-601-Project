using UnityEngine;
using static PopupEvent;

public class UIEventTest : MonoBehaviour
{
    public void Test()
    {
        EventBus<OpenPopup>.Raise(new OpenPopup { RuleName = "Killed by test" });
        print("Popup Event fired");
    }

}
