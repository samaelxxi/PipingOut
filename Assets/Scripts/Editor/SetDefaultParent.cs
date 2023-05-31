using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class SetDefaultParent : MonoBehaviour
{

    [MenuItem("Custom/Snap To Ground %g")]
    public static void SnapTransformGround()
    {
        if (Selection.transforms.Length == 1)
            EditorUtility.SetDefaultParentObject(Selection.transforms[0].gameObject);
    }
}
