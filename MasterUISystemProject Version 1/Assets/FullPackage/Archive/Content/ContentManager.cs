
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
            Debug.Log("No dynamic content requested");
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
                Debug.Log("Attempting to load: " + plugin);
                System.Reflection.Assembly assembly =
                        System.Reflection.Assembly.LoadFrom(plugin);
                Debug.Log("Assembly loaded: " + assembly.FullName);
			}
        }

        /// Load asset bundle
		Debug.Log("Loading asset bundle: " + path);
        //TODO: change to 5.2 api AssetBundle bundle = AssetBundle.LoadFromFile(path);

        //TODO: uncomment scenes = bundle.GetAllScenePaths();
        if (scenes != null && scenes.Length > 0)
        {
            string sceneList = "";
            foreach (string s in scenes)
                sceneList += s + Environment.NewLine;

            Debug.Log("Scenes: " + Environment.NewLine + sceneList);

            if (scene == null)
                scene = Path.GetFileNameWithoutExtension(scenes[0]);
        }
        else
            Debug.Log("Asset bundle contains no scenes");

        /// Load scene
        if (scene == null)
            Debug.Log("No scene to load.");
        else
        {
            Debug.Log("Loading scene: " + scene);
            Application.LoadLevel(scene);
        }
	}
}
