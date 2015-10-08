using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Processese the application's command line parameters into a dictionary of 
/// keys/values.  Each key will contain a list of associated string values.  Keys
/// are denoted with a "-" or "/" preceeding the key id.
/// 
/// Eg.  -key1 key1Value1 key1Value2 -key2 key2Value1 -key3 key3Value1 Key3Value2
/// 
/// This will create a dictionary with three key values (key1, key2, and key3).  Key1 
/// Key3 each has 2 values.  Key2 has a single value
/// </summary>
/// 
public class ArgumentProcessor : Dictionary< string, List<string> >
{
    /// <summary>
    /// Default instance of the argument processor for the currently running
    /// application.  
    /// </summary>
	public static ArgumentProcessor CmdLine;

    /// <summary>
    /// Static class constructor.  Creates the argument processor for the currently
    /// running application.  Arguments are retrieved from the System.Environment 
    /// class.
    /// </summary>
	static ArgumentProcessor()
	{
		CmdLine = new ArgumentProcessor( System.Environment.GetCommandLineArgs() );
	}

    /// <summary>
    /// Argument processor constructor.  Takes a list of string argurments and segments
    /// them into key/values pairs.
    /// </summary>
    /// <param name="args">List of (command line) argument strings.</param>
    public ArgumentProcessor(string[] args)
    {
        // create default list of unassociated parameters.  These are arguments that
        // have preceeded the first defined key.
        string key = "";
        List<string> values = new List<string>();
        Add(key, values);

        ///
        ///  Loop through each argument in the argument list.  If the argument is 
        ///  a key, create a new argument list with that key's index.  Otherwise,
        ///  place the value into the last defined key's value list.
        ///  
        foreach (string arg in args)
        {
            string s = arg.ToLower();

            if (s.StartsWith("-") || s.StartsWith("/"))
            {
                key = s.Substring(1);
                values = new List<string>();
                Add(key, values);
            }
            else
            {
                values.Add(arg);
            }
        }
    }
}
