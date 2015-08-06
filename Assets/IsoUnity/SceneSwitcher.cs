using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// SceneSwitcher behaviour changes scenes by using E and Q keys.
/// To use a subset of levels, check UseLevelNames and write the subset
/// in the LevelNames list.
/// 
/// Copyright to Víctor Pérez Colado
/// </summary>
public class SceneSwitcher : MonoBehaviour {

    /// <summary>
    /// If false, all the levels are used. Otherwise (true) 
    /// the subset contained in LevelNames is used.
    /// False by default.
    /// </summary>
    public bool useLevelNames = false;

    /// <summary>
    /// LevelNames list, used if UseLevelNames is true.
    /// </summary>
    public List<string> levelNames;

    // Auxiliar var to know what level is loaded in both levelNames.Count or Application.levelCount
    private int currentLevel;

    private static bool switcherExists = false;

	void Start () {
        // We only want one SceneSwitcher, otherwise, will load scenes in a strange way
        if (switcherExists)
            GameObject.DestroyImmediate(this.gameObject);
        else
            switcherExists = true;

        // Avoid SceneSwitcher to be destroyed between the changes
        GameObject.DontDestroyOnLoad(this.gameObject);

        // First we try to set currentlevel to the current loaded level (if exists in the list)
        currentLevel = (useLevelNames) ? levelNames.IndexOf(Application.loadedLevelName) : Application.loadedLevel;

        // In case level not found set to 0
        if (currentLevel == -1)
            currentLevel = 0;
	}
	
	// Update is called once per frame
	void Update () {
        // Perform if enabled and not loading levels
        if (this.isActiveAndEnabled && !Application.isLoadingLevel)
        {
            int sum = 0;

            // Key detection and operation
            if (Input.GetKeyDown(KeyCode.E))
                sum = 1;
            else if(Input.GetKeyDown(KeyCode.Q))
                sum = -1;

            // Level change
            if (sum != 0)
            {
                int max = useLevelNames ? levelNames.Count : Application.levelCount;

                currentLevel = (currentLevel + sum + max) % max;

                if (useLevelNames)
                    Application.LoadLevel(levelNames[currentLevel]);
                else
                    Application.LoadLevel(currentLevel);
            }
        }
	}
}
