using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

public class DIREEditor
{
	static string buildDir    = "../Bin";
	static string buildExe    = buildDir + "/DIRE.exe";
	static string settingsDir = "../Settings";
	static string packageDir = "../Packages";

	[MenuItem("DIRE/Build All")]
	static void BuildAll()
	{
		ClearAll();
		BuildPlayer();
		ExportPackages();
	}

	[MenuItem("DIRE/Clear All")]
	static void ClearAll()
	{
		DeletePlayer();
		DeletePackages();
	}

	static void DeletePlayer()
	{
		FileUtil.DeleteFileOrDirectory( buildDir );
	}

	static void BuildPlayerFromScene( string scene )
	{
		DeletePlayer();
		
		Directory.CreateDirectory( buildDir );
		string[] DIREScene = new string[] { scene };
		
		PlayerSettings.companyName = "ARL Penn State University";
		PlayerSettings.productName = "DIRE";
		PlayerSettings.runInBackground = true;
		
		BuildPipeline.BuildPlayer(DIREScene, buildExe, BuildTarget.StandaloneWindows64, BuildOptions.None );
		//FileUtil.CopyFileOrDirectory( settingsDir, buildDir + "/Settings" );
	}

	[MenuItem("DIRE/Build/Player")]
	static void BuildPlayer()
	{
		BuildPlayerFromScene("Assets/DIRE/Scenes/DIRE.unity");
	}

	[MenuItem("DIRE/Build Current As DIRE")]
	static void BuildCurrent()
	{
		BuildPlayerFromScene (EditorApplication.currentScene);
	}

	static void DeletePackages()
	{
		FileUtil.DeleteFileOrDirectory( packageDir ); 
	}

	[MenuItem("DIRE/Build/Packages")]
	static void ExportPackages () 
	{
		DeletePackages();

		Directory.CreateDirectory( packageDir );

		string [] DIREExportAssets = 
		{ 
			"Assets/DIRE/Public"
		};
		string DIREExportFile = packageDir + "/DIRE.unitypackage";
		
		AssetDatabase.ExportPackage( DIREExportAssets, DIREExportFile, ExportPackageOptions.Recurse );
		
	}

}
