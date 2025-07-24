using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomCursor
{
    public static class CustomCursorPatches
    {
        public static void EmitCall<T>(this ILCursor iLCursor, string methodName, Type[] parameters = null, Type[] generics = null)
        {
            MethodInfo methodInfo = AccessTools.Method(typeof(T), methodName, parameters, generics);
            iLCursor.Emit(OpCodes.Call, methodInfo);
        }

        public static T GetFieldInEnumerator<T>(object instance, string fieldNamePattern)
        {
            return (T)instance.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(f => f.Name.Contains("$" + fieldNamePattern) || f.Name.Contains("<" + fieldNamePattern + ">") || f.Name == fieldNamePattern)
                .GetValue(instance);
        }

        public static bool TheNthTime(this Func<bool> predict, int n = 1)
        {
            for (int i = 0; i < n; ++i)
            {
                if (!predict())
                    return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(GameCursorController), nameof(GameCursorController.DrawCursor))]
        public class GameCursorControllerGetShowMouseCursorPatchClass
        {
            [HarmonyILManipulator]
            public static void GameCursorControllerGetShowMouseCursorPatch(ILContext ctx)
            {
                ILCursor crs = new ILCursor(ctx);

                ILLabel skipLabel = ctx.DefineLabel();

                if (crs.TryGotoNext(MoveType.Before,
                    x => x.MatchCall<GameCursorController>("get_showPlayerTwoControllerCursor")))
                {
                    crs.MarkLabel(skipLabel);
                }
                crs.Index = 0;

                if (crs.TryGotoNext(MoveType.Before,
                    x => x.MatchCallvirt<Texture>("get_width")))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_1));
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.Before,
                    x => x.MatchCallvirt<Texture>("get_height")))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_1));
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.Before,
                    x => x.MatchStloc(3)))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_2));
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.After,
                    x => x.MatchCallvirt<GameManager>("get_CurrentGameType")))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_3));
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.Before,
                    x => x.MatchCall<BraveInput>(nameof(BraveInput.GetInstanceForPlayer))))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_4));
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.After,
                    x => x.MatchCallvirt<BraveInput>(nameof(BraveInput.IsKeyboardAndMouse))))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_5));
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0)))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_1));
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.After,
                    x => x.MatchNewobj<Color>()))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_6));
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.Before,
                    x => x.MatchCallvirt<Texture>("get_width")))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_7));
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.Before,
                    x => x.MatchCallvirt<Texture>("get_height")))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_7));
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.Before,
                    x => x.MatchStloc(12)))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_8));
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.After,
                    x => x.MatchCall<Rect>(".ctor")))
                {
                    crs.Emit(OpCodes.Ldloc_S, (byte)13);
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_9));
                    crs.Emit(OpCodes.Br, skipLabel);
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.Before,
                    x => x.MatchCallvirt<Texture>("get_width")))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_10));
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.Before,
                    x => x.MatchCallvirt<Texture>("get_height")))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_10));
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.Before,
                    x => x.MatchStloc(20)))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_11));
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0)))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_10));
                }
                crs.Index++;

                if (crs.TryGotoNext(MoveType.After,
                    x => x.MatchNewobj<Color>()))
                {
                    crs.EmitCall<GameCursorControllerGetShowMouseCursorPatchClass>(nameof(GameCursorControllerGetShowMouseCursorPatchClass.GameCursorControllerGetShowMouseCursorPatchCall_12));
                }
            }

            private static Texture2D GameCursorControllerGetShowMouseCursorPatchCall_1(Texture2D orig)
            {
                if (CursorManager.instance.customCursorIsOn)
                {
                    if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && BraveInput.GetInstanceForPlayer(1).IsKeyboardAndMouse(false))
                        return CursorManager.instance.playerTwoCursor;
                    return CursorManager.instance.playerOneCursor;
                }
                return orig;
            }

            private static Vector2 GameCursorControllerGetShowMouseCursorPatchCall_2(Vector2 orig)
            {
                if (CursorManager.instance.customCursorIsOn)
                {
                    if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && BraveInput.GetInstanceForPlayer(1).IsKeyboardAndMouse(false))
                        return CursorManager.instance.playerTwoCursorScale * orig;
                    return CursorManager.instance.playerOneCursorScale * orig;
                }
                return orig;
            }

            private static int GameCursorControllerGetShowMouseCursorPatchCall_3(int orig)
            {
                if (CursorManager.instance.customCursorIsOn)
                    return 1;
                return orig;
            }

            private static int GameCursorControllerGetShowMouseCursorPatchCall_4(int orig)
            {
                if (CursorManager.instance.customCursorIsOn)
                    return 0;
                return orig;
            }

            private static bool GameCursorControllerGetShowMouseCursorPatchCall_5(bool orig)
            {
                if (CursorManager.instance.customCursorIsOn)
                    return true;
                return orig;
            }

            private static Color GameCursorControllerGetShowMouseCursorPatchCall_6(Color orig)
            {
                if (CursorManager.instance.customCursorIsOn)
                {
                    if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && BraveInput.GetInstanceForPlayer(1).IsKeyboardAndMouse(false))
                        return CursorManager.instance.playerTwoCursorModulation;
                    return CursorManager.instance.playerOneCursorModulation;
                }
                return orig;
            }

            private static Texture2D GameCursorControllerGetShowMouseCursorPatchCall_7(Texture2D orig)
            {
                if (CursorManager.instance.customCursorIsOn)
                    return CursorManager.instance.playerOneCursor;
                return orig;
            }

            private static Vector2 GameCursorControllerGetShowMouseCursorPatchCall_8(Vector2 orig)
            {
                if (CursorManager.instance.customCursorIsOn)
                    return CursorManager.instance.playerOneCursorScale * orig;
                return orig;
            }

            private static void GameCursorControllerGetShowMouseCursorPatchCall_9(Rect screenRect2)
            {
                if (CursorManager.instance.customCursorIsOn)
                    Graphics.DrawTexture(screenRect2, CursorManager.instance.playerOneCursor, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, CursorManager.instance.playerOneCursorModulation);
            }

            private static Texture2D GameCursorControllerGetShowMouseCursorPatchCall_10(Texture2D orig)
            {
                if (CursorManager.instance.customCursorIsOn)
                    return CursorManager.instance.playerTwoCursor;
                return orig;
            }

            private static Vector2 GameCursorControllerGetShowMouseCursorPatchCall_11(Vector2 orig)
            {
                if (CursorManager.instance.customCursorIsOn)
                    return CursorManager.instance.playerTwoCursorScale * orig;
                return orig;
            }

            private static Color GameCursorControllerGetShowMouseCursorPatchCall_12(Color orig)
            {
                if (CursorManager.instance.customCursorIsOn)
                    return CursorManager.instance.playerTwoCursorModulation;
                return orig;
            }
        }
    }
}
