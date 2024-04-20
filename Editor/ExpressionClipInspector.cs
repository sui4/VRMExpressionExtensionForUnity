using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UniVRM10;

namespace VrmExpressionExtension
{
    [CustomEditor(typeof(ExpressionClip))]
    public class ExpressionClipInspector : Editor
    {
        private SerializedProperty _template;
        private SerializedProperty _preset;
        private SerializedProperty _weight;
        private SerializedProperty _customExpression;
        private ExpressionClip TargetClip => target as ExpressionClip;

        private int _selectedCustomClipIndex = -1;
        private string[] _customClipNames;
        private List<string> _warnings = new List<string>();

        private bool HasCustomClip => TargetClip.VrmObjectExpression.CustomClips.Count > 0;

        private void OnEnable()
        {
            _template = serializedObject.FindProperty("_template");
            _preset = _template.FindPropertyRelative("_preset");
            _weight = _template.FindPropertyRelative("_weight");
            _customExpression = _template.FindPropertyRelative("_customExpression");

            _warnings.Clear();
            if (TargetClip._template.Preset == ExpressionPreset.custom && TargetClip._template.CustomExpression != null)
            {
                _customClipNames = ExtractCustomClipNames();
                for (int i = 0; i < _customClipNames.Length; i++)
                {
                    if (_customClipNames[i] == TargetClip._template.CustomExpression.name)
                    {
                        _selectedCustomClipIndex = i;
                        break;
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (var change = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(_preset, new GUIContent("Emotion Type"));
                if (change.changed) serializedObject.ApplyModifiedProperties();
            }

            if (TargetClip._template.Preset == ExpressionPreset.custom)
            {
                EditorGUI.indentLevel++;
                CustomClipGUI();
                EditorGUI.indentLevel--;
            }

            using (var change = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(_weight);
                if (change.changed) serializedObject.ApplyModifiedProperties();
            }
        }

        private void CustomClipGUI()
        {
            var disabled = false;
            if (TargetClip.VrmObjectExpression == null)
            {
                EditorGUILayout.HelpBox(
                    "No VRM Object found. Please assign a VRM Object to this clip.",
                    MessageType.Error);
                disabled = true;
            }
            else if (!HasCustomClip)
            {
                EditorGUILayout.HelpBox(
                    "No custom clips found in the VRM Object Expression. ",
                    MessageType.Warning);
                disabled = true;
            }
            else if (_selectedCustomClipIndex == -1 && TargetClip._template.CustomExpression != null)
            {
                EditorGUILayout.HelpBox(
                    "Serialized custom clip not found in the current VRM object's expressions, or the binding information may be outdated. " +
                    "Please move the seek bar to refresh and update the binding info.",
                    MessageType.Warning);
            }

            _customClipNames = ExtractCustomClipNames();

            using (new EditorGUI.DisabledScope(disabled))
            {
                int selected = EditorGUILayout.Popup(
                    label: new GUIContent("Selected custom clip"),
                    selectedIndex: _selectedCustomClipIndex,
                    displayedOptions: _customClipNames
                );

                if (selected != _selectedCustomClipIndex)
                {
                    _selectedCustomClipIndex = selected;
                    string clipName = _customClipNames[_selectedCustomClipIndex];
                    // VrmObjectExpressionがnullになるケースでは、disabled = true になるので、nullチェックは不要 
                    VRM10Expression selectedClip =
                        TargetClip.VrmObjectExpression!.CustomClips.Find(clip => clip.name == clipName);
                    OnCustomClipChanged(selectedClip);
                }

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(_customExpression, new GUIContent("Clip reference"));
                }
            }
        }

        private string[] ExtractCustomClipNames()
        {
            if (TargetClip.VrmObjectExpression == null) return Array.Empty<string>();

            List<VRM10Expression> clips = TargetClip.VrmObjectExpression.CustomClips;
            var customClipNames = new string[clips.Count];
            for (int i = 0; i < clips.Count; i++)
            {
                customClipNames[i] = clips[i].name;
            }

            return customClipNames;
        }

        private void OnCustomClipChanged(VRM10Expression expression)
        {
            TargetClip._template.CustomExpression = expression;
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssetIfDirty(target);
            serializedObject.Update();
        }
    }
}