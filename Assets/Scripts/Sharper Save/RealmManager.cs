using System;
using UnityEngine;
using Realms;
using Realms.Sync;
using System.Linq;
using SharperSave.DataClasses;

namespace SharperSave
{
    public class RealmManager : MonoBehaviour
    {
        [SerializeField] private string _realmAppId = "YourAppId";
        [SerializeField] private SaveContainer _saveContainer;
        [SerializeField] private bool _isLogged;

        private Realm _realm;
        private User _realmUser;
        private App _realmApp;

        public static RealmManager Instance { get; private set; }

        public Action OnStartConnect;
        public Action OnConnect;
        public Action<Exception> OnFailConnect;

        public Action OnStartGetData;
        public Action<Exception> OnFailGetData;

        public Action OnStartSaveData;
        public Action<Exception> OnFailSaveData;


        private void Awake()
        {
            Instance = this;
            ConnectRealm();
        }

        private async void ConnectRealm()
        {
            try
            {
                OnStartConnect?.Invoke();
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
                OnFailConnect?.Invoke(e);
            }
        }

        public string GetData()
        {
            try
            {
                OnStartGetData?.Invoke();
                if (!_isLogged) throw new Exception("It is not logged.");

                UserData result = new();
                result = _realm.All<UserData>().Where(u => u.OwnerId == _realmUser.Id).FirstOrDefault();

                if (result != null)
                {
                    return result.saveData;
                }
                else
                {
                    throw new Exception("Failed to get data");
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                OnFailGetData?.Invoke(e);
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
                OnFailSaveData?.Invoke(e);
            }
            
        }
    }
}