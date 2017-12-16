using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class FileSearch : List <string>
{


	static FileSearch()
	{	
	}

    public List <string> FindFile(string pattern)
    {
		List<string> paths = new List<string>();

        foreach (string searchPath in this)
        {
            try
            {
				if ( Directory.Exists( searchPath ))
				{
	                string[] files = Directory.GetFiles(searchPath, pattern);
	                foreach (string fname in files)
						paths.Add( fname );
				}
			}
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        return (paths);
    }

	public override string ToString ()
	{
		string val = "";

		if ( Count <= 0 )
			return( "(empty)" );
		else
		{
			foreach( string s in this )
				val += s + Environment.NewLine;
		}

		return( val );
	}
}
