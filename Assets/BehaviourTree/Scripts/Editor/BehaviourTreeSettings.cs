using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree.Scripts.Editor
{
    internal class BehaviourTreeSettings : ScriptableObject
    {
        public VisualTreeAsset BehaviourTreeXml;
        public StyleSheet BehaviourTreeStyle;
        
        public VisualTreeAsset NodeXml;

        public StyleSheet BlackboardStyle;
        
        public TextAsset ScriptTemplateActionNode;
        public TextAsset ScriptTemplateCompositeNode;
        public TextAsset ScriptTemplateDecoratorNode;
        
        public string NewNodeBasePath = "Assets/";

        private static BehaviourTreeSettings FindSettings()
        {
            var guids = AssetDatabase.FindAssets("t:BehaviourTreeSettings");
            if (guids.Length > 1)
            {
                Debug.LogWarning($"Found multiple settings files, using the first.");
            }

            switch (guids.Length)
            {
                case 0:
                    return null;
                default:
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    return AssetDatabase.LoadAssetAtPath<BehaviourTreeSettings>(path);
            }
        }

        internal static BehaviourTreeSettings GetOrCreateSettings()
        {
            var settings = FindSettings();
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<BehaviourTreeSettings>();
                AssetDatabase.CreateAsset(settings, "Assets");
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    internal static class MyCustomSettingsUIElementsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            var provider = new SettingsProvider("Project/MyCustomUIElementsSettings", SettingsScope.Project)
            {
                label = "BehaviourTree",
                activateHandler = (searchContext, rootElement) =>
                {
                    var settings = BehaviourTreeSettings.GetSerializedSettings();
                    
                    var title = new Label()
                    {
                        text = "Behaviour Tree Settings"
                    };
                    title.AddToClassList("title");
                    rootElement.Add(title);

                    var properties = new VisualElement()
                    {
                        style =
                        {
                            flexDirection = FlexDirection.Column
                        }
                    };
                    properties.AddToClassList("property-list");
                    rootElement.Add(properties);

                    properties.Add(new InspectorElement(settings));

                    rootElement.Bind(settings);
                },
            };

            return provider;
        }
    }
}