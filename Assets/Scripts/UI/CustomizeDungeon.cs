using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeDungeon : MonoBehaviour
{
    [SerializeField]
    private Slider rows = null;
    [SerializeField]
    private Slider columns = null;
    [SerializeField]
    private Slider keys = null;

    [SerializeField]
    private Slider minRoomHeight = null;
    [SerializeField]
    private Slider minRoomWidth = null;
    [SerializeField]
    private Slider maxRoomHeight = null;
    [SerializeField]
    private Slider maxRoomWidth = null;

    [SerializeField]
    private Slider roomToDungeonRatio = null;
    [SerializeField]
    private Slider doorToWallRatio = null;
    [SerializeField]
    private Slider corridorTurnChance = null;

    [SerializeField]
    private InputField seed = null;

    [SerializeField]
    private LoadDungeonScene loadDungeonScene = null;

    [SerializeField]
    private DungeonParameters[] presets = null;

    void Awake()
    {
        LoadSlidersFromPreset(0);
    }

    void Update()
    {
        if (minRoomHeight.value > maxRoomHeight.value)
            maxRoomHeight.value = minRoomHeight.value;
        if (minRoomWidth.value > maxRoomWidth.value)
            maxRoomWidth.value = minRoomWidth.value;
    }

    public void CreateDungeonFromSliders()
    {
        LoadParametersFromSliders();
        CreateDungeon();
    }

    public void CreateDungeonFromPreset(int preset)
    {
        GM.Instance.DungeonParameters = presets[preset].Clone();
        GM.Instance.UseRandomSeed = true;
        CreateDungeon();
    }

    private void CreateDungeon()
    {
        loadDungeonScene.LoadNewDungeon();
    }

    private void LoadParametersFromSliders()
    {
        GM.Instance.DungeonParameters = new DungeonParameters
        {
            Rows = (int)rows.value,
            Cols = (int)columns.value,
            TotalKeys = KeysSliderToRealValue((int)keys.value, (int)keys.maxValue),
            MinRoomHeight = (int)minRoomHeight.value,
            MinRoomWidth = (int)minRoomWidth.value,
            MaxRoomHeight = (int)maxRoomHeight.value,
            MaxRoomWidth = (int)maxRoomWidth.value,
            TargetRoomToDungeonRatio = Math.Round(roomToDungeonRatio.value, 2),
            DoorsToWallRatio = Math.Round(doorToWallRatio.value, 2),
            CorridorTurnChance = Math.Round(corridorTurnChance.value, 2),
            Seed = HashSeed(seed.text),
        };

        GM.Instance.UseRandomSeed = seed.text == "";
    }

    public void LoadSlidersFromPreset(int index)
    {
        DungeonParameters preset = presets[index];
        rows.value = preset.Rows;
        columns.value = preset.Cols;
        keys.value = preset.TotalKeys;
        minRoomHeight.value = preset.MinRoomHeight;
        minRoomWidth.value = preset.MinRoomWidth;
        maxRoomHeight.value = preset.MaxRoomHeight;
        maxRoomWidth.value = preset.MaxRoomWidth;
        roomToDungeonRatio.value = (float) preset.TargetRoomToDungeonRatio;
        doorToWallRatio.value = (float) preset.DoorsToWallRatio;
        corridorTurnChance.value = (float) preset.CorridorTurnChance;
        // Leave the defined seed as-is
    }

    public static int KeysSliderToRealValue(int sliderValue, int sliderMaxValue)
    {
        if (sliderValue == sliderMaxValue)
            return Int32.MaxValue;
        else if (sliderValue > 60)
            return 100 * sliderValue - 5900;
        else if (sliderValue > 50)
            return 5 * sliderValue - 200;
        else
            return sliderValue;
    }

    // String hashing in the standard library isn't guaranteed to be persistent,
    // so we implement a simple hash function instead. I've arbitrarily chosen
    // int over long because seeds don't need to be huge.
    public static int HashSeed(string seed)
    {
        if (Int32.TryParse(seed, out int number))
            return number;

        // https://stackoverflow.com/questions/7666509/hash-function-for-string
        int hash = 5381;
        foreach (char c in seed)
        {
            hash = hash * 33 + Convert.ToInt32(c);
        }
        return hash;
    }
}
