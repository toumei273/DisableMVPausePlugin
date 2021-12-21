using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using Stage;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace DisableMVPausePlugin
{
    [BepInProcess("imascgstage.exe")]
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
        public class Main : BaseUnityPlugin
    {

        public const string PluginGuid = "toumei.cgss.DisableMVPause";
        public const string PluginName = "DisableMVPausePlugin";
        public const string PluginVersion = "1.1.0.0";
        public void Awake()
        {
            Logger.LogInfo($"Plugin {PluginName} is loaded!");
            new Harmony(PluginGuid).PatchAll();
        }


        [HarmonyPatch(typeof(LiveController), "Update_MusicVideo")]
        internal class ChangeMVPauseKey //クリックからキー入力に変更する
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Type myType = typeof(LiveController);
                Type[] typeArgs = { typeof(KeyCode)};
                MethodInfo myMethod = myType.GetMethod("Update_MusicVideo");
                IEnumerable<CodeInstruction> inst =  (IEnumerable<CodeInstruction>)new CodeMatcher(instructions)
                    .Start()
                    .MatchForward(false, 
                        new CodeMatch(OpCodes.Ldc_I4_0),
                        new CodeMatch(OpCodes.Call),
                        new CodeMatch(OpCodes.Brfalse))

                    .SetAndAdvance(OpCodes.Ldc_I4_S, 0x70)
                    .SetAndAdvance(OpCodes.Call, AccessTools.Method(typeof(UnityEngine.Input), "GetKeyInt", typeArgs))
                    .InstructionEnumeration();
                    
                return inst;
            }
        }

        [HarmonyPatch(typeof(LiveController), "Update_Input")]
        internal class ChangePausePopupKey //ポーズ表示を消したとき、クリックで再ポップアップしない
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Type myType = typeof(LiveController);
                Type[] typeArgs = { typeof(KeyCode)};
                MethodInfo myMethod = myType.GetMethod("Update_Input");
                IEnumerable<CodeInstruction> inst =  (IEnumerable<CodeInstruction>)new CodeMatcher(instructions)
                    .Start()
                    .MatchForward(false, 
                        new CodeMatch(OpCodes.Ldc_I4_0),
                        new CodeMatch(OpCodes.Call))

                    .SetAndAdvance(OpCodes.Ldc_I4_S, 0x70)
                    .SetAndAdvance(OpCodes.Call, AccessTools.Method(typeof(UnityEngine.Input), "GetKeyInt", typeArgs))

                    .InstructionEnumeration();
                    
                return inst;
            }
        }
    }
}
