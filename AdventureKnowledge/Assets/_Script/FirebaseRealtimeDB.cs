using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Reflection;
using System.Threading.Tasks;
using Firebase;
using System.Linq;
using Firebase.Database;

public class FirebaseRealtimeDB : MonoBehaviour
{
    private DatabaseReference playerRef;
    private DatabaseReference connectedRef;
    public static FirebaseRealtimeDB Instance { get; private set; }

    private static FirebaseDatabase db;
    public bool isInit { get; private set; } = false;
    #region private Fields
    private EventHandler<ChildChangedEventArgs> eventInviteChildHandler;
    private EventHandler<ChildChangedEventArgs> eventChatChildHandler = null;
    private EventHandler<ValueChangedEventArgs> eventStatusChildHandler;
    private EventHandler<ChildChangedEventArgs> eventRemoveChildHandler;
    private EventHandler<ValueChangedEventArgs> eventNotificationChatHandler;

    private DatabaseReference chatChildReference;
    private DatabaseReference inviteChildReference;
    private DatabaseReference statusChildReference;
    private DatabaseReference inviteRemoveReference;
    private DatabaseReference notificationChatReference;
    #endregion
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    db = FirebaseDatabase.DefaultInstance;
                    this.isInit = true;
                }
                else { Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result); }
            });
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region public Methods
    public void DenyInvite(string nameFriend, string myName, Action callback)
    {
        DatabaseReference myReference = db.RootReference.Child("Player").Child(myName).Child("Invite").Child(nameFriend);
        myReference.RemoveValueAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsCompleted) { Debug.LogError("Failed to add Invite: " + task.Exception); }
            else
            {
                callback();
            }
        });
    }

    public void InviteFriend(string nameFriend, string myName, string roomName, Action callback)
    {
        DatabaseReference myReference = db.RootReference.Child("Player").Child(nameFriend).Child("Invite").Child(myName);
        myReference.SetValueAsync(roomName).ContinueWithOnMainThread(task =>
        {
            if (!task.IsCompleted) { Debug.LogError("Failed to add Invite: " + task.Exception); }
            else
            {
                callback();
            }
        });
    }

    public void CheckExistChat(string Getter, string Sender, Action<string> callback)
    {
        Debug.Log("CheckExistChat bắt đầu");
        DatabaseReference reference = db.RootReference.Child("Player").Child(Sender).Child("Chat");
        reference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("GetValueAsync hoàn thành");
                DataSnapshot snapshot = task.Result;
                List<string> chatIds = new List<string>();

                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    Debug.Log("Duyệt qua childSnapshot: " + childSnapshot.Key);
                    if (childSnapshot.Key.Equals(Getter))
                    {
                        Debug.Log("Tìm thấy Getter: " + Getter);
                        foreach (DataSnapshot friendValueSnapshot in childSnapshot.Children)
                        {
                            string keyName = friendValueSnapshot.Key;
                            Debug.Log("Tìm thấy keyName: " + keyName);
                            callback(keyName);
                            return;
                        }
                    }
                }
                callback("False");
                Debug.Log("Không tìm thấy Getter, callback False");
            }
            else
            {
                Debug.LogError("Failed to set notification: " + task.Exception);
            }
        });
    }


    public void SendMessage(string Getter, string Sender, string Message, Action<string> callback)
    {
        this.CheckExistChat(Getter, Sender, (response =>
        {
            if (response.Equals("False"))
            {
                Guid uniqueId = Guid.NewGuid();
                string uniqueIdString = uniqueId.ToString();

                DatabaseReference myReference = db.RootReference.Child("Player").Child(Sender).Child("Chat").Child(Getter).Child(uniqueIdString);
                myReference.SetValueAsync(false).ContinueWithOnMainThread(task =>
                {
                    if (!task.IsCompleted) { Debug.LogError("Failed to add myReference: " + task.Exception); }
                    else
                    {
                        callback(uniqueIdString);
                    }
                });

                DatabaseReference friendReference = db.RootReference.Child("Player").Child(Getter).Child("Chat").Child(Sender).Child(uniqueIdString);
                friendReference.SetValueAsync(true).ContinueWithOnMainThread(task =>
                {
                    if (!task.IsCompleted) { Debug.LogError("Failed to add friendReference: " + task.Exception); }
                });

                Dictionary<string, string> chatData = new Dictionary<string, string> { { Sender, Message } };
                DatabaseReference chatReference = db.RootReference.Child("Chat").Child(uniqueIdString).Push();
                chatReference.SetValueAsync(chatData).ContinueWithOnMainThread(task =>
                 {
                     if (!task.IsCompleted) { Debug.LogError("Failed to add chatReference: " + task.Exception); }
                     else { Debug.Log("Chat reference added successfully."); }
                 });
            }
            else
            {
                DatabaseReference friendReference = db.RootReference.Child("Player").Child(Getter).Child("Chat").Child(Sender).Child(response.ToString());
                friendReference.SetValueAsync(true).ContinueWithOnMainThread(task =>
                {
                    if (!task.IsCompleted) { Debug.LogError("Failed to add friendReference: " + task.Exception); }
                });


                Dictionary<string, string> chatData = new Dictionary<string, string> { { Sender, Message } };
                DatabaseReference chatReference = db.RootReference.Child("Chat").Child(response.ToString()).Push();
                chatReference.SetValueAsync(chatData).ContinueWithOnMainThread(task =>
                 {
                     if (!task.IsCompleted) { Debug.LogError("Failed to add chatReference: " + task.Exception); }
                     else { Debug.Log("Chat reference added successfully."); }
                 });
            }
        }));
    }
    public void ListenForChatChildChanges(string idChat, Action<Dictionary<string, object>> callback)
    {
        if (this.eventChatChildHandler == null)
        {
            this.chatChildReference = db.RootReference.Child("Chat").Child(idChat);
            this.eventChatChildHandler = (sender, args) => HandleChildChatChanged(sender, args, callback);
            this.chatChildReference.ChildAdded += eventChatChildHandler;
        }
    }

    public void UnListenForChatChildChanges()
    {
        if (eventChatChildHandler != null && chatChildReference != null)
        {
            this.chatChildReference.ChildAdded -= eventChatChildHandler;
            this.eventChatChildHandler = null;
            this.chatChildReference = null;
        }
    }

    public void ListenForNotificationChat(string Id, Action<Dictionary<string, object>> callback)
    {
        this.notificationChatReference = db.RootReference.Child("Player").Child(Id).Child("Chat");
        this.eventNotificationChatHandler = (sender, args) => HandleNotificationChatValueChanged(sender, args, callback);
        this.notificationChatReference.ValueChanged += eventNotificationChatHandler;
    }


    public void UnListenForNotificationChat()
    {
        if (this.eventNotificationChatHandler != null)
        {
            this.notificationChatReference.ValueChanged -= eventNotificationChatHandler;
            this.eventNotificationChatHandler = null;
            this.notificationChatReference = null;
        }
    }

    public void ChatIsRead(string Getter, string Sender, string idChat)
    {
        if (string.IsNullOrEmpty(idChat)) return;
        try
        {
            DatabaseReference friendReference = db.RootReference.Child("Player").Child(Getter).Child("Chat").Child(Sender).Child(idChat);
            friendReference.SetValueAsync(false).ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompleted)
                {
                    Debug.LogError("Failed to add friendReference: " + task.Exception);
                }
                else
                {
                    Debug.Log("ChatIsRead successfully updated.");
                }
            });
        }
        catch (ArgumentException ex)
        {
            Debug.LogError("ArgumentException: " + ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception: " + ex.Message);
        }
    }

    #endregion
    #region private Methods
    private void HandleNotificationChatValueChanged(object sender, ValueChangedEventArgs e, Action<Dictionary<string, object>> callback)
    {
        if (e.Snapshot != null && e.Snapshot.Value != null)
        {
            // Tạo dictionary để chứa dữ liệu từ chat
            Dictionary<string, object> chatData = new Dictionary<string, object>();

            // Lặp qua các con của snapshot
            foreach (DataSnapshot childSnapshot in e.Snapshot.Children)
            {
                // Lấy FriendID
                string friendID = childSnapshot.Key;

                // Lấy key-value trong FriendID
                Dictionary<string, object> friendData = childSnapshot.Value as Dictionary<string, object>;

                if (friendData != null)
                {
                    // Thêm dữ liệu của FriendID vào chatData
                    chatData[friendID] = friendData;
                }
            }

            // Gọi callback với dữ liệu đã xử lý
            callback(chatData);
        }
    }


    private void HandleChildChatChanged(object sender, ChildChangedEventArgs e, Action<Dictionary<string, object>> callback)
    {
        if (e.Snapshot != null && e.Snapshot.Value != null)
        {
            try
            {
                Dictionary<string, object> chatData = new Dictionary<string, object> { { e.Snapshot.Key, e.Snapshot.Value } };
                callback(chatData);
            }
            catch (Exception ex)
            {
                Debug.LogError("Có lỗi xảy ra khi xử lý dữ liệu: " + ex.Message + "\nStack Trace: " + ex.StackTrace);
            }
        }
        else
        {
            Debug.LogError("Lỗi HandleChildChatChanged: Snapshot hoặc Snapshot.Value là null.");
        }
    }


    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện khi script bị phá hủy
        if (connectedRef != null)
        {
            connectedRef.ValueChanged -= OnConnectionStatusChanged;
        }
    }
    #endregion


    #region Module Status Friends
    public void DenyAllInvite(string playerName)
    {
        DatabaseReference inviteChildReference = db.RootReference.Child("Player").Child(playerName).Child("Invite");
        inviteChildReference.RemoveValueAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log("Denyall");
        });
    }

    public void SetupPlayerStatusEvent(string playerName)
    {
        Dictionary<string, object> OnlineStatus = new Dictionary<string, object> { { "isOnline", true }, { "isInRoom", false } };
        Dictionary<string, object> OffineStatus = new Dictionary<string, object> { { "isOnline", false }, { "isInRoom", false } };
        DatabaseReference inviteChildDisconnectReference = db.RootReference.Child("Player").Child(playerName).Child("Invite");

        this.playerRef = db.GetReference("Status").Child(playerName);
        // Cập nhật trạng thái online
        this.playerRef.SetValueAsync(OnlineStatus);
        // Đặt trạng thái offline khi ngắt kết nối
        this.playerRef.OnDisconnect().SetValue(OffineStatus);
        inviteChildDisconnectReference.OnDisconnect().RemoveValue();
        // Theo dõi trạng thái kết nối của Firebase
        this.connectedRef = FirebaseDatabase.DefaultInstance.GetReference(".info/connected");
        this.connectedRef.ValueChanged += OnConnectionStatusChanged;
    }

    private void OnConnectionStatusChanged(object sender, ValueChangedEventArgs e)
    {
        if (e.DatabaseError != null)
        {
            Debug.LogError($"Database error: {e.DatabaseError.Message}");
            return;
        }

        bool isConnected = e.Snapshot.Value != null && (bool)e.Snapshot.Value;

        if (isConnected)
        {
            Dictionary<string, object> OnlineStatus = new Dictionary<string, object> { { "isOnline", true }, { "isInRoom", false } };
            Debug.Log("Player is connected to Firebase.");
            // Khi kết nối lại, đảm bảo trạng thái online được cập nhật
            playerRef.SetValueAsync(OnlineStatus);
        }
        else
        {
            Debug.Log("Player is disconnected from Firebase.");
        }
    }


    public void ListenForInviteValueChildChanges(string playerName, Action<Dictionary<string, string>> callback)
    {
        this.inviteChildReference = db.RootReference.Child("Player").Child(playerName).Child("Invite");
        this.eventInviteChildHandler = (sender, args) => HandleChildInviteChanged(sender, args, callback);
        this.inviteChildReference.ChildAdded += this.eventInviteChildHandler;
    }

    public void UnListenForInviteValueChildChanges()
    {
        if (this.eventInviteChildHandler != null)
        {
            this.inviteChildReference.ChildAdded -= eventInviteChildHandler;
            this.eventInviteChildHandler = null;
            this.inviteChildReference = null;
        }
    }


    private void HandleChildInviteChanged(object sender, ChildChangedEventArgs e, Action<Dictionary<string, string>> callback)
    {
        if (e.Snapshot != null && e.Snapshot.Value != null)
        {
            Debug.Log(e.Snapshot.Value + ", " + e.Snapshot.Key);
            try
            {
                string inviteValue = e.Snapshot.Value.ToString();
                Dictionary<string, string> inviteData = new Dictionary<string, string> { { e.Snapshot.Key, inviteValue } };
                callback(inviteData);
            }
            catch (Exception ex)
            {
                Debug.LogError("Có lỗi xảy ra khi chuyển đổi giá trị: " + ex.Message);
            }
        }
    }

    public void ListenForInviteRemoveChanges(string playerName, Action callback)
    {
        this.inviteRemoveReference = db.RootReference.Child("Player").Child(playerName).Child("Invite");
        this.eventRemoveChildHandler = (sender, args) => HandleChildInviteRemoved(sender, args, callback);
        this.inviteRemoveReference.ChildRemoved += this.eventRemoveChildHandler;
    }


    public void UnListenForInviteRemoveChanges()
    {
        if (this.eventRemoveChildHandler != null)
        {
            this.inviteRemoveReference.ChildRemoved -= this.eventRemoveChildHandler;
            this.eventRemoveChildHandler = null;
            this.inviteRemoveReference = null;
        }
    }


    private void HandleChildInviteRemoved(object sender, ChildChangedEventArgs e, Action callback)
    {
        if (e.Snapshot != null)
        {
            callback();
        }
    }

    public void AmIJoinRoom(string myName, bool isJoinRoom)
    {
        DatabaseReference roomStatusReference = db.RootReference.Child("Status").Child(myName).Child("isInRoom");
        roomStatusReference.SetValueAsync(isJoinRoom).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted) { Debug.Log($"AM I Join Room : {isJoinRoom}"); }
        });
    }

    public void ListenStatusFriend(Action<Dictionary<string, Dictionary<string, bool>>> callback)
    {
        this.statusChildReference = db.RootReference.Child("Status");
        this.eventStatusChildHandler = (sender, args) => HandleChildStatusChanged(sender, args, callback);
        this.statusChildReference.ValueChanged += this.eventStatusChildHandler;
    }

    public void UnListenStatusFriend()
    {
        if (this.eventStatusChildHandler != null)
        {
            this.statusChildReference.ValueChanged -= eventStatusChildHandler;
            this.eventStatusChildHandler = null;
            this.statusChildReference = null;
        }
    }


    public void HandleChildStatusChanged(object sender, ValueChangedEventArgs e, Action<Dictionary<string, Dictionary<string, bool>>> callback)
    {
        if (e.Snapshot != null)
        {
            Debug.Log("Snapshot không null");
            if (e.Snapshot.Value != null)
            {
                Debug.Log("Snapshot.Value không null");
                try
                {
                    Dictionary<string, Dictionary<string, bool>> statusData = new Dictionary<string, Dictionary<string, bool>>();
                    foreach (DataSnapshot childSnapshot in e.Snapshot.Children)
                    {
                        Dictionary<string, bool> childData = new Dictionary<string, bool>();
                        foreach (DataSnapshot grandChildSnapshot in childSnapshot.Children)
                        {
                            bool value = Convert.ToBoolean(grandChildSnapshot.Value);
                            childData.Add(grandChildSnapshot.Key, value);
                        }
                        statusData.Add(childSnapshot.Key, childData);
                    }
                    if (callback != null)
                    {
                        callback(statusData);
                    }
                    else
                    {
                        Debug.LogError("Callback is null.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Có lỗi xảy ra khi chuyển đổi giá trị: " + ex.Message + "\nStack Trace: " + ex.StackTrace);
                }
            }
            else
            {
                Debug.LogError("Snapshot.Value là null.");
            }
        }
        else
        {
            Debug.LogError("Snapshot là null.");
        }
    }



    #endregion
}