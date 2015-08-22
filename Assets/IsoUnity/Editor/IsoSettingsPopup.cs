using UnityEngine;
using UnityEditor;
using System.Collections;


public class IsoSettingsPopup : EditorWindow
{
    public static bool showAgain = false;
    public static IsoSettingsPopup popupActive;
    private IsoSettings instance;

    public static bool IsShowAgain()
    {
        return showAgain;
    }

    public static void ShowIsoSettingsPopup(IsoSettings instance)
    {
        if (EditorUtility.DisplayDialog("IsoSettings not configured","IsoSettings seems to be not configured properly. ¿Go to IsoSettings? (Ignoring can cause unexpected bugs...)", "Show", "Ignore"))
        {
            Selection.activeObject = instance;
        }

        if (showAgain)
        {
            if (popupActive != null)
                return;

            popupActive = EditorWindow.GetWindow(typeof(IsoSettingsPopup)) as IsoSettingsPopup;
            popupActive.instance = instance;
        }
    }

    void OnGUI()
    {
        GUILayout.TextArea("IsoSettings seems to be not configured properly. ¿Go to IsoSettings? (Ignoring can cause unexpected bugs...)");
        showAgain = GUILayout.Toggle(showAgain, "Do not show popup again.");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Show"))
        {
            Selection.activeObject = instance;
            this.Close();
        }
        if (GUILayout.Button("Ignore"))
        {
            this.Close();
        }
        GUILayout.EndHorizontal();
    }

}
