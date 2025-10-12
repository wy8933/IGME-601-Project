using Unity.VisualScripting;
using UnityEngine;

public struct OpenPopup : IEvent { public string RuleName; }

public class PopupEvent : MonoBehaviour
{
    public Popup ruleBreakPopup;
    private EventBinding<OpenPopup> _ruleName;

    private void OnEnable()
    {
        _ruleName = new EventBinding<OpenPopup>(OnPopup);
        EventBus<OpenPopup>.Register(_ruleName);
    }

    private void OnPopup(OpenPopup e)
    {
        print(e + "Instantiating rule break popup here");
        ruleBreakPopup.popupText.text = e.RuleName;
        Instantiate(ruleBreakPopup, GameObject.FindWithTag("UI").GetComponent<Canvas>().transform);
    }
}
