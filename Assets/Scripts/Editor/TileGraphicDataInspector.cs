using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shrines {
    [CustomEditor(typeof(TileGraphicData))]
    public class TileGraphicDataInspector : UnityEditor.Editor {


        public TileGraphicData.Edge edge;
        int depth;
        int maxDepth;
        TileGraphicData.SpriteShape current;
        bool addingNew;
        TileGraphicData asset;

        void OnEnable()
        {
            asset = (TileGraphicData)target;
            depth = 0;
            maxDepth = 5;
            SetCurrent();
        }

        void SetCurrent()
        {
            if (addingNew && current.sprites.Length > 0 && current.sprites[0]!=null)
            {
                asset.shapes.Add(current);
                EditorUtility.SetDirty(asset);
            }

            addingNew = false;
            current = null;
            foreach (var s in asset.shapes)
            {
                if (s.depth == depth && s.edge == edge)
                {
                    current = s;
                }
            }
            if (current == null)
            {
                current = new TileGraphicData.SpriteShape()
                {
                    depth = depth,
                    edge = edge,
                    sprites = new Sprite[1]
                };
                addingNew = true;
            }
        }

        public override void OnInspectorGUI()
        {
            edge = (TileGraphicData.Edge)EditorGUILayout.EnumPopup("Edge", edge);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Depth");
            for (int i = 0; i < maxDepth; i++)
            {
                if (GUILayout.Button(string.Format(i == depth ? ">{0}<" : "{0}", i)))
                {
                    depth = i;
                }
            }
            EditorGUILayout.EndHorizontal();
            SetCurrent();
            if (current != null)
            {
            var rect = GUILayoutUtility.GetRect(75, 75);
            rect.width = 75;
                current.sprites[0] = (Sprite)EditorGUI.ObjectField(rect,current.sprites[0], typeof(Sprite));
            }

        }

        void MoveElement(SerializedProperty prop, int index, int step)
        {
            Debug.Log("Move element " + index + " of " + prop.displayName + " to " + (index + step));
            prop.MoveArrayElement(index, index + step);
            serializedObject.ApplyModifiedProperties();
        }

        void DeleteElement(SerializedProperty array, int index)
        {
            array.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
        }

    }
}