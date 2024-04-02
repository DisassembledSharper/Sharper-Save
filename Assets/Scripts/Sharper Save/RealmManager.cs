using System;
using UnityEngine;
using Realms;
using MongoDB.Bson;
using Realms.Sync;
using System.Linq;
using SharperSave.DataClasses;

namespace SharperSave
{
    public class RealmManager : MonoBehaviour
    {
        [SerializeField] private string _realmAppId = "sharpersave-thiya";
        [SerializeField] private SaveContainer _saveContainer;

        private Realm _realm;
        private User _realmUser;
        private App _realmApp;


        private void Awake()
        {
            RealmLogin();
        }

        private async void RealmLogin()
        {
            _realmApp = App.Create(new AppConfiguration(_realmAppId));
            _realmUser = await _realmApp.LogInAsync(Credentials.Anonymous());
            _realm = await Realm.GetInstanceAsync(new FlexibleSyncConfiguration(_realmUser));
            Debug.Log("Logado");     
        }

        public void Register()
        {
            try
            {
                string stringId = PlayerPrefs.GetString("objectId", ObjectId.GenerateNewId().ToString());

                var id = ObjectId.Parse(stringId);
                UserData result = new();
                _realm.Subscriptions.Update(() =>
                {
                    var userRealm = _realm.All<UserData>().Where(u => u.OwnerId == _realmUser.Id);
                    result = userRealm.FirstOrDefault();
                    _realm.Subscriptions.Add(userRealm);
                });
                if (result != null)
                {
                    Debug.Log("Achou");
                    SaveData parsedData = JsonUtility.FromJson<SaveData>(result.saveData);
                    _saveContainer.saveData = parsedData;
                }
                else
                {
                    Debug.Log("Não achou");

                    _realm.Write(() =>
                    {
                        UserData data = new();
                        data.OwnerId = _realmUser.Id;
                        data.saveData = JsonUtility.ToJson(_saveContainer.saveData);
                        data.Id = id;
                        _realm.Add(data);
                    });
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}