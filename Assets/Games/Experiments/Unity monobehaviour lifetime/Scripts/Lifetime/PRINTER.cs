using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PRINTER : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitThenPrintRoutine());
    }

    private IEnumerator WaitThenPrintRoutine()
    {
        yield return new WaitForSeconds(1f);

		LogLocatorItems();

		LogLocatorLogs();
    }

	void LogLocatorItems()
	{
		StringBuilder locatorLog = new StringBuilder();

		locatorLog.Append("[Test] Locator ITEM entries:\n");

		var instances = TESTLOCATOR.Instance.LISTITEMS();

		for (int i = 0; i < instances.Count; i++)
		{
			var instance = instances[i];

			if (instance == null)
				locatorLog.Append("[Test] {{Instance has been destroyed}}\n");
			else
				locatorLog.Append($"[Test] Instance exists. Random value: {{ {instance.randomValue} }} Name: {{ {instance.gameObject.name} }} Scene: {{ {instance.gameObject.scene.name} }}\n");
		}

		Debug.Log(locatorLog.ToString());
	}

	void LogLocatorLogs()
	{
		StringBuilder locatorLog = new StringBuilder();

		locatorLog.Append("[Test] Locator LOG entries:\n");

		var logs = TESTLOCATOR.Instance.LISTLOGS();

		for (int i = 0; i < logs.Count; i++)
		{
			var log = logs[i];

			locatorLog.Append("###########\n");

			locatorLog.Append(log);

			locatorLog.Append("\n");
		}

		locatorLog.Append("###########\n");

		Debug.Log(locatorLog.ToString());
	}
}