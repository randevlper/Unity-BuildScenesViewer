using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildSceneViewer : EditorWindow {

	[MenuItem ("Window/BuildSceneViewer")]
	private static void ShowWindow () {
		var window = GetWindow<BuildSceneViewer> ();
		window.titleContent = new GUIContent ("BuildSceneViewer");
		window.Show ();
	}

	string[] levels;
	string[] levelPaths;

	private void OnGUI () {
		if (GUILayout.Button ("Generate")) {
			levelPaths = BuildSceneViewerUtil.ReadPaths ();
			levels = BuildSceneViewerUtil.ReadNames();
		}
		GUILayout.Label ("Scenes", EditorStyles.largeLabel);
		if (levels != null) {
			for (int i = 0; i < levels.Length; i++) {
				if (GUILayout.Button (levels[i])) {
					UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
					UnityEditor.SceneManagement.EditorSceneManager.OpenScene (levelPaths[i]);
				}
			}
		}
	}
}