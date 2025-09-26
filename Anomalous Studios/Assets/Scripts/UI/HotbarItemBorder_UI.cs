using UnityEngine.UIElements;
using UnityEngine;

public class HotbarItemBorder_UI : MonoBehaviour
{
    [SerializeField] VisualTreeAsset uiDocument;

    private void Start()
    {
        VisualElement root = uiDocument.Instantiate();

        GetComponent<UIDocument>().rootVisualElement.Add(root);

        Box myBox = new Box();
        myBox.style.backgroundColor = Color.gray;
        myBox.style.paddingBottom = 10;
        myBox.style.marginTop = 10;
        myBox.style.marginBottom = 10;

        myBox.Add(new Label("Welcome to my Box!"));
        TextField nameField = new TextField("Name:");
        myBox.Add(nameField);

        root.Add(myBox);
    }

    private void OnMyButtonClicked()
    {
        Debug.Log("UI TOOLKIT Button Clicked!");
    }
}
