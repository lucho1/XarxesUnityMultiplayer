using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
 
//Original TagSelector by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
//Altered TagSelector by Brecht Lecluyse http://www.brechtos.com
//LayerSelector by Sergi Parra https://github.com/t3m1X
 
[CustomPropertyDrawer(typeof(LayerSelectorAttribute))]
public class LayerSelectorPropertyDrawer : PropertyDrawer
{
 
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.BeginProperty(position, label, property);
 
            var attrib = this.attribute as LayerSelectorAttribute;
 
            if (attrib.UseDefaultLayerFieldDrawer)
            {
                property.intValue = EditorGUI.LayerField(position, label, property.intValue);
            }
            else
            {
                //generate the layerList
                List<string> layerList = new List<string>();
                layerList.AddRange(UnityEditorInternal.InternalEditorUtility.layers);
                string layerString = LayerMask.LayerToName(property.intValue);
                int index = 0;
                for (int i = 0; i < layerList.Count; i++) {
                    if (layerList[i] == layerString)
                        index = i;
                }
                
                //Draw the popup box with the current selected index
                index = EditorGUI.Popup(position, label.text, index, layerList.ToArray());
 
                //Adjust the actual value of the property based on the selection
                property.intValue = LayerMask.NameToLayer(layerList[index]);
            }
 
            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}