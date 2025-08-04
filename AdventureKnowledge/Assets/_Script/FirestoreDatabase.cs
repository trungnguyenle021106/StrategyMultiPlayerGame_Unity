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

public class FirestoreDatabase : MonoBehaviour
{
    public static FirestoreDatabase Instance { get; private set; }
    private static FirebaseFirestore db;
    #region private Fields
    private string nameCollection;
    #endregion
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            db = FirebaseFirestore.DefaultInstance;
            db.Settings.PersistenceEnabled = false;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #region public Methods
    public void GetListCountAsync<T>(Action<int> callback)
    {
        CollectionReference colRef = db.Collection(this.nameCollection);
        colRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot querySnapshot = task.Result;
            int count = querySnapshot.Count;
            callback(count);
        });
    }
    public void GetListAsync<T>(Action<List<T>> callback) where T : new()
    {
        List<T> list = new List<T>();
        CollectionReference colRef = db.Collection(this.nameCollection);
        colRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot querySnapshot = task.Result;
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                T obj = new T();
                Dictionary<string, object> dicObject = documentSnapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in dicObject)
                {
                    PropertyInfo property = typeof(T).GetProperty(pair.Key);
                    if (property != null && property.CanWrite) { property.SetValue(obj, Convert.ChangeType(pair.Value, property.PropertyType), null); }
                }
                list.Add(obj);
            }
            callback(list);
        });
    }
    public void AddAsync<T>(T entity, Action<T> callback) where T : new()
    {
        CollectionReference colRef = db.Collection(this.nameCollection);

        // Chuyển đối tượng entity thành từ điển
        Dictionary<string, object> entityDict = new Dictionary<string, object>();

        foreach (PropertyInfo property in typeof(T).GetProperties())
        {
            if (property.CanRead)
            {
                var value = property.GetValue(entity);
                entityDict.Add(property.Name, value);
            }
        }

        // Thêm tài liệu mới vào Firestore
        colRef.AddAsync(entityDict).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Lỗi khi thêm tài liệu vào Firestore: " + task.Exception);
                callback(default(T)); // Gọi callback với giá trị mặc định nếu lỗi
                return;
            }

            // Thành công, gọi callback với đối tượng vừa thêm
            callback(entity);
        });
    }
    public void AddAccountAsync(string name, Action<Account> callback)
    {
        Account account = new Account(); // Khởi tạo đối tượng Account
        CollectionReference colRef = db.Collection("Account");
        DateTime tomorrow = DateTime.UtcNow.AddDays(1);
        DateTime nextResetTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 0, DateTimeKind.Utc);
        // Chuẩn bị dữ liệu để thêm vào Firestore

        AccountFB accountFB = new AccountFB();
        accountFB.Name = name;
        accountFB.Energy = 20;
        accountFB.ADViewTime = 5;
        accountFB.Characters = "Rock Golem";
        accountFB.Coins = 100;
        accountFB.LastLoginTime = Timestamp.FromDateTime(nextResetTime);
        accountFB.RankPoint = 0;
        // Thêm tài liệu mới vào Firestore
        colRef.AddAsync(accountFB).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Lỗi khi thêm tài liệu vào Firestore: " + task.Exception);
                callback(null); // Gọi callback với giá trị mặc định nếu lỗi
                return;
            }

            // Nếu thành công, lấy DocumentReference của tài liệu mới
            DocumentReference docRef = task.Result;

            // Lấy dữ liệu của tài liệu vừa thêm
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(snapshotTask =>
            {
                if (snapshotTask.IsFaulted || snapshotTask.IsCanceled)
                {
                    Debug.LogError("Lỗi khi lấy dữ liệu từ Firestore: " + snapshotTask.Exception);
                    callback(null); // Gọi callback với giá trị mặc định nếu lỗi
                    return;
                }

                // Lấy DocumentSnapshot
                DocumentSnapshot documentSnapshot = snapshotTask.Result;

                if (documentSnapshot.Exists)
                {
                    // Chuyển dữ liệu từ DocumentSnapshot sang đối tượng Account
                    Dictionary<string, object> refAccountFB = documentSnapshot.ToDictionary();
                    account.id = documentSnapshot.Id;
                    account.Name = refAccountFB["Name"].ToString();
                    account.Energy = Convert.ToInt32(refAccountFB["Energy"]);
                    account.Coins = Convert.ToInt32(refAccountFB["Coins"]);
                    account.Characters = refAccountFB["Characters"].ToString();
                    account.ADViewTime = Convert.ToInt32(refAccountFB["ADViewTime"]);
                    account.LastLoginTime = (Timestamp)refAccountFB["LastLoginTime"];
                    account.RankPoint = Convert.ToInt32(refAccountFB["RankPoint"]);
                    // Gọi callback với đối tượng Account đã được điền dữ liệu
                    callback(account);
                }
                else
                {
                    Debug.LogError("Tài liệu không tồn tại sau khi thêm.");
                    callback(null); // Gọi callback với giá trị mặc định nếu không tìm thấy tài liệu
                }
            });
        });
    }
    public void GetAccountAsyncByName(string name, Action<Account> callback)
    {
        CollectionReference colRef = db.Collection("Account");
        colRef.WhereEqualTo("Name", name).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot querySnapshot = task.Result;

            if (querySnapshot.Documents.Count() > 0)
            {
                // Lấy tài liệu đầu tiên khớp với điều kiện
                DocumentSnapshot documentSnapshot = querySnapshot.Documents.ToList()[0];

                // Tạo đối tượng Account từ tài liệu
                Account account = new Account
                {
                    id = documentSnapshot.Id,
                    Name = documentSnapshot.GetValue<string>("Name"),
                    Energy = documentSnapshot.GetValue<int>("Energy"),
                    Coins = documentSnapshot.GetValue<int>("Coins"),
                    Characters = documentSnapshot.GetValue<string>("Characters"),
                    LastLoginTime = documentSnapshot.GetValue<Timestamp>("LastLoginTime"),
                    ADViewTime = documentSnapshot.GetValue<int>("ADViewTime"),
                    RankPoint = documentSnapshot.GetValue<int>("RankPoint"),
                };

                callback(account); // Trả về tài khoản tìm được
            }
            else
            {
                callback(null); // Không tìm thấy tài khoản
            }
        });
    }
    public void SetNameCollection(string nameCollection)
    {
        this.nameCollection = nameCollection;
    }
    public void UpdateAsync<T>(string documentId, T entity, Action<T> callback) where T : new()
    {
        DocumentReference docRef = db.Collection(this.nameCollection).Document(documentId);

        // Chuyển đối tượng entity thành từ điển để cập nhật vào Firestore
        Dictionary<string, object> entityDict = new Dictionary<string, object>();

        foreach (PropertyInfo property in typeof(T).GetProperties())
        {
            if (property.CanRead)
            {
                var value = property.GetValue(entity);
                entityDict.Add(property.Name, value);
            }
        }

        // Cập nhật tài liệu với UpdateAsync
        docRef.UpdateAsync(entityDict).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Lỗi khi cập nhật tài liệu vào Firestore: " + task.Exception);
                callback(default(T)); // Gọi callback với giá trị mặc định nếu lỗi
                return;
            }

            // Thành công, gọi callback với đối tượng đã cập nhật
            callback(entity);
        });
    }
    #endregion
}
