using SharperSave.DataClasses;
using UnityEngine;

namespace SharperSave
{
    /// <summary>
    /// The save container that stores the SaveData class.
    /// </summary>
    [CreateAssetMenu(fileName = "Save Container", menuName = "Sharper Save/Save Container")]
    public class SaveContainer : ScriptableObject
    {
        public SaveData saveData;
    }
}