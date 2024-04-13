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

        private bool _hasCustomClip => TargetClip.VrmObjectExpression.CustomClips.Count > 0;

        private void OnEnable()
        {
            _template = serializedObject.FindProperty("_template");
            _preset = _template.FindPropertyRelative("_preset");
            _weight = _template.FindPropertyRelative("_weight");
            _customExpression = _template.FindPropertyRelative("_customExpression");

            if (TargetClip._template.Preset == ExpressionPreset.custom)
            {
                ExtractCustomClipNames();
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
                if (TargetClip.VrmObjectExpression == null)
                {
                    EditorGUILayout.HelpBox(
                        "No VRM Object Expression found. Please assign a VRM Object Expression to this clip.",
                        MessageType.Error);
                }
                else if (!_hasCustomClip)
                {
                    EditorGUILayout.HelpBox(
                        "No custom clips found in the VRM Object Expression. ",
                        MessageType.Warning);
                }
                else
                {
                    EditorGUI.indentLevel++;

                    ExtractCustomClipNames();
                    int selected = EditorGUILayout.Popup(
                        label: new GUIContent("Custom Expression"),
                        selectedIndex: _selectedCustomClipIndex,
                        displayedOptions: _customClipNames
                    );

                    if (selected != _selectedCustomClipIndex)
                    {
                        _selectedCustomClipIndex = selected;
                        OnCustomClipChanged(TargetClip.VrmObjectExpression.CustomClips[_selectedCustomClipIndex]);
                    }

                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.PropertyField(_customExpression, new GUIContent("Reference"));
                    }

                    EditorGUI.indentLevel--;
                }
            }

            using (var change = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(_weight);
                if (change.changed) serializedObject.ApplyModifiedProperties();
            }
        }

        private void ExtractCustomClipNames()
        {
            List<VRM10Expression> clips = TargetClip.VrmObjectExpression.CustomClips;
            _customClipNames = new string[clips.Count];
            for (int i = 0; i < clips.Count; i++)
            {
                _customClipNames[i] = clips[i].name;
            }
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