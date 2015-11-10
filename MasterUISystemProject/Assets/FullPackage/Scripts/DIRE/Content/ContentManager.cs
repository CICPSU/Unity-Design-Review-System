
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;


/// <summary>
/// Loads content data at runtime via Unity asset bundles.
/// </summary>
public class ContentManager : MonoBehaviour
{
    public bool LoadPluginsWithContent = false;
    public bool LoadPreferencesWithContent = true;

	public string BundlePath = null;
	public string SceneName = null;
	public bool LoadTrigger = false;

    /// <summary>
    /// Get the paths to all the content data and load each in order
    /// </summary>
    void Awake()
    {
        if (DIRE.ContentOption.Count() >= 1)
		{
			BundlePath = DIRE.ContentOption[0];
			LoadTrigger = true;

			if (DIRE.ContentOption.Count() >= 2)
				SceneName = DIRE.ContentOption[1];
		}
        else
            MessageHandler.Message("System", "No dynamic content requested");
    }

    void Update()
    {
        if (LoadTrigger)
            LoadContent(BundlePath, SceneName);
        LoadTrigger = false;
    }

    /// <summary>
    /// Load content data.
    /// </summary>
    /// <param name="path">Path to Unity asset bundle</param>
    /// <returns></returns>

	public string [] scenes;

	public void LoadContent(string path, string scene)
    {
		if ( scene.Trim().Length <= 0 )
			scene = null;

        string directory = Path.GetDirectoryName(path);

        if (LoadPluginsWithContent)
        {
            /// Load plugins
            foreach (string plugin in Directory.GetFiles(directory, "*.dll"))
            {
				MessageHandler.Message("System", "Attempting to load: " + plugin );
                System.Reflection.Assembly assembly =
                        System.Reflection.Assembly.LoadFrom(plugin);
				MessageHandler.Message("System", "Assembly loaded: " + assembly.FullName );
			}
        }

        /// Load asset bundle
		MessageHandler.Message("System", "Loading asset bundle: " + path);
        AssetBundle bundle = AssetBundle.LoadFromFile(path);

		scenes = bundle.GetAllScenePaths();
		if ( scenes != null && scenes.Length > 0 )
		{
			string sceneList = "";
			foreach( string s in scenes )
				sceneList += s + Environment.NewLine;

			MessageHandler.Message ("System", "Scenes: " + Environment.NewLine + sceneList );

	    	if ( scene == null )
				scene = Path.GetFileNameWithoutExtension( scenes[0] );
		}
		else
			MessageHandler.Message("System", "Asset bundle contains no scenes");

		/// Load scene
		if ( scene == null )
			MessageHandler.Message("System", "No scene to load.");
		else
		{
			MessageHandler.Message ("System", "Loading scene: " + scene );
	        Application.LoadLevel(scene);
		}

        if (LoadPreferencesWithContent)
        {
            /// Load all preference files in content directory
            /// No guarentee on load order, so conflicting preferences are bad
            foreach (string pref in Directory.GetFiles(directory, "*" + DIRE.PreferenceExtension))
                Preferences.Load(pref);
        }
	}
}
