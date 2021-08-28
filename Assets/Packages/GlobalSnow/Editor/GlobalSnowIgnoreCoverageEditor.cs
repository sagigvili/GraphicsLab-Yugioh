using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GlobalSnowEffect {
    [CustomEditor(typeof(GlobalSnowIgnoreCoverage))]
    public class GlobalSnowIgnoreCoverageEditor : Editor {

        SerializedProperty receiveSnow, blockSnow;

        private void OnEnable() {
            receiveSnow = serializedObject.FindProperty("_receiveSnow");
            blockSnow = serializedObject.FindProperty("_blockSnow");
        }


        public override void OnInspectorGUI() {

            serializedObject.Update();

            EditorGUILayout.PropertyField(receiveSnow);
            EditorGUILayout.PropertyField(blockSnow);

            if (serializedObject.ApplyModifiedProperties()) {
                GlobalSnow snow = GlobalSnow.instance;
                if (snow != null) {
                    snow.RefreshExcludedObjects();
                }
            }

        }
    }

}