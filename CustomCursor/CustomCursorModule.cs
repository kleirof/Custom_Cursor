using BepInEx;
using HarmonyLib;

namespace CustomCursor
{
    [BepInDependency("etgmodding.etg.mtgapi")]
    [BepInDependency("pretzel.etg.gunfig")]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class CustomCursorModule : BaseUnityPlugin
    {
        public const string GUID = "kleirof.etg.customcursor";
        public const string NAME = "Custom Cursor";
        public const string VERSION = "1.0.0";
        public const string TEXT_COLOR = "#857DF1";

        public static CustomCursorModule instance;

        public CursorManager cursorManager;

        public void Start()
        {
            instance = this;

            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);

            cursorManager = new CursorManager();

            Harmony harmony = new Harmony(GUID);
            harmony.PatchAll();
        }

        internal static void Log(string text, string color = "FFFFFF")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }

        internal void GMStart(GameManager g)
        {
            Log($"{NAME} v{VERSION} started successfully.", TEXT_COLOR);

            cursorManager.LoadAllCursorData();
            cursorManager.InitializeGunfig();
        }
    }
}
