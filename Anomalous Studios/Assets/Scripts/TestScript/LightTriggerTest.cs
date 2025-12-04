using UnityEngine;

public class LightTriggerTest : MonoBehaviour
{
    public string sectionA = "Hallway";
    public string sectionB = "RoomA";

    [ContextMenu("Section A ON")]
    public void SectionAOn()
    {
        LightingSectionManager.TurnOn(sectionA);
    }

    [ContextMenu("Section A OFF")]
    public void SectionAOff()
    {
        LightingSectionManager.TurnOff(sectionA);
    }

    [ContextMenu("Section B ON")]
    public void SectionBOn()
    {
        LightingSectionManager.TurnOn(sectionB);
    }

    [ContextMenu("Section B OFF")]
    public void SectionBOff()
    {
        LightingSectionManager.TurnOff(sectionB);
    }
}
