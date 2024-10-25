using HeresySolutions.Lifetime;
using System.Collections.Generic;

public class TESTLOCATOR
{
    private List<Test> items = new List<Test>(10);

	private List<string> logs = new List<string>(10);

    private System.Object lockObject = new System.Object();

    private static TESTLOCATOR instance;
    
    public static TESTLOCATOR Instance
    {
        get
        {
            if (instance == null)
                instance = new TESTLOCATOR();
                
            return instance;
        }
    }
    
    public void ADD(Test item)
    {
        items.Add(item);
    }
    
    public List<Test> LISTITEMS()
    {
        return items;
    }

	public void ADDLOG(string log)
	{
        lock (lockObject)
        {
		    logs.Add(log);
        }
	}

	public List<string> LISTLOGS()
	{
		return logs;
	}
}