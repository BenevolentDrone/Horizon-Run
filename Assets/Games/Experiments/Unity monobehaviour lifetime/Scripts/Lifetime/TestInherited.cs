using UnityEngine;
using System.Threading;
using System.Text;

namespace HereticalSolutions.Lifetime
{
    public class TestInherited : Test, ISerializationCallbackReceiver
    {
        public TestInherited() : base(-1)
        {
            TESTLOCATOR.Instance.ADD(this);

			logBuilder.Append("[TestInherited] {{ public TestInherited() : base(-1) }} INHERITED constructor call\n");

			logBuilder.Append($"[TestInherited] Managed thread ID: {Thread.CurrentThread.ManagedThreadId}\n");

			logBuilder.Append($"[TestInherited] INHERITED constructor is called from {(Thread.CurrentThread.ManagedThreadId == MAIN_THREAD_ID ? "MAIN" : "NOT MAIN")} thread\n");

			TESTLOCATOR.Instance.ADDLOG($"INHERITED CONSTRUCTOR LOGPRINT:\n {logBuilder.ToString()}");
        }

		public new void OnBeforeSerialize()
		{
			logBuilder.Append("[TestInherited] OnBeforeSerialize() is called\n");
			logBuilder.Append($"[TestInherited] Managed thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
			logBuilder.Append("-----------------------------------------\n");

			TESTLOCATOR.Instance.ADDLOG($"ON_BEFORE_SERIALIZE LOGPRINT:\n {logBuilder.ToString()}");
		}

		public new void OnAfterDeserialize()
		{
			logBuilder.Append("[TestInherited] OnAfterDeserialize() is called\n");
			logBuilder.Append($"[TestInherited] Managed thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
			logBuilder.Append($"[TestInherited] Serialized game object deserialization: {(serializedGameObject != null ? "SUCCESS" : "FAILURE")}\n");
			logBuilder.Append("-----------------------------------------\n");

			TESTLOCATOR.Instance.ADDLOG($"ON_AFTER_SERIALIZE LOGPRINT:\n {logBuilder.ToString()}");
		}

		void Awake()
		{
			logBuilder.Append("[TestInherited] Awake() is called\n");
			logBuilder.Append("-----------------------------------------\n");

			Debug.Log($"[TestInherited] AWAKE LOGPRINT FOR GAMEOBJECT {{ {gameObject.name} }}:\n{logBuilder.ToString()}");
		}

		void OnEnable()
		{
			logBuilder.Append("[TestInherited] OnEnable() is called\n");
			logBuilder.Append("-----------------------------------------\n");
		}

		void Start()
		{
			logBuilder.Append("[TestInherited] Start() is called\n");

			logBuilder.Append("-----------------------------------------\n");


			Debug.Log($"[TestInherited] START LOGPRINT FOR GAMEOBJECT {{ {gameObject.name} }}:\n{logBuilder.ToString()}");
		}

		void OnValidate()
		{
			logBuilder.Append("[TestInherited] OnValidate() is called\n");
			logBuilder.Append($"[TestInherited] Managed thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
			logBuilder.Append("-----------------------------------------\n");
		}
    }
}