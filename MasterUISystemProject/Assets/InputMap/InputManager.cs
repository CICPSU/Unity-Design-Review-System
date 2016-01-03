using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


public class InputManager : MonoBehaviour 
{
	static public InputMap map = new InputMap();

	static InputManager()
	{
		IEnumerable<Type> deviceTypes = from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
		                                from assemblyType in domainAssembly.GetTypes()
		                                where assemblyType.IsSubclassOf( typeof( Device ) )
		                                select assemblyType;
		foreach( Type t in deviceTypes )
			if ( !XmlIO.AdditionalTypes.Contains(t) )
				XmlIO.AdditionalTypes.Add( t );
	}

	static public void LoadMap( string path )
	{
		InputMap newMap = XmlIO.Load( path, typeof(InputMap)) as InputMap;
		newMap.Configure();
		map.Append( newMap ); 
	}

	public bool LoadDefinitionFiles = false;
	public List<string> InputFiles = new List<string>();
	
	void Start()
	{

        InputFiles = (from f in Directory.GetFiles(Application.dataPath + "/FullPackage/Settings").ToList<string>()
                      where f.Substring(f.Length - 6) == ".input"
                      select f).ToList<string>();
        Debug.Log(InputFiles);
        foreach (string path in InputFiles)
            LoadMap(path);
        /*
		if ( LoadDefinitionFiles )
			foreach( string path in InputFiles )
				LoadMap( path );
                */
    }
}
