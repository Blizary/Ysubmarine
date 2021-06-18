using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LightingManager2D))]
public class LightingManager2DEditor : Editor {
	static bool[] cameraFoldout = new bool[10];

	static bool[,] lightmapPreset = new bool[10, 10];
	static bool[,] lightmapMaterial = new bool[10, 10];

	override public void OnInspectorGUI() {
		LightingManager2D script = target as LightingManager2D;

		LightingSettings.Profile newProfile = (LightingSettings.Profile)EditorGUILayout.ObjectField("Profile", script.setProfile, typeof(LightingSettings.Profile), true);
		if (newProfile != script.setProfile) {
			script.setProfile = newProfile;

			script.UpdateProfile();

			// LightMainBuffer2D.Clear();
			// Light2D.ForceUpdateAll();
		}
		
		EditorGUILayout.Space();

		int count = script.cameraSettings.Length;
		count = EditorGUILayout.IntSlider("Camera Count", count, 0, 10);
		if (count != script.cameraSettings.Length) {
			System.Array.Resize(ref script.cameraSettings, count);
		}

		EditorGUILayout.Space();

		for(int id = 0; id < script.cameraSettings.Length; id++) {
			CameraSettings cameraSetting = script.cameraSettings[id];

			cameraFoldout[id] = EditorGUILayout.Foldout(cameraFoldout[id], "Camera " + (id + 1) + " (" + cameraSetting.GetTypeName() + ")");

			if (cameraFoldout[id] == false) {
				EditorGUILayout.Space();
				continue;
			}

			EditorGUI.indentLevel++;

			cameraSetting.cameraType = (CameraSettings.CameraType)EditorGUILayout.EnumPopup("Camera Type", cameraSetting.cameraType);

			if (cameraSetting.cameraType == CameraSettings.CameraType.Custom) {
				cameraSetting.customCamera = (Camera)EditorGUILayout.ObjectField(cameraSetting.customCamera, typeof(Camera), true);
			}

			CameraLightmap[] cameraLightmaps = cameraSetting.Lightmaps;

			EditorGUILayout.Space();

			int bcount = cameraLightmaps.Length;
			bcount = EditorGUILayout.IntSlider("Lightmap Count", bcount, 0, 10);
			if (bcount != cameraLightmaps.Length) {
				System.Array.Resize(ref cameraLightmaps, bcount);
			}

			EditorGUILayout.Space();

			cameraSetting.Lightmaps = cameraLightmaps;

			for(int b = 0; b < cameraSetting.Lightmaps.Length; b++) {
				CameraLightmap cameraBufferPreset = cameraSetting.Lightmaps[b];

				string presetName = Lighting2D.Profile.lightmapPresets.GetLightmapLayers()[cameraBufferPreset.bufferID];

				lightmapPreset[id, b] = EditorGUILayout.Foldout(lightmapPreset[id, b], "Lightmap " + (b + 1) + " (" + presetName + ")");

				if (lightmapPreset[id, b] == false) {
					EditorGUILayout.Space();
					continue;
				}

				EditorGUI.indentLevel++;

				cameraBufferPreset.bufferID = EditorGUILayout.Popup("Lightmap Preset", (int)cameraBufferPreset.bufferID, Lighting2D.Profile.lightmapPresets.GetLightmapLayers());

				cameraBufferPreset.renderMode = (CameraLightmap.RenderMode)EditorGUILayout.EnumPopup("Mode", cameraBufferPreset.renderMode);

				if (cameraBufferPreset.renderMode == CameraLightmap.RenderMode.Draw) {
					cameraBufferPreset.renderShader = (CameraLightmap.RenderShader)EditorGUILayout.EnumPopup("Shader", cameraBufferPreset.renderShader);
				
					if (cameraBufferPreset.renderShader == CameraLightmap.RenderShader.Custom) {
						cameraBufferPreset.customMaterial = (Material)EditorGUILayout.ObjectField(cameraBufferPreset.customMaterial, typeof(Material), true);
					}

					cameraBufferPreset.renderLayerType = (CameraLightmap.RenderLayerType)EditorGUILayout.EnumPopup("Layer Type", cameraBufferPreset.renderLayerType);

					if (cameraBufferPreset.renderLayerType == CameraLightmap.RenderLayerType.Custom) {
						cameraBufferPreset.renderLayerId = EditorGUILayout.LayerField("Layer", cameraBufferPreset.renderLayerId);
					}

				}

				cameraBufferPreset.output = (CameraLightmap.Output)EditorGUILayout.EnumPopup("Output", cameraBufferPreset.output);

				switch(cameraBufferPreset.output) {
					case CameraLightmap.Output.Materials:
								
						lightmapMaterial[id, b] = EditorGUILayout.Foldout(lightmapMaterial[id, b], "Materials");

						if (lightmapMaterial[id, b]) {
							EditorGUI.indentLevel++;

							CameraMaterials materials = cameraBufferPreset.GetMaterials();

							int matCount = materials.materials.Length;
							matCount = EditorGUILayout.IntField("Count", matCount);
							if (matCount !=  materials.materials.Length) {
								System.Array.Resize(ref materials.materials, matCount);
							}

							for(int i = 0; i < materials.materials.Length; i++) {
								materials.materials[i] = (Material)EditorGUILayout.ObjectField(materials.materials[i], typeof(Material), true);
							
							}

							EditorGUI.indentLevel--;
						}

					break;
				}

				cameraSetting.Lightmaps[b] = cameraBufferPreset;

				EditorGUI.indentLevel--;

				EditorGUILayout.Space();
			}

			cameraSetting.id = id;

			script.cameraSettings[id] = cameraSetting;

			EditorGUI.indentLevel--;

			EditorGUILayout.Space();
		}

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("version " + Lighting2D.VERSION_STRING);

		string buttonName = "";
		if (script.version < Lighting2D.VERSION) {
			buttonName += "Re-Initialize (Outdated)";
			GUI.backgroundColor = Color.red;

			Reinitialize(script);

			return;
		} else {
			buttonName += "Re-Initialize";
		}
		
		if (GUILayout.Button(buttonName)) {
			Reinitialize(script);
		}

		if (GUI.changed) {
			Light2D.ForceUpdateAll();

			LightingManager2D.ForceUpdate();

			if (EditorApplication.isPlaying == false) {
				
				EditorUtility.SetDirty(target);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				
			}
		}
	}

	public void Reinitialize(LightingManager2D manager) {
		Debug.Log("Lighting Manager 2D: reinitialized");

		if (manager.version > 0 && manager.version < Lighting2D.VERSION) {
			Debug.Log("Lighting Manager 2D: version update from " + manager.version_string + " to " + Lighting2D.VERSION_STRING);
		}

		foreach(Transform transform in manager.transform) {
			DestroyImmediate(transform.gameObject);
		}
			
		manager.Initialize();

		Light2D.ForceUpdateAll();

		LightingManager2D.ForceUpdate();

		if (EditorApplication.isPlaying == false) {
			
			EditorUtility.SetDirty(target);
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			
		}
	}
}