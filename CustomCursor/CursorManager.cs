using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using BepInEx;
using Gunfiguration;
using System.Linq;

namespace CustomCursor
{
    public class CursorManager
    {
        private class CursorDataFile
        {
            [System.Serializable]
            public class CursorEntry
            {
                public string name = "";
                public string image = "";
            }

            [System.Serializable]
            public class RGBAModulationEntry
            {
                public string name = "";
                public float[] rgba_modulation = new float[0];
            }

            public List<CursorEntry> cursors = new List<CursorEntry>();
            public List<RGBAModulationEntry> rgba_modulations = new List<RGBAModulationEntry>();
            public List<float> scales = new List<float>();
        }

        private class CursorItem
        {
            public string name;
            public Texture2D texture;
        }

        private class RGBAModulationItem
        {
            public string name;
            public Color modulation;
        }

        internal static CursorManager instance;

        private Gunfig gunfig;

        private const string CustomCursorOnStr = "Custom Cursor On";
        private const string PlayerOneCursorStr = "1P Cursor";
        private const string PlayerOneCursorModulationStr = "1P Cursor Modulation";
        private const string PlayerOneCursorScaleStr = "1P Cursor Scale";
        private const string PlayerTwoCursorStr = "2P Cursor";
        private const string PlayerTwoCursorModulationStr = "2P Cursor Modulation";
        private const string PlayerTwoCursorScaleStr = "2P Cursor Scale";

        public bool customCursorIsOn;
        public Texture2D playerOneCursor;
        public Color playerOneCursorModulation;
        public float playerOneCursorScale;
        public Texture2D playerTwoCursor;
        public Color playerTwoCursorModulation;
        public float playerTwoCursorScale;

        private List<string> cursorNameList = new List<string>();
        private Dictionary<string, CursorItem> registeredCursors = new Dictionary<string, CursorItem>();

        private List<string> modulationNameList = new List<string>();
        private Dictionary<string, RGBAModulationItem> registeredModulations = new Dictionary<string, RGBAModulationItem>();

        private Dictionary<string, float> registeredScales = new Dictionary<string, float>();

        private readonly Color defaultModulation = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        private Texture2D[] defaultCursors;

        private dfGUIManager uiManager;

        public CursorManager()
        {
            instance = this;
        }

        private void SetCustomCursorIsOn(bool value) => customCursorIsOn = value;
        private void SetPlayerOneCursor(Texture2D value) => playerOneCursor = value;
        private void SetPlayerOneCursorModulation(Color value) => playerOneCursorModulation = value;
        private void SetPlayerOneCursorScale(float value) => playerOneCursorScale = value;
        private void SetPlayerTwoCursor(Texture2D value) => playerTwoCursor = value;
        private void SetPlayerTwoCursorModulation(Color value) => playerTwoCursorModulation = value;
        private void SetPlayerTwoCursorScale(float value) => playerTwoCursorScale = value;

        private string GetUniqueName<T>(Dictionary<string, T> dict, string baseName)
        {
            if (!dict.ContainsKey(baseName))
                return baseName;

            int maxNumber = 0;
            string prefix = baseName + "_";
            bool hasAnyNumberedKey = false;

            foreach (var key in dict.Keys)
            {
                if (key == baseName)
                    continue;

                if (key.StartsWith(prefix))
                {
                    string suffix = key.Substring(prefix.Length);
                    if (int.TryParse(suffix, out int number))
                    {
                        hasAnyNumberedKey = true;
                        if (number > maxNumber)
                            maxNumber = number;
                    }
                }
            }

            return hasAnyNumberedKey ? $"{baseName}_{maxNumber + 1}" : $"{baseName}_1";
        }

        internal void LoadAllCursorData()
        {
            cursorNameList.Clear();
            registeredCursors.Clear();

            modulationNameList.Clear();
            registeredModulations.Clear();

            registeredScales.Clear();

            defaultCursors = GameUIRoot.Instance.GetComponent<GameCursorController>()?.cursors;

            if (defaultCursors != null)
            {
                for (int i = 0; i < defaultCursors.Length; ++i)
                {
                    string uniqueName = "default" + i.ToString();
                    registeredCursors[uniqueName] = new CursorItem { name = uniqueName, texture = defaultCursors[i] };
                    cursorNameList.Add(uniqueName);
                }
            }

            string defaultModulationName = "default";
            registeredModulations[defaultModulationName] = new RGBAModulationItem { name = defaultModulationName, modulation = defaultModulation };
            modulationNameList.Add(defaultModulationName);

            string defaultScaleName = "1";
            registeredScales[defaultScaleName] = 1f;

            var files = Directory.GetFiles(Paths.PluginPath, "*.customcursor", SearchOption.AllDirectories);

            foreach (string path in files)
            {
                string json = File.ReadAllText(path);
                CursorDataFile data = JsonConvert.DeserializeObject<CursorDataFile>(json);

                if (data == null)
                {
                    Debug.LogWarning($"Failed to deserialize JSON in file: {path}");
                    continue;
                }

                if (data.cursors != null)
                {
                    foreach (var cursor in data.cursors)
                    {
                        if (string.IsNullOrEmpty(cursor.name)) continue;

                        string fullPath = Path.Combine(Path.GetDirectoryName(path), cursor.image);
                        if (!File.Exists(fullPath))
                        {
                            Debug.LogWarning($"Cursor image not found: {fullPath}");
                            continue;
                        }

                        byte[] bytes = File.ReadAllBytes(fullPath);
                        Texture2D texture = new Texture2D(0, 0, TextureFormat.RGBA32, mipmap: false);
                        texture.filterMode = FilterMode.Point;
                        texture.LoadImage(bytes);
                        string uniqueName = GetUniqueName(registeredCursors, cursor.name);
                        registeredCursors[uniqueName] = new CursorItem { name = uniqueName, texture = texture };
                        cursorNameList.Add(uniqueName);
                    }
                }

                if (data.rgba_modulations != null)
                {
                    foreach (var modulation in data.rgba_modulations)
                    {
                        if (modulation.rgba_modulation == null || modulation.rgba_modulation.Length < 3) continue;

                        string baseName = modulation.name ?? "UnnamedModulation";
                        string uniqueName = GetUniqueName(registeredModulations, baseName);
                        Color col = new Color(modulation.rgba_modulation[0] / 2f, modulation.rgba_modulation[1] / 2f, modulation.rgba_modulation[2] / 2f, (modulation.rgba_modulation.Length >= 4) ? (modulation.rgba_modulation[3] / 2f) : 0.5f);
                        registeredModulations[uniqueName] = new RGBAModulationItem { name = uniqueName, modulation = col };
                        modulationNameList.Add(uniqueName);
                    }
                }

                if (data.scales != null)
                {
                    foreach (var scale in data.scales)
                    {
                        string scaleName = scale.ToString();
                        if (!registeredScales.ContainsKey(scaleName))
                        {
                            registeredScales[scaleName] = scale;
                        }
                    }
                }
            }

            Debug.Log($"Loaded cursors: {cursorNameList.Count}, modulations: {modulationNameList.Count}, sizes: {registeredScales.Keys.Count}");
        }

        private void UpdateCustomCursor(string value = null)
        {
            if (value == null)
                value = gunfig.Value(CustomCursorOnStr);

            SetCustomCursorIsOn(value == "1");
            if (customCursorIsOn)
            {
                UpdatePlayerOneCursor();
                UpdatePlayerOneCursorModulation();
                UpdatePlayerOneCursorScale();
                UpdatePlayerTwoCursor();
                UpdatePlayerTwoCursorModulation();
                UpdatePlayerTwoCursorScale();
            }
        }

        private void UpdatePlayerOneCursor(string value = null)
        {
            if (value == null)
                value = gunfig.Value(PlayerOneCursorStr);

            if (registeredCursors.TryGetValue(value, out CursorItem cursorItem))
                SetPlayerOneCursor(cursorItem.texture);
            else
                SetPlayerOneCursor(defaultCursors?[0] ?? GameUIRoot.Instance.GetComponent<GameCursorController>()?.normalCursor);
        }

        private void UpdatePlayerOneCursorModulation(string value = null)
        {
            if (value == null)
                value = gunfig.Value(PlayerOneCursorModulationStr);

            if (registeredModulations.TryGetValue(value, out RGBAModulationItem modulationItem))
                SetPlayerOneCursorModulation(modulationItem.modulation);
            else
                SetPlayerOneCursorModulation(defaultModulation);
        }

        private void UpdatePlayerOneCursorScale(string value = null)
        {
            if (value == null)
                value = gunfig.Value(PlayerOneCursorScaleStr);

            if (registeredScales.TryGetValue(value, out float scale))
                SetPlayerOneCursorScale(scale);
            else
                SetPlayerOneCursorScale(1f);
        }

        private void UpdatePlayerTwoCursor(string value = null)
        {
            if (value == null)
                value = gunfig.Value(PlayerTwoCursorStr);

            if (registeredCursors.TryGetValue(value, out CursorItem cursorItem))
                SetPlayerTwoCursor(cursorItem.texture);
            else
                SetPlayerTwoCursor(defaultCursors?[0] ?? GameUIRoot.Instance.GetComponent<GameCursorController>()?.normalCursor);
        }

        private void UpdatePlayerTwoCursorModulation(string value = null)
        {
            if (value == null)
                value = gunfig.Value(PlayerTwoCursorModulationStr);

            if (registeredModulations.TryGetValue(value, out RGBAModulationItem modulationItem))
                SetPlayerTwoCursorModulation(modulationItem.modulation);
            else
                SetPlayerTwoCursorModulation(defaultModulation);
        }

        private void UpdatePlayerTwoCursorScale(string value = null)
        {
            if (value == null)
                value = gunfig.Value(PlayerTwoCursorScaleStr);

            if (registeredScales.TryGetValue(value, out float scale))
                SetPlayerTwoCursorScale(scale);
            else
                SetPlayerTwoCursorScale(1f);
        }

        internal void InitializeGunfig()
        {
            gunfig = Gunfig.Get("Custom Cursor".WithColor(Color.white));

            List<string> sortedScales = registeredScales.Keys
                .OrderBy(key =>
                {
                    if (float.TryParse(key, out float result))
                        return result;
                    return float.MaxValue;
                })
                .ToList();

            gunfig.AddToggle(key: CustomCursorOnStr, label: GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE ?
                "开启Custom Cursor" :
                CustomCursorOnStr, enabled: true, updateType: Gunfig.Update.Immediate,
                callback: (optionKey, optionValue) => UpdateCustomCursor(optionValue));
            gunfig.AddScrollBox(key: PlayerOneCursorStr, label: GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE ?
                "1P光标" :
                PlayerOneCursorStr, options: cursorNameList, updateType: Gunfig.Update.Immediate,
                callback: (optionKey, optionValue) => UpdatePlayerOneCursor(optionValue));
            gunfig.AddScrollBox(key: PlayerOneCursorModulationStr, label: GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE ?
                "1P光标颜色" :
                PlayerOneCursorModulationStr, options: modulationNameList, updateType: Gunfig.Update.Immediate,
                callback: (optionKey, optionValue) => UpdatePlayerOneCursorModulation(optionValue));
            gunfig.AddScrollBox(key: PlayerOneCursorScaleStr, label: GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE ?
                "1P光标缩放" :
                PlayerOneCursorScaleStr, options: sortedScales, updateType: Gunfig.Update.Immediate,
                callback: (optionKey, optionValue) => UpdatePlayerOneCursorScale(optionValue));
            gunfig.AddScrollBox(key: PlayerTwoCursorStr, label: GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE ?
                "2P光标" :
                PlayerTwoCursorStr, options: cursorNameList, updateType: Gunfig.Update.Immediate,
                callback: (optionKey, optionValue) => UpdatePlayerTwoCursor(optionValue));
            gunfig.AddScrollBox(key: PlayerTwoCursorModulationStr, label: GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE ?
                "2P光标颜色" :
                PlayerTwoCursorModulationStr, options: modulationNameList, updateType: Gunfig.Update.Immediate,
                callback: (optionKey, optionValue) => UpdatePlayerTwoCursorModulation(optionValue));
            gunfig.AddScrollBox(key: PlayerTwoCursorScaleStr, label: GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE ?
                "2P光标缩放" :
                PlayerTwoCursorScaleStr, options: sortedScales, updateType: Gunfig.Update.Immediate,
                callback: (optionKey, optionValue) => UpdatePlayerTwoCursorScale(optionValue));

            UpdateCustomCursor();
        }

        internal void Initialize()
        {
            uiManager = GameObject.Find("UI Root")?.GetComponent<dfGUIManager>();
            if (uiManager == null)
                Debug.LogError("DfGUIManager not found.");

            LoadAllCursorData();
            InitializeGunfig();
        }
    }
}
