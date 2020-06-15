using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DebugScreen : MonoBehaviour
{
    [SerializeField]
    private Canvas debugCanvas;
    private Text dungeonParameterValues;

    void Awake()
    {
        debugCanvas = Instantiate(debugCanvas);
        dungeonParameterValues = debugCanvas.GetComponentsInChildren<Text>().First(
            x => x.name == "ParameterValues");
        debugCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            debugCanvas.enabled = !debugCanvas.enabled;
            if (debugCanvas.enabled)
            {
                LoadParameterValues();
            }
        }
    }

    private void LoadParameterValues()
    {
        DungeonParameters parameters = GM.Instance.DungeonParameters;
        dungeonParameterValues.text = string.Format(
            "{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}",
            parameters.Seed,
            parameters.Rows,
            parameters.Cols,
            parameters.TotalKeys,
            parameters.MinRoomHeight,
            parameters.MaxRoomHeight,
            parameters.MinRoomWidth,
            parameters.MaxRoomWidth,
            SliderText.FormatPercent(parameters.TargetRoomToDungeonRatio),
            SliderText.FormatPercent(parameters.DoorsToWallRatio),
            SliderText.FormatPercent(parameters.CorridorTurnChance)
            );
    }
}
