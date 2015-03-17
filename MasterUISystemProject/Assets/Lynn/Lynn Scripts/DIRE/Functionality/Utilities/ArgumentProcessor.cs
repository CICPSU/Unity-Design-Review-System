using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ArgumentProcessor : Dictionary< string, List<string> >
{
	public static ArgumentProcessor CmdLine;

	static ArgumentProcessor()
	{
		CmdLine = new ArgumentProcessor( System.Environment.GetCommandLineArgs() );
	}

    public ArgumentProcessor(string[] args)
    {
        // create default list of unassociated parameters
        string key = "";
        List<string> values = new List<string>();
        Add(key, values);

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
