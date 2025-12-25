using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using Verse.Noise;
using Verse.Grammar;
using RimWorld;
using RimWorld.Planet;


// *Uncomment for Harmony*
using System.Reflection;
using HarmonyLib;

namespace LostCat
{

    [StaticConstructorOnStartup]
    public static class Start
    {
        static Start()
        {
            Log.Message("Mod template loaded successfully!");

            Harmony harmony = new Harmony("LostCat.TradersSoldWeaponWithAmmo");
            harmony.PatchAll( Assembly.GetExecutingAssembly() );
        }
    }

}
