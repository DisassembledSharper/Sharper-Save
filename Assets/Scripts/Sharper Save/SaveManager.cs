using SharperSave.DataClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SharperSave
{
    /// <summary>
    /// The Save Manager containing methods to load and save data.
    /// </summary>
    [CreateAssetMenu(fileName ="Save Manager", menuName = "Sharper Save/Save Manager")]
    public class SaveManager : ScriptableObject
    {
        [Header("Setup")]
        [Space(1)]

        [Header("Files")]
        [Tooltip("The save file name.")]
        [SerializeField] private string _saveFileName = "save";

        [Tooltip("The save file extension.")]
        [SerializeField] private string _saveFileExtension = "sav";

        [Tooltip("The hash file name.")]
        [SerializeField] private string _hashFileName = "hash";

        [Tooltip("The hash file extension.")]
        [SerializeField] private string _hashFileExtension = "hsh";

        [Tooltip("Selected slot to save.")]
        [SerializeField] private SaveSlots _currentSlot = SaveSlots.Slot1;

        [Tooltip("The path to save data.")]
        [SerializeField] private SavePaths _savePath = SavePaths.PersistentDataPath;
        [Space(2)]

        [Header("Save Protection")]
        [Tooltip("Enables a simple protection to prevent an easy save modification by user.")]
        [SerializeField] private bool _protectSave = true;

        [Tooltip("Shuffle seed to be used in the RNG to shuffle the data bytes.")]
        [SerializeField] private int _shuffleSeed = 1;

        [Tooltip("A salt to be used in the hash generator, making difficult to inject a new hash.")]
        [SerializeField] private string _hashSalt = "salt423";

        [SerializeField] private bool _useMongoDB;
        [Space(2)]
        

        [Header("References")]
        [Tooltip("The container to get and set data.")]
        [SerializeField] private SaveContainer _saveContainer;

        /// <summary>
        /// Gets and sets de current save slot.
        /// </summary>
        public SaveSlots CurrentSlot { get => _currentSlot; set => _currentSlot = value; }
        /// <summary>
        /// A enum containing the possible save slots.
        /// </summary>
        public enum SaveSlots { Slot1, Slot2, Slot3 };
        /// <summary>
        /// The possible save paths.
        /// </summary>
        public enum SavePaths { PersistentDataPath, DataPath };

        //Events

        /// <summary>
        /// Called when the SaveManager starts to save.
        /// </summary>
        public Action OnStartSave;
        
        /// <summary>
        /// Called when the SaveManager ends saving;
        /// </summary>
        public Action OnEndSave;
        
        /// <summary>
        /// Called when the SaveManager saves succesfully.
        /// </summary>
        public Action OnSaveSuccess;
        
        /// <summary>
        /// Called when the SaveManager fails to save and it returns an Exception. 
        /// </summary>
        public Action<Exception> OnSaveError;


        /// <summary>
        /// Called when the SaveManager starts to load.
        /// </summary>
        public Action OnStartLoad;

        /// <summary>
        /// Called when the SaveManager ends loading.
        /// </summary>
        public Action OnEndLoad;

        /// <summary>
        /// Called when the SaveManager loads succesfully.
        /// </summary>
        public Action OnLoadSuccess;

        /// <summary>
        /// Called when the SaveManager fails to load and it returns an Exception. 
        /// </summary>
        public Action<Exception> OnLoadError;


        /// <summary>
        /// Saves the Save Container to file.
        /// </summary>
        public void Save()
        {
            try
            {
                OnStartSave?.Invoke();
                if (_saveFileName == "")
                {
                    throw new Exception("Empty save file name");
                }

                if (_saveFileExtension == "")
                {
                    throw new Exception("Empty save file extension");
                }

                string saveContent = JsonUtility.ToJson(_saveContainer.saveData);

                if (_useMongoDB)
                {
                    RealmManager.Instance.SaveData(saveContent);
                }
                else
                {
                    FileStream fileStream = new(GetSavePath(), FileMode.Create);

                    if (_protectSave)
                    {
                        string saveHash = SaveIntegrityUtility.GetStringHash(saveContent, _hashSalt);
                        using (StreamWriter streamWriter = new(GetHashPath()))
                        {
                            streamWriter.Write(saveHash);
                        }

                        List<byte> shuffledBytes = SaveIntegrityUtility.ShuffleBytes(Encoding.UTF8.GetBytes(saveContent).ToList(), _shuffleSeed);

                        saveContent = "";

                        for (int i = 0; i < shuffledBytes.Count; i++)
                        {
                            saveContent += shuffledBytes[i].ToString();

                            if (i < shuffledBytes.Count - 1)
                            {
                                saveContent += " ";
                            }
                        }

                        using (BinaryWriter binaryWriter = new(fileStream))
                        {
                            binaryWriter.Write(saveContent);
                        }
                        
                    }
                    else
                    {
                        using (StreamWriter streamWriter = new(fileStream))
                        {
                            streamWriter.Write(saveContent);
                        }
                    }

                    fileStream.Close();
                }
                
                OnSaveSuccess?.Invoke();
            }
            catch (Exception e)
            { 
                OnSaveError?.Invoke(e);
                Debug.LogException(e);
            }
            OnEndSave?.Invoke();
        }
        /// <summary>
        /// Loads the save file.
        /// </summary>
        public void Load()
        {
            bool wasLoaded = false;
            try
            {
                OnStartLoad?.Invoke();
                string content = "";

                if (_useMongoDB)
                {
                    content = RealmManager.Instance.GetData();
                }
                else
                {
                    if (_protectSave)
                    {
                        byte[] contentInBytes;
                        string previousHash = "";
                        string currentHash;

                        using (FileStream fileStream = new(GetSavePath(), FileMode.Open))
                        {
                            using (BinaryReader binaryReader = new(fileStream))
                            {
                                var bytesList = new List<byte>();

                                content = binaryReader.ReadString();

                                foreach (string stringByte in content.Split(' '))
                                {
                                    bytesList.Add(Convert.ToByte(stringByte));
                                }

                                contentInBytes = SaveIntegrityUtility.UnshuffleBytes(bytesList, _shuffleSeed).ToArray();
                            }
                        }

                        content = Encoding.UTF8.GetString(contentInBytes);
                        //Debug.Log("Binary reading ok");

                        currentHash = SaveIntegrityUtility.GetStringHash(content, _hashSalt);

                        using (StreamReader reader = new(GetHashPath()))
                        {
                            previousHash = reader.ReadToEnd();
                        }
                        Debug.Log(RealmManager.Instance.GetData());
                        if (previousHash == currentHash)
                        {
                            //Debug.Log("Hash pass");
                        }
                        else if (previousHash == "")
                        {
                            throw new Exception("Hash not found");
                        }
                        else
                        {
                            throw new Exception("Hash check fail");
                        }
                    }
                    else
                    {
                        using (StreamReader reader = new(GetSavePath()))
                        {
                            content = reader.ReadToEnd();
                        }
                    }

                    _saveContainer.saveData = JsonUtility.FromJson<SaveData>(content);
                }
                
                wasLoaded = true;
                OnLoadSuccess?.Invoke();
            }
            catch(EndOfStreamException e)
            {
                OnLoadError?.Invoke(e);
                Debug.LogError("Binary fail");
            }
            catch (ArgumentException e)
            {
                OnLoadError?.Invoke(e);
                Debug.LogError("Content reading fail");
            }
            catch (FileNotFoundException e)
            {
                OnLoadError?.Invoke(e);
                Debug.LogError("Save file not found");
            }
            catch(Exception e)
            {
                OnLoadError?.Invoke(e);
                Debug.LogException(e);
            }
            if (wasLoaded)
            {
                //Debug.Log("Load success");
            }
            else
            {
                Debug.LogError("Load failed");
            }
            OnEndLoad?.Invoke();
        }

        /// <summary>
        /// Gets the selected path to save.
        /// </summary>
        /// <returns>The path to save.</returns>
        private string GetPath()
        {
            string path;
            switch (_savePath)
            {
                case SavePaths.DataPath:
                    path = Application.dataPath;
                    break;
                default:
                    path = Application.persistentDataPath;
                    break;
            }
            return path;
        }
        
        private string GetSavePath()
        {
            return GetPath() + "/" + _saveFileName + "_" + ((int)CurrentSlot + 1) + "." + _saveFileExtension;
        }

        private string GetHashPath()
        {
            return GetPath() + "/" + _hashFileName + "_" + ((int)CurrentSlot + 1) + "." + _hashFileExtension;
        }
    }
}