#region Using statements

using System.Reflection;

using UnityEngine;
using UnityEditor;

#endregion

//namespace NeverRestStudio.Editor
//{
//    [InitializeOnLoad]
//    public static class HierarchyWindowEnhancements
//    {
//        #region Fields - State

//        private static Color? backgroundColor;

//        #endregion

//        #region Properties

//        public static Color BackgroundColor
//        {
//            get
//            {
//                // check if we haven't cached the ui background color yet
//                if (!backgroundColor.HasValue)
//                {
//                    // make a reflection call to get the current ui background color in the editor
//                    var method = typeof(EditorGUIUtility).GetMethod("GetDefaultBackgroundColor", BindingFlags.NonPublic | BindingFlags.Static);
//                    backgroundColor = (Color)method.Invoke(null, null);
//                }

//                // return the cached value
//                return backgroundColor.Value;
//            }
//        }

//        #endregion

//        #region Constructor

//        static HierarchyWindowEnhancements()
//        {
//            // hook in to the rendering of hierarchy items
//            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
//        }

//        #endregion

//        #region Event handlers

//        static void HierarchyWindowItemOnGUI(int _instanceID, Rect _selectionRect)
//        {
//            // grab the associated game object
//            var gameObject = EditorUtility.InstanceIDToObject(_instanceID) as GameObject;
//            if (!gameObject)
//            {
//                return;
//            }

//            // check if the game object is a heading/section
//            if (gameObject.name.StartsWith("---", System.StringComparison.Ordinal))
//            {
//                // render the heading/section
//                EditorGUI.DrawRect(_selectionRect, new Color(0.07f, 0.07f, 0.07f));
//                var labelRect = new Rect(_selectionRect);
//                var offset = 5f;
//                labelRect.y -= offset;
//                labelRect.height += offset * 2f;
//                var oldContentColor = GUI.contentColor;
//                GUI.contentColor = new Color(1f, 0.7f, 0f);
//                EditorGUI.DropShadowLabel(labelRect, gameObject.name.Replace("-", "").ToUpperInvariant());
//                GUI.contentColor = oldContentColor;
//            }
//            else
//            {
//                // check for components on the game object
//                var allComponents = gameObject.GetComponents<Component>();
//                if (allComponents == null || allComponents.Length == 0)
//                {
//                    return;
//                }

//                // find the first with an icon
//                Component firstComponent = null;
//                Texture firstComponentImage = null;
//                for (var i = 0; i < allComponents.Length; i++)
//                {
//                    if (allComponents[i].GetType() == typeof(Transform))
//                    {
//                        continue;
//                    }
//                    firstComponent = allComponents[i];
//                    firstComponentImage = EditorGUIUtility.ObjectContent(firstComponent, firstComponent.GetType()).image;
//                    if (firstComponentImage)
//                    {
//                        break;
//                    }
//                }
//                if (!firstComponentImage)
//                {
//                    return;
//                }

//                // if an icon was found for a component on the game object, render the background color over the existing icon and then render the new one
//                var iconRect = new Rect(_selectionRect);
//                iconRect.width = iconRect.height;
//                EditorGUI.DrawRect(iconRect, BackgroundColor);
//                GUI.DrawTexture(iconRect, firstComponentImage);
//            }
//        }

//        #endregion
//    }
//}