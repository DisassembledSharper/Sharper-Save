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
        [SerializeField] private bool _isLogged;

        private Realm _realm;
        private User _realmUser;
        private App _realmApp;

        public static RealmManager Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
            ConnectRealm();
        }

        private async void ConnectRealm()
        {
            try
            {
                _realmApp = App.Create(new AppConfiguration(_realmAppId));
                _realmUser = await _realmApp.LogInAsync(Credentials.Anonymous());
                _realm = await Realm.GetInstanceAsync(new FlexibleSyncConfiguration(_realmUser));

                _realm.Subscriptions.Update(() =>
                {
                    var userRealm = _realm.All<UserData>().Where(u => u.OwnerId == _realmUser.Id);
                    _realm.Subscriptions.Add(userRealm);
                });

                _isLogged = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _isLogged = false;
            }
        }

        public string GetData()
        {
            try
            {
                if (!_isLogged) throw new Exception("It is not logged.");

                UserData result = new();
                result = _realm.All<UserData>().Where(u => u.OwnerId == _realmUser.Id).FirstOrDefault();

                if (result != null)
                {
                    Debug.Log("Achou");
                    return result.saveData;
                }
                else
                {
                    Debug.Log("Não achou");
                    return null;

                    //_realm.Write(() =>
                    //{
                    //    UserData data = new();
                    //    data.OwnerId = _realmUser.Id;
                    //    data.saveData = JsonUtility.ToJson(_saveContainer.saveData);
                    //    data.Id = id;
                    //    _realm.Add(data);
                    //});
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public void SaveData(string saveContent)
        {
            try
            {
                var data = _realm.All<UserData>().FirstOrDefault();
                _realm.Write(() =>
                {
                    data.saveData = saveContent;
                });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
        }
    }
}