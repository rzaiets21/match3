using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GGMatch3;
using ProtoModels;
using Proyecto26;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Firestore;
using Newtonsoft.Json;

public class MessageUtility : BehaviourSingletonInit<MessageUtility>
{
    [SerializeField] private GameObject saving;
    [SerializeField] private GameObject logout;
    
    [SerializeField] private EmailDialog emailDialog;
    [SerializeField] private SuccessDialog successDialog;
    [SerializeField] private LoadingDialog loadingDialog;
    [SerializeField] private ErrorDialog errorDialog;
    [SerializeField] private HasNewestDialog hasNewestDialog;

    public Action<string> action;
    [Serializable]
    public class Message
    {
        public int coins;
        public int diamonds;
        public int passedStages;
        public byte[] model;
        public byte[] roomsmodel;
        public string levelProgress;
    }

    // [Serializable]
    // public class PlayerData
    // {
    //     public int coins;
    //     public int diamonds;
    //     public int passedStages;
    //     public byte[] model;
    //     public byte[] roomsmodel;
    // }
    private FirebaseFirestore _databaseReference;

    private FirebaseFirestore databaseReference
    {
        get
        {
            if (_databaseReference == null)
            {
                _databaseReference = FirebaseFirestore.DefaultInstance;
                ChangeState();
            }

            return _databaseReference;
        }

        set => _databaseReference = value;
    }
    private readonly string basePath = "https://home-design---extreme-default-rtdb.firebaseio.com/";
    private const string Token = "QFoXzIktKSYEPoleM2tfBCZFErE6PEbTLFEwtitW";
    private RequestHelper currentRequest;

    private void OnEnable()
    {
        ChangeState();
    }

    private void OnDisable()
    {
        AutoSave();
    }

    private void OnApplicationQuit()
    {
        AutoSave();
    }

    private void ChangeState()
    {
        var logged = PlayerPrefs.GetInt("Logged", 0);
        GGUtil.SetActive(saving, logged == 0);
        GGUtil.SetActive(logout, logged == 1);
    }

    private void Start()
    {
        if(databaseReference != null)
            return;
        
        databaseReference = FirebaseFirestore.DefaultInstance;
    }

    public void Logout()
    {
        PlayerPrefs.SetInt("Logged", 0);
        ChangeState();
    }
    
    private void Combine(out StringBuilder path, string query = null, params string[] children)
    {
        path = new StringBuilder();
        path.Append(basePath); //Add default url 

        foreach (var child in children)
        {
            path.Append(child);
            path.Append("/");
        }

        path.Append(".json?");
        path.Append($"auth={Token}");

        if (string.IsNullOrWhiteSpace(query)) return;

        path.Append("?");
        path.Append(query);
        path.Append($"&auth={Token}");
    }

    public async void AutoSave()
    {
        var logged = PlayerPrefs.GetInt("Logged", 0);
        if(logged == 0)
            return;
        
        var email = PlayerPrefs.GetString("Email");
        email = email.Replace("@", "");
        email = email.Replace(".", "_");
        Debug.Log("Send Message");

        if (string.IsNullOrWhiteSpace(email))
        {
            Debug.LogError("Error post!");
            return;
        }
        
        var ms = new Message();
        
        Combine(out StringBuilder path, children: new string[] { "UserData", email });
        var val = new ProtoSerializer();
        
        var walletManager = GGPlayerSettings.instance.walletManager;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            val.Serialize(memoryStream, Match3StagesDB.instance.GetModel());
            memoryStream.Flush();
            ms.model = memoryStream.ToArray();
        }
        
        using (MemoryStream memoryStream = new MemoryStream())
        {
            val.Serialize(memoryStream, RoomsBackend.instance.GetModel());
            memoryStream.Flush();
            ms.roomsmodel = memoryStream.ToArray();
        }

        ms.coins = (int)walletManager.CurrencyCount(CurrencyType.coins);
        ms.diamonds = (int)walletManager.CurrencyCount(CurrencyType.diamonds);

        if (PlayerPrefs.HasKey(GameScreen.CURRENT_LEVEL_PROGRESS))
        {
            ms.levelProgress = PlayerPrefs.GetString(GameScreen.CURRENT_LEVEL_PROGRESS);
        }
        
        // string json = JsonUtility.ToJson(ms);
        Dictionary<string, object> msg = new Dictionary<string, object>()
        {
            {"coins", ms.coins},
            {"diamonds", ms.diamonds},
            {"passedStages", ms.passedStages},
            {"model", ms.model},
            {"roomsmodel", ms.roomsmodel},
            {"levelProgress", ms.levelProgress}
        };
        await databaseReference.Collection("Users").Document(email).SetAsync(msg).ContinueWithOnMainThread(x =>
        {
            try
            {
                if (x.IsCompleted)
                {
                    Debug.Log("success!");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error post!");
                Debug.LogError(e.ToString());
            }
        });
        
        // RestClient.Put<Message>(path.ToString(), ms).Then((s) =>
        //     {
        //         Debug.Log("success!");
        //     }
        // ).Catch((e) => {
        //     
        //     Debug.LogError("Error post!");
        //     Debug.LogError(e.ToString());
        // });
    }

    private void CheckNewestPlayerData(string email)
    {
        PlayerPrefs.SetString("Email", email);
        var hasNewest = false;
        
        loadingDialog.Show();

        LoadData(email, (message) =>
        {
            var passedStages = Match3StagesDB.instance.passedStages;
            if (message.passedStages > passedStages)
                hasNewest = true;
            if (!string.IsNullOrEmpty(message.levelProgress) && !hasNewest)
            {
                if (PlayerPrefs.HasKey(GameScreen.CURRENT_LEVEL_PROGRESS))
                {
                    var json = PlayerPrefs.GetString(GameScreen.CURRENT_LEVEL_PROGRESS);
                    var old = JsonConvert.DeserializeObject<LevelDefinition>(json);
                    var levelDefinition = JsonConvert.DeserializeObject<LevelDefinition>(message.levelProgress);

                    if (old.name == levelDefinition.name)
                    {
                        if (old.goals.goals.Sum(x => x.collectCount) >
                            levelDefinition.goals.goals.Sum(x => x.collectCount))
                        {
                            hasNewest = true;
                        }
                    }
                }
            }
            if (hasNewest)
            {
                loadingDialog.Hide();
                hasNewestDialog.Show();
                return;
            }

            PostUser(email);
        }, (e) =>
        {
            SaveToFirebase(false);
        });

  
    }

    public void SaveToFirebase(bool useNewest)
    {
        var email = PlayerPrefs.GetString("Email");
        PostUser(email, useNewest);
    }

    public void LoadNewest()
    {
        var email = PlayerPrefs.GetString("Email");
        LoadData(email, ApplyLoadedPlayerData, (e) =>
        {
            loadingDialog.Hide();
            errorDialog.Show("Something went wrong!");
            Debug.LogError("User not found!");
            Debug.LogError(e.ToString());
        });
    }
    
    private async void PostUser(string email, bool hasNewest = false)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            loadingDialog.Hide();
            errorDialog.Show("Something went wrong!");
            return;
        }
        
        PlayerPrefs.SetString("Email", email);
        email = email.Replace("@", "");
        email = email.Replace(".", "_");
        Debug.Log("Send Message");

        if (hasNewest)
            loadingDialog.Show();

        var ms = new Message();
        
        Combine(out StringBuilder path, children: new string[] { "UserData", email });
        var val = new ProtoSerializer();
        
        var walletManager = GGPlayerSettings.instance.walletManager;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            val.Serialize(memoryStream, Match3StagesDB.instance.GetModel());
            memoryStream.Flush();
            ms.model = memoryStream.ToArray();
        }
        
        using (MemoryStream memoryStream = new MemoryStream())
        {
            val.Serialize(memoryStream, RoomsBackend.instance.GetModel());
            memoryStream.Flush();
            ms.roomsmodel = memoryStream.ToArray();
        }

        ms.coins = (int)walletManager.CurrencyCount(CurrencyType.coins);
        ms.diamonds = (int)walletManager.CurrencyCount(CurrencyType.diamonds);
        ms.passedStages = Match3StagesDB.instance.passedStages;

        Dictionary<string, object> msg = new Dictionary<string, object>()
        {
            {"coins", ms.coins},
            {"diamonds", ms.diamonds},
            {"passedStages", ms.passedStages},
            {"model", ms.model},
            {"roomsmodel", ms.roomsmodel}
        };
        await databaseReference.Collection("Users").Document(email).SetAsync(msg).ContinueWithOnMainThread(x =>
        {
            Debug.LogError(x.AsyncState);
            try
            {
                loadingDialog.Hide();
                successDialog.Show("Save complete!");
                Debug.Log("success!");
                PlayerPrefs.SetInt("Logged", 1);
                ChangeState();
            }
            catch (Exception e)
            {
                loadingDialog.Hide();
                errorDialog.Show("Something went wrong!");
                Debug.LogError("Error post!");
                Debug.LogError(e.ToString());
            }
        });
        //
        // RestClient.Put<Message>(path.ToString(), ms).Then((s) =>
        //     {
        //         loadingDialog.Hide();
        //         successDialog.Show("Save complete!");
        //         Debug.Log("success!");
        //         PlayerPrefs.SetInt("Logged", 1);
        //         ChangeState();
        //     }
        // ).Catch((e) => {
        //     
        //     loadingDialog.Hide();
        //     errorDialog.Show("Something went wrong!");
        //     Debug.LogError("Error post!");
        //     Debug.LogError(e.ToString());
        // });
    }

    public async void LoadData(string email, Action<Message> onSuccess, Action<Exception> onError)
    {
        PlayerPrefs.SetString("Email", email);
        email = email.Replace("@", "");
        email = email.Replace(".", "_");
        loadingDialog.Show();
        Combine(out StringBuilder path, children: new string[] { "UserData", email });
        var snapshot = await databaseReference.Collection("Users").Document(email).GetSnapshotAsync();
        if(snapshot.Exists)
        {
            var message = new Message();
            var dictionary = snapshot.ToDictionary();
            foreach (var VARIABLE in dictionary)
            {
                // Debug.LogError($"{VARIABLE.Key},{VARIABLE.Value}");
                switch (VARIABLE.Key)
                {
                    case "coins":
                        message.coins = (int)VARIABLE.Value.ConvertTo(typeof(int));
                        break;
                    case "diamonds":
                        message.diamonds = (int)VARIABLE.Value.ConvertTo(typeof(int));
                        break;
                    case "passedStages":
                        message.passedStages = (int)VARIABLE.Value.ConvertTo(typeof(int));
                        break;
                    case "model": 
                        message.model = ((Blob)VARIABLE.Value).ToBytes();
                        break;
                    case "roomsmodel":
                        message.roomsmodel = ((Blob)VARIABLE.Value).ToBytes();
                        break;
                }
            }
            Debug.Log("success!");
            PlayerPrefs.SetInt("Logged", 1);
            onSuccess?.Invoke(message);
        }
        else
        {
            onError?.Invoke(null);
        }
        // RestClient.Get<Message>(path.ToString()).Then((s) =>
        //     {
        //         Debug.Log("success!");
        //         PlayerPrefs.SetInt("Logged", 1);
        //         onSuccess?.Invoke(s);
        //     }
        // ).Catch(onError);
    }

    private void ApplyLoadedPlayerData(Message s)
    {
        ProtoIO.LoadFromByteStream(s.model, out Match3Stages model);
        ProtoIO.LoadFromByteStream(s.roomsmodel, out RoomDecoration roomsmodel);
        Match3StagesDB.instance.SetNewModel(model);
        RoomsBackend.instance.SetNewModel(roomsmodel);
                
        var walletManager = GGPlayerSettings.instance.walletManager;
        walletManager.SetCurrency(CurrencyType.coins, s.coins);
        walletManager.SetCurrency(CurrencyType.diamonds, s.diamonds);
        
        PlayerPrefs.SetString(GameScreen.CURRENT_LEVEL_PROGRESS, s.levelProgress);
        SceneManager.LoadScene("MainUI");
    }
    
    [ContextMenu("SendToServer")]
    public void SendToServer()
    {
        if (PlayerPrefs.GetInt("Logged", 0) == 1)
        {
            PostUser(PlayerPrefs.GetString("Email"));
            return;
        }
        emailDialog.Show();
        action = CheckNewestPlayerData;
    }    
    
    [ContextMenu("LoadFromServer")]
    public void LoadFromServer()
    {
        emailDialog.Show();
        action = (s) => LoadData(s, ApplyLoadedPlayerData, (e) =>
        {
            
            loadingDialog.Hide();
            errorDialog.Show("Something went wrong!");
            Debug.LogError("User not found!");
            Debug.LogError(e.ToString());
        });
    }

    [ContextMenu("ResetAll")]
    public void ResetAll()
    {
        Match3StagesDB.instance.ResetAll();
        RoomsBackend.instance.Reset();
        
        SceneManager.LoadScene("MainUI");
    }
}
