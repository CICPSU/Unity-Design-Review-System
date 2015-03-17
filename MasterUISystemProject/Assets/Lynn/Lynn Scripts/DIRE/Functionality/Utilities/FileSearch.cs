using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using UnityEngine;

class FileSearch : List <string>
{
	/// <summary>
	/// Default search for display definitions
	/// </summary>
	public static FileSearch DisplaySearch;
	//public static FileSearch ContentSearch;
	//public static FileSearch InputSearch;

	static FileSearch()
	{	
		string programDir = "/DIRE";

		/*
		Debug.Log("ApplicationData " + Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ) );
		Debug.Log("Common App Data "  +  Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData ) );
		Debug.Log("Local App Data "  + Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ) );
		Debug.Log("My Documents " + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments ) );
		Debug.Log("Personal " + Environment.GetFolderPath(  Environment.SpecialFolder.Personal ) );
		*/

		// Populate display search directory order.
		string displaySubDir = "/Settings/Displays";

		string inputSubDir = "/Settings/Inputs";

		DisplaySearch = new FileSearch();
		//search priority is C: + programDir > Directory.GetCurrentDirectory > Application.dataPath
		DisplaySearch.Add( Application.dataPath);
		DisplaySearch.Add( Directory.GetCurrentDirectory());
		DisplaySearch.Add("C:" + programDir);

		//allows specifying additional path in commandline
		if ( ArgumentProcessor.CmdLine.ContainsKey("displaypath") )
			foreach( string path in ArgumentProcessor.CmdLine["displaypath"] )
				DisplaySearch.Add( path );
		
		Debug.Log( "Display search order: " + Environment.NewLine + DisplaySearch );


		/*
		// Populate input search directory order.
		InputSearch = new FileSearch();
		InputSearch.Add( Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData ) + programDir + inputSubDir );
		InputSearch.Add( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ) + programDir + inputSubDir );
		InputSearch.Add( Environment.GetFolderPath( Environment.SpecialFolder.Personal ) + programDir + inputSubDir );
		InputSearch.Add( Directory.GetCurrentDirectory() + "/../.." + inputSubDir );
		InputSearch.Add( Directory.GetCurrentDirectory() + inputSubDir );
		InputSearch.Add( Directory.GetCurrentDirectory() + "/.." + inputSubDir );

		Debug.Log( "Input search order: " + Environment.NewLine + InputSearch );

		// Populate content search order
		string contentSubDir = "/Samples";

		ContentSearch = new FileSearch();
		ContentSearch.Add( Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData ) + programDir + contentSubDir );
		ContentSearch.Add( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ) + programDir + contentSubDir );
		ContentSearch.Add( Environment.GetFolderPath( Environment.SpecialFolder.Personal ) + programDir + contentSubDir );
		ContentSearch.Add( Directory.GetCurrentDirectory() + contentSubDir );
		ContentSearch.Add( Directory.GetCurrentDirectory() + "/.." + contentSubDir );
		ContentSearch.Add( Directory.GetCurrentDirectory() );
		*/

		//Debug.Log( "Content search order: " + Environment.NewLine + ContentSearch );
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
