using System;

namespace SharperSave.DataClasses
{
    /// <summary>
    /// A serializable class to store data.
    /// </summary>
    [Serializable]
    public partial class SaveData
    {
        //Sample data
        public int intData;
        public float floatData;
        public bool boolData;
        public string stringData;
    }
}