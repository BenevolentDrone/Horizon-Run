using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Text;
using Random = System.Random;

namespace HereticalSolutions.Lifetime
{
    public class Test : MonoBehaviour, ISerializationCallbackReceiver
    {
        protected const int MAIN_THREAD_ID = 1;

        [SerializeField]
        protected GameObject serializedGameObject;
        
        public GameObject SerializedGameObject { get => serializedGameObject; }
        
        //static Thread mainThread = Thread.CurrentThread;
        

        protected StringBuilder logBuilder = new StringBuilder();


        [System.NonSerialized]
        public int randomValue = -1;
        
        public Test()
        {
            TESTLOCATOR.Instance.ADD(this);
            
			logBuilder.Append("[Test] {{ public Test() }} constructor call\n");

			logBuilder.Append($"[Test] Managed thread ID: {Thread.CurrentThread.ManagedThreadId}\n");

			logBuilder.Append($"[Test] Constructor is called from {(Thread.CurrentThread.ManagedThreadId == MAIN_THREAD_ID ? "MAIN" : "NOT MAIN")} thread\n");
            
            Random rnd = new Random();
            
            randomValue = rnd.Next();

			logBuilder.Append($"[Test] Random value: {randomValue}\n");

			if (Thread.CurrentThread.ManagedThreadId == MAIN_THREAD_ID)
			    logBuilder.Append($"[Test] Instance ID: {GetInstanceID()}\n");

			logBuilder.Append("-----------------------------------------\n");

			TESTLOCATOR.Instance.ADDLOG($"CONSTRUCTOR LOGPRINT:\n {logBuilder.ToString()}");
        }
        
        public Test(int customValue)
        {
			logBuilder.Append("[Test] {{ public Test(int customValue) }} CUSTOM constructor call\n");

			logBuilder.Append($"[Test] Managed thread ID: {Thread.CurrentThread.ManagedThreadId}\n");

			logBuilder.Append($"[Test] CUSTOM constructor is called from {(Thread.CurrentThread.ManagedThreadId == MAIN_THREAD_ID ? "MAIN" : "NOT MAIN")} thread\n");

			Random rnd = new Random();

			randomValue = rnd.Next();

			logBuilder.Append($"[Test] Random value: {randomValue}\n");

            if (Thread.CurrentThread.ManagedThreadId == MAIN_THREAD_ID)
			    logBuilder.Append($"[Test] Intance ID: {GetInstanceID()}\n");

			logBuilder.Append("-----------------------------------------\n");

			TESTLOCATOR.Instance.ADDLOG($"CUSTOM CONSTRUCTOR LOGPRINT:\n {logBuilder.ToString()}");
        }
        
        public void OnBeforeSerialize()
        {
			logBuilder.Append("[Test] OnBeforeSerialize() is called\n");
			logBuilder.Append($"[Test] Managed thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
			logBuilder.Append("-----------------------------------------\n");

			TESTLOCATOR.Instance.ADDLOG($"ON_BEFORE_SERIALIZE LOGPRINT:\n {logBuilder.ToString()}");
        }

        public void OnAfterDeserialize()
        {
			logBuilder.Append("[Test] OnAfterDeserialize() is called\n");
			logBuilder.Append($"[Test] Managed thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
			logBuilder.Append($"[Test] Serialized game object deserialization: {(serializedGameObject != null ? "SUCCESS" : "FAILURE")}\n");
			logBuilder.Append("-----------------------------------------\n");

			TESTLOCATOR.Instance.ADDLOG($"ON_AFTER_SERIALIZE LOGPRINT:\n {logBuilder.ToString()}");
        }

		void Awake()
		{
			logBuilder.Append("[Test] Awake() is called\n");
			logBuilder.Append("-----------------------------------------\n");

			Debug.Log($"[Test] AWAKE LOGPRINT FOR GAMEOBJECT {{ {gameObject.name} }}:\n{logBuilder.ToString()}");
		}

		void OnEnable()
		{
			logBuilder.Append("[Test] OnEnable() is called\n");
			logBuilder.Append("-----------------------------------------\n");
		}
        
        void Start()
        {
			logBuilder.Append("[Test] Start() is called\n");

			logBuilder.Append("-----------------------------------------\n");


			TESTLOCATOR.Instance.ADDLOG($"START LOGPRINT:\n {logBuilder.ToString()}");

			//Debug.Log($"[Test] START LOGPRINT FOR GAMEOBJECT {{ {gameObject.name} }}:\n{logBuilder.ToString()}");
        }

        void OnValidate()
        {
			logBuilder.Append("[Test] OnValidate() is called\n");
			logBuilder.Append($"[Test] Managed thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
			logBuilder.Append("-----------------------------------------\n");
        }
    }
}