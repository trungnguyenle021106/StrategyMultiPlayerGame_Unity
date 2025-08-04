using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
public class GooglePlayGameSignIn : MonoBehaviourPunCallbacks
{
    public GameObject loadingPanel;
    public InfoAccount infoAccount;
    private string Error;
    public string Name { get; private set; }
    void Awake()
    {
    }

    private void Start()
    {
        PlayGamesPlatform.Activate();
        LoginGooglePlayGames();
    }
    private void LoginGooglePlayGames()
    {
        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Success");
                FirestoreDatabase.Instance.GetAccountAsyncByName(PlayGamesPlatform.Instance.GetUserDisplayName(),
                (account) =>
                {
                    this.name = PlayGamesPlatform.Instance.GetUserDisplayName();
                    FirebaseRealtimeDB.Instance.SetupPlayerStatusEvent(this.name);
                    PlayGamesPlatform.Instance.RequestServerSideAccess(false, (token) =>
                    {
                        Debug.Log($"TOKEN: {token}");
                        if (account == null)
                        {
                            Debug.Log("null");
                            FirestoreDatabase.Instance.AddAccountAsync(PlayGamesPlatform.Instance.GetUserDisplayName(),
                            (account) =>
                            {
                                this.infoAccount.account = account;
                                StartCoroutine(this.SetImage(() =>
                            {
                                this.ConnectPun2();
                            }));
                            });
                        }
                        else
                        {
                            Debug.Log("Check Time");
                            Timestamp lastResetTimestamp = account.LastLoginTime;
                            Debug.Log($"LastResetTimestamp: {lastResetTimestamp}");

                            DateTime lastResetTime = lastResetTimestamp.ToDateTime().ToUniversalTime();
                            Debug.Log($"LastResetTime: {lastResetTime}");

                            DateTime now = DateTime.UtcNow;
                            Debug.Log($"CurrentTime (UTC): {now}");


                            Debug.Log($"{now} > {lastResetTime} is {lastResetTime < now}");
                            if (lastResetTime < now)
                            {
                                this.ResetMorning(account, (response) =>
                                {
                                    FirestoreDatabase.Instance.GetAccountAsyncByName(PlayGamesPlatform.Instance.GetUserDisplayName(), (UpdateAccount) =>
                                    {
                                        this.infoAccount.account = UpdateAccount;
                                        StartCoroutine(this.SetImage(() =>
                                        {
                                            this.ConnectPun2();
                                        }));
                                    });
                                });
                            }
                            else
                            {
                                this.infoAccount.account = account;
                                // this.ConnectPun2();
                                StartCoroutine(this.SetImage(() =>
                                {
                                    this.ConnectPun2();
                                }));
                            }
                        }
                    });

                });
            }
            else
            {
                Error = "Failed to retrieve Google play games authorization code";
                Debug.Log("Login Unsuccessful");
            }
        });
    }

    private void ResetMorning(Account account, Action<AccountFB> callback)
    {
        DateTime tomorrow = DateTime.UtcNow.AddDays(1);
        DateTime nextResetTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 0, DateTimeKind.Utc);
        AccountFB accountFB = new AccountFB();
        accountFB.Name = account.Name;
        accountFB.Energy = 20;
        accountFB.ADViewTime = 5;
        accountFB.LastLoginTime = Timestamp.FromDateTime(nextResetTime);
        accountFB.Characters = account.Characters;
        accountFB.Coins = account.Coins;
        accountFB.RankPoint = account.RankPoint;

        FirestoreDatabase.Instance.SetNameCollection("Account");
        FirestoreDatabase.Instance.UpdateAsync<AccountFB>(account.id, accountFB, (response) =>
        {
            callback(response);
        });
    }

    private void ConnectPun2()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.AuthValues = new Photon.Realtime.AuthenticationValues(this.infoAccount.account.Name);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.NickName = this.name;
        SceneManager.LoadScene("MyLobbyScene");
    }

    private void LoadFriend(Action callback)
    {
        GooglePlayGames.PlayGamesPlatform.Instance.LoadFriends(100, false, (status) =>
       {
           if (status == GooglePlayGames.BasicApi.LoadFriendsStatus.Completed)
           {
               IUserProfile[] friends = GooglePlayGames.PlayGamesPlatform.Instance.GetFriends();
               StartCoroutine(this.WaitForImagesToLoad(friends, () =>
               {
                   FirestoreDatabase.Instance.SetNameCollection("Account");
                   FirestoreDatabase.Instance.GetListAsync<AccountFB>((list) =>
                   {
                       List<IUserProfile> myFriends = new List<IUserProfile>();
                       for (int i = 0; i < friends.Length; i++)
                       {
                           foreach (AccountFB playerName in list)
                           {
                               if (playerName.Name.Equals(friends[i].userName))
                               {
                                   myFriends.Add(friends[i]);
                               }
                           }
                       }
                       this.infoAccount.account.friends = myFriends;
                       callback();
                   });
               }));
           }
           else
           {
               this.LoadMoreFriends(callback);
           }
       });
    }

    private void LoadMoreFriends(Action callback)
    {
        GooglePlayGames.PlayGamesPlatform.Instance.LoadMoreFriends(100, (status) =>
             {
                 if (status == GooglePlayGames.BasicApi.LoadFriendsStatus.Completed)
                 {
                     IUserProfile[] friends = GooglePlayGames.PlayGamesPlatform.Instance.GetFriends();
                     StartCoroutine(this.WaitForImagesToLoad(friends, () =>
                     {
                         FirestoreDatabase.Instance.SetNameCollection("Account");
                         FirestoreDatabase.Instance.GetListAsync<AccountFB>((list) =>
                         {
                             List<IUserProfile> myFriends = new List<IUserProfile>();
                             for (int i = 0; i < friends.Length; i++)
                             {
                                 foreach (AccountFB playerName in list)
                                 {
                                     if (playerName.Name.Equals(friends[i].userName))
                                     {
                                         myFriends.Add(friends[i]);
                                     }
                                 }
                             }
                             this.infoAccount.account.friends = myFriends;
                             callback();
                         });
                     }));
                 }
                 else
                 {
                     LoadMoreFriends(callback);
                 }
             });
    }
    private IEnumerator WaitForImagesToLoad(IUserProfile[] friends, Action callback)
    {
        foreach (var friend in friends)
        {
            while (friend.image == null)
            {
                yield return null;
            }
        }
        callback();
    }
    private IEnumerator SetImage(Action callback)
    {
        this.loadingPanel.SetActive(true);
        string imageUrl = PlayGamesPlatform.Instance.GetUserImageUrl();
        Debug.Log("Avatar URL: " + imageUrl);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return www.SendWebRequest();

        try
        {
            if (www.result != UnityWebRequest.Result.Success)
            {
                throw new Exception(www.error);
            }

            Texture2D avatarTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            if (avatarTexture != null)
            {
                this.infoAccount.userAvatarSprite = Sprite.Create(avatarTexture, new Rect(0, 0, avatarTexture.width, avatarTexture.height), new Vector2(0.5f, 0.5f));
                Debug.Log("Avatar image set successfully.");
            }
            else
            {
                Debug.LogError("Avatar texture is null.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error setting avatar image: " + ex.Message);
        }
        finally
        {
            this.LoadFriend(callback);
        }
    }


}
