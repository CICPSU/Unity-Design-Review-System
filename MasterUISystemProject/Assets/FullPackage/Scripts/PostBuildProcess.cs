﻿#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

//Post build event class that moves settings files to designated folder.
public static class PostBuildProcess {

	[PostProcessBuildAttribute(1)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {

		string forBuildFolderPath;
		string desFolderPath;
		if(System.Environment.OSVersion.Platform == System.PlatformID.MacOSX 
		   || System.Environment.OSVersion.Platform == System.PlatformID.Unix){
		    forBuildFolderPath = Application.dataPath + @"/FullPackage/Settings/For Build";
			desFolderPath = pathToBuiltProject + @"/Settings";
		}else{
			forBuildFolderPath = Application.dataPath + @"\FullPackage\Settings\For Build";
			desFolderPath = pathToBuiltProject + @"\Settings";
		}
		 
		if(Directory.Exists(forBuildFolderPath)){
			CopyDirectory(forBuildFolderPath, desFolderPath, true);
		}else{
			Debug.LogError(forBuildFolderPath + " does not exist! settings are note copied to executable!");
			return;
		}
	}

	//<summary>
	//copy all the files from sourceDirName to destDirName folder, sub directories will be copied if copySubDirs is true
	//</summary>
	public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs){

		// Get the subdirectories for the specified directory.
		DirectoryInfo dir = new DirectoryInfo(sourceDirName);
		
		if (!dir.Exists)
		{
			throw new DirectoryNotFoundException(
				"Source directory does not exist or could not be found: "
				+ sourceDirName);
		}
		
		DirectoryInfo[] dirs = dir.GetDirectories();
		// If the destination directory doesn't exist, create it.
		if (!Directory.Exists(destDirName))
		{
			Directory.CreateDirectory(destDirName);
		}
		
		// Get the files in the directory and copy them to the new location.
		FileInfo[] files = dir.GetFiles();
		foreach (FileInfo file in files)
		{
			string temppath = Path.Combine(destDirName, file.Name);
			file.CopyTo(temppath, false);
		}
		
		// If copying subdirectories, copy them and their contents to new location.
		if (copySubDirs)
		{
			foreach (DirectoryInfo subdir in dirs)
			{
				string temppath = Path.Combine(destDirName, subdir.Name);
				CopyDirectory(subdir.FullName, temppath, copySubDirs);
			}
		}
	}
}

#endif