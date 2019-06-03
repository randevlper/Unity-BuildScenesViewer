using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildSceneViewer : EditorWindow {

	[MenuItem ("Window/BuildSceneViewer")]
	private static void ShowWindow () {
		var window = GetWindow<BuildSceneViewer> ();
		window.titleContent = new GUIContent ("BuildSceneViewer");
		window.Show ();
	}

	static string[] levels;
	static string[] levelPaths;

	private void OnGUI () {
		if (levels == null) {
			GetLevels ();
		}
		if (GUILayout.Button ("Generate List of Scenes")) {
			GetLevels ();
		}
		if (GUILayout.Button ("Generate Level Names Object")) {
			GenerateLevelNamesForRuntime ();
		}
		GUILayout.Label ("Scenes", EditorStyles.largeLabel);
		if (levels != null) {
			for (int i = 0; i < levels.Length; i++) {
				if (GUILayout.Button (levels[i])) {
					if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ()) {
						UnityEditor.SceneManagement.EditorSceneManager.OpenScene (levelPaths[i]);
					}

				}
			}
		}
	}

	static void GetLevels () {
		levelPaths = BuildSceneViewerUtil.ReadPaths ();
		levels = BuildSceneViewerUtil.ReadNames ();
	}

	public static void GenerateLevelNamesForRuntime () {
		GetLevels ();
		//ScriptableObjectUtility.CreateAsset<StringArrayContainer>("Assets/Resources");
		StringArrayContainer asset = Resources.Load<StringArrayContainer> ("Level/levelNames");
		if (asset == null) {
			Debug.LogError ("Create a StringArrayContainer levelNames object at Assets/Resources/Level/");
			asset = ScriptableObject.CreateInstance<StringArrayContainer> ();

			string absolutePath = Application.dataPath + "/Resources/Level/";
			if (!Directory.Exists (absolutePath)) {
				Directory.CreateDirectory (absolutePath);
			}
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath ("Assets/Resources/Level/levelNames.asset");
			AssetDatabase.CreateAsset (asset, assetPathAndName);
		}
		EditorUtility.SetDirty (asset);
		asset.values = levels;
		AssetDatabase.Refresh ();
		AssetDatabase.SaveAssets ();
	}
}

class BuildSceneViewerBuildProcessor : IPreprocessBuildWithReport {
	public int callbackOrder { get { return 0; } }
	public void OnPreprocessBuild (BuildReport report) {
		Debug.Log ("BuildSceneViewerBuildProcessor.OnPreprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);
		BuildSceneViewer.GenerateLevelNamesForRuntime ();
	}
}