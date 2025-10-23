using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveKeyProfile", menuName = "Save System/Save Key Profile")]
public class SaveProfileSO : ScriptableObject, IVariableKeySource
{
    [SerializeField] private List<string> keys = new();
    public IEnumerable<string> GetAllKeys() => keys;
}
