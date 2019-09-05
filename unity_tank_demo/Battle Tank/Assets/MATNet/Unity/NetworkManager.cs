using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MATNet.Unity
{
    class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance;
        private MNManager netManager;

        public GameObject[] networkObjects;
        public string gameSceneName;
        public bool inGame;

        private Dictionary<int, GameObject> myObjsToBeSync;
        private List<string> myInstantiatedObjIds;
        private string mySyncRequestId;
        private List<string> sendDataRequestId;

        private List<InstantiateDataContainer> instantiateOnNext;

        private bool syncRequest;
        private string syncRequestId;

        private bool loadGameSceneOnNext;

        public class InstantiateDataContainer
        {
            public string id { get; set; }
            public int netObjId { get; set; }
            public Vector3 position { get; set; }
            public Quaternion rotation { get; set; }
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
            MATNet.MNTools.DebugLogEvent += NetManager_DebugLog;
            MATNet.MNTools.DebugLogErrorEvent += MNTools_DebugLogErrorEvent;
            netManager = MNManager.Instance;
        }

        private void MNTools_DebugLogErrorEvent(object message)
        {
            Debug.LogError(message);
        }

        private void NetManager_DebugLog(object message)
        {
            Debug.Log(message);
        }

        public void Start()
        {
            myObjsToBeSync = new Dictionary<int, GameObject>();
            myInstantiatedObjIds = new List<string>();
            instantiateOnNext = new List<InstantiateDataContainer>();
            sendDataRequestId = new List<string>();
            netManager.OnStart();

            netManager.RegisterRpcMethod("InstantiateGObjFromNet", this, "InstantiateGObjFromNet");
            netManager.RegisterRpcMethod("SyncGameObj", this, "SyncGameObj");
            netManager.RegisterRpcMethod("RequestSyncGObj", this, "RequestSyncGObj");

            netManager.OnJoiningRoomDataChangedEvent += NetManager_OnJoiningRoomDataChangedEvent;
        }

        private void NetManager_OnJoiningRoomDataChangedEvent(MNRoomData roomData)
        {
            if (netManager.CurrentStatus == MNManager.Status.Playing)
            {
                loadGameSceneOnNext = true;
            }
        }

        public void Update()
        {
            if (instantiateOnNext.Count > 0 && !myInstantiatedObjIds.Contains(instantiateOnNext[0].id))
            {
                Debug.Log("instantiate from list id:" + instantiateOnNext[0].id);
                myInstantiatedObjIds.Add(instantiateOnNext[0].id);
                GameObject obj = Instantiate(networkObjects[instantiateOnNext[0].netObjId], instantiateOnNext[0].position, instantiateOnNext[0].rotation);
                obj.name = instantiateOnNext[0].id;
                instantiateOnNext.RemoveAt(0);
            }
            if (syncRequest)
            {
                Debug.Log("sync mes send by req");
                syncRequest = false;
                sendDataRequestId.Add(syncRequestId);
                foreach (var pair in myObjsToBeSync)
                {
                    Vector3 pos = pair.Value.transform.position;
                    Quaternion rotation = pair.Value.transform.rotation;
                    netManager.SendRPC("SyncGameObj", syncRequestId, pair.Value.name, pair.Key, pos.x, pos.y, pos.z, rotation.x, rotation.y, rotation.z, rotation.w);
                }
            }
            if (loadGameSceneOnNext)
            {
                loadGameSceneOnNext = false;
                if (SceneManager.GetActiveScene().name != gameSceneName)
                {
                    Invoke("LoadGameScene", 2f);
                }
            }
        }

        void LoadGameScene()
        {
            SceneManager.LoadScene(gameSceneName);
            inGame = true;
        }

        public void OnDestroy()
        {
            netManager.OnDestroy();
        }

        public GameObject InstantiateGameObj(string id, int netObjId, Vector3 position, Quaternion rotation)
        {
            if (netManager.CurrentStatus != MNManager.Status.Playing)
            {
                Debug.Log("ゲームが開始されていません！");
                return null;
            }
            myInstantiatedObjIds.Add(id);
            netManager.SendRPC("InstantiateGObjFromNet", id, netObjId, position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w);
            Debug.Log("SendRPC inst gameobj id:"+id);
            GameObject obj = Instantiate(networkObjects[netObjId], position, rotation);
            obj.name = id;
            return obj;
        }
        public GameObject InstantiateGameObj(int netObjId, Vector3 position, Quaternion rotation)
        {
            return InstantiateGameObj(Guid.NewGuid().ToString(), netObjId, position, rotation);
        }

        public void InstantiateGObjFromNet(string id, int netObjId, float x, float y, float z, float rx, float ry, float rz, float rw)
        {
            if (!inGame || myInstantiatedObjIds.Contains(id)) { return; }
            Debug.Log("inst obj from net id:" + id);
            InstantiateDataContainer container = new InstantiateDataContainer { id = id, netObjId = netObjId, position = new Vector3(x,y,z), rotation = new Quaternion(rx,ry,rz,rw) };
            instantiateOnNext.Add(container);
        }

        public void SyncGameObj(string requestId, string id, int netObjId, float x, float y, float z, float rx, float ry, float rz, float rw)
        {
            if (!inGame || sendDataRequestId.Contains(requestId) || myInstantiatedObjIds.Contains(id)) { return; }
            Debug.Log("sync obj id:" + id);
            InstantiateDataContainer container = new InstantiateDataContainer { id = id, netObjId = netObjId, position = new Vector3(x, y, z), rotation = new Quaternion(rx, ry, rz, rw) };
            instantiateOnNext.Add(container);
        }

        public void RequestSyncGObj(string requestId)
        {
            if (!inGame || requestId == mySyncRequestId) { return; }
            Debug.Log("req sync obj reqID:"+requestId);
            syncRequest = true;
            syncRequestId = requestId;
        }

        public void RegisterObjToBeSync(int netObjId, GameObject obj)
        {
            myObjsToBeSync.Add(netObjId, obj);
        }

        public void SendSyncRequest()
        {
            mySyncRequestId = Guid.NewGuid().ToString();
            Debug.Log("send sync req reqId:" + mySyncRequestId);
            netManager.SendRPC("RequestSyncGObj", mySyncRequestId);
        }
    }
}
