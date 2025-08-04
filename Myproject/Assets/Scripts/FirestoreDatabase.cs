using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using System;
using Firebase.Extensions;
using System.Reflection;
public class FirestoreDatabase : MonoBehaviour
{
    public static FirestoreDatabase Instance { get; private set; }
    private static FirebaseFirestore db;
    private string nameCollection = "QAPair";
    #region private Fields
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

    private void Start()
    {
        this.Reset();
    }
    public void Reset()
    {
        FirestoreDatabase.Instance.GetQAPairsAsync((listqa) =>
        {
            foreach (QAPair QA in listqa)
            {
                QAPairFB qAPairFB = new QAPairFB();
                qAPairFB.Question = QA.Question;
                qAPairFB.Answer = QA.Answer;
                qAPairFB.HardPoint = QA.HardPoint;
                qAPairFB.Category = QA.Category;
                if (QA.Type.Equals("Chọn đáp án"))
                {
                    qAPairFB.Type = "Multiple";
                    FirestoreDatabase.Instance.UpdateAsync<QAPairFB>(QA.id, qAPairFB, (qaFB) =>
                    {

                    });
                }
                else if (QA.Type.Equals("Nhập đáp án"))
                {
                    qAPairFB.Type = "Text";
                    FirestoreDatabase.Instance.UpdateAsync<QAPairFB>(QA.id, qAPairFB, (qaFB) =>
                    {

                    });
                }
            }
        });
    }

    public void GetQAPairsAsync(Action<List<QAPair>> callback)
    {
        List<QAPair> list = new List<QAPair>();
        CollectionReference colRef = db.Collection(this.nameCollection);

        colRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Lỗi khi lấy dữ liệu từ Firestore: " + task.Exception);
                callback(null);
                return;
            }

            QuerySnapshot querySnapshot = task.Result;

            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                QAPair qaPair = new QAPair();
                Dictionary<string, object> dicObject = documentSnapshot.ToDictionary();

                // Gán documentID từ Firestore vào trường `id`
                qaPair.id = documentSnapshot.Id;

                foreach (KeyValuePair<string, object> pair in dicObject)
                {
                    PropertyInfo property = typeof(QAPair).GetProperty(pair.Key);
                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(qaPair, Convert.ChangeType(pair.Value, property.PropertyType), null);
                    }
                }

                list.Add(qaPair);
            }

            callback(list);
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

    public void DeleteAsync<T>(string documentId, Action<bool> callback) where T : new()
    {
        CollectionReference colRef = db.Collection(this.nameCollection);

        // Tham chiếu đến tài liệu có ID cụ thể
        DocumentReference docRef = colRef.Document(documentId);

        // Thực hiện xóa tài liệu
        docRef.DeleteAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Lỗi khi xóa tài liệu từ Firestore: " + task.Exception);
                callback(false); // Gọi callback với giá trị false nếu lỗi
                return;
            }

            // Thành công, gọi callback với giá trị true
            callback(true);
        });
    }
}
