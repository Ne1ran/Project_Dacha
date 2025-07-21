using UnityEditor;
using UnityEngine;

namespace Game.Utils
{
#if UNITY_EDITOR

    public static class GizmosHelper
    {
        public static void DrawWireParallelepiped(Vector3 localCenter, Vector3 size, Vector3 position, Quaternion rotation, Color color)
        {
            float halfWidth = size.x * 0.5f;
            float halfHeight = size.y * 0.5f;
            float halfLength = size.z * 0.5f;

            Vector3 point1 =
                    position + rotation
                    * new Vector3(localCenter.x - halfWidth, localCenter.y - halfHeight, localCenter.z - halfLength); // left back bottom corner
            Vector3 point2 =
                    position + rotation
                    * new Vector3(localCenter.x - halfWidth, localCenter.y - halfHeight, localCenter.z + halfLength); // left forward bottom corner
            Vector3 point3 =
                    position + rotation
                    * new Vector3(localCenter.x + halfWidth, localCenter.y - halfHeight, localCenter.z - halfLength); // right back bottom corner
            Vector3 point4 =
                    position + rotation
                    * new Vector3(localCenter.x + halfWidth, localCenter.y - halfHeight, localCenter.z + halfLength); // right forward bottom corner
            Vector3 point5 =
                    position + rotation
                    * new Vector3(localCenter.x - halfWidth, localCenter.y + halfHeight, localCenter.z - halfLength); // left back top corner
            Vector3 point6 =
                    position + rotation
                    * new Vector3(localCenter.x - halfWidth, localCenter.y + halfHeight, localCenter.z + halfLength); // left forward top corner
            Vector3 point7 =
                    position + rotation
                    * new Vector3(localCenter.x + halfWidth, localCenter.y + halfHeight, localCenter.z - halfLength); // right back top corner
            Vector3 point8 =
                    position + rotation
                    * new Vector3(localCenter.x + halfWidth, localCenter.y + halfHeight, localCenter.z + halfLength); // right forward top corner

            Gizmos.color = color;

            // bottom face
            Gizmos.DrawLine(point1, point2);
            Gizmos.DrawLine(point1, point3);
            Gizmos.DrawLine(point3, point4);
            Gizmos.DrawLine(point4, point2);

            // top face
            Gizmos.DrawLine(point5, point6);
            Gizmos.DrawLine(point5, point7);
            Gizmos.DrawLine(point7, point8);
            Gizmos.DrawLine(point8, point6);

            // side faces
            Gizmos.DrawLine(point1, point5);
            Gizmos.DrawLine(point2, point6);
            Gizmos.DrawLine(point3, point7);
            Gizmos.DrawLine(point4, point8);
        }

        public static void DrawString(string text, Vector3 worldPos, Color? textColor = null, Color? backColor = null)
        {
            Handles.BeginGUI();
            Color restoreTextColor = GUI.color;
            Color restoreBackColor = GUI.backgroundColor;

            GUI.color = textColor ?? Color.white;
            GUI.backgroundColor = backColor ?? Color.black;

            SceneView view = SceneView.currentDrawingSceneView;
            if (view != null && view.camera != null) {
                Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
                if (screenPos.y < 0.0f || screenPos.y > Screen.height || screenPos.x < 0.0f || screenPos.x > Screen.width || screenPos.z < 0.0f) {
                    GUI.color = restoreTextColor;
                    Handles.EndGUI();
                    return;
                }
                Vector2 size = GUI.skin.label.CalcSize(new(text));
                Rect rect = new(screenPos.x - size.x / 0.5f, -screenPos.y + view.position.height + 4, size.x, size.y);
                GUI.Box(rect, text, EditorStyles.numberField);
                GUI.Label(rect, text);
                GUI.color = restoreTextColor;
                GUI.backgroundColor = restoreBackColor;
            }
            Handles.EndGUI();
        }
    }

#endif
}