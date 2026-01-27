using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using CombatExtended;
using RimWorld.Planet;

namespace LostCat
{
    [HarmonyPatch]
    public static class TraderStockPatches
    {
        [HarmonyTargetMethods]
        static IEnumerable<MethodBase> TargetMethods()
        {
            var baseType = typeof(StockGenerator);
            var methods = AccessTools.GetTypesFromAssembly(baseType.Assembly)
            .SelectMany(type => type.GetMethods())
            .Where(method => method.Name == nameof(StockGenerator.GenerateThings) && method.IsVirtual && !method.IsAbstract)
            .Cast<MethodBase>();
            foreach (var method in methods)
            {
                Log.Message($"TraderStockPatches: targeting {method.DeclaringType.FullName}.{method.Name} for patching.");
            }
            return methods;
        }

        static void Postfix(ref IEnumerable<Thing> __result, StockGenerator __instance, Faction faction, MethodBase __originalMethod)
        {   

            // Log.Message($"TraderStockPatches: Postfix called from {__originalMethod.DeclaringType.FullName}.{__originalMethod.Name}.");
            // // 
            if (__result == null)
            {
                // Log.Message("TraderStockPatches: __result is null, skipping.");
                return;
            }

            var list = __result as List<Thing> ?? __result.ToList();

            var weapons = new List<Thing>();
            for (int i = 0; i < list.Count; i++)
            {
                var t = list[i];
                if (t is ThingWithComps twc)
                {
                    if (CE_Utility.allWeaponDefs.Contains(t.def) && twc.HasComp<CompAmmoUser>())
                    {
                        weapons.Add(t);
                    }
                }
            }

            var ammosToAdd = new List<AmmoDef>();
            foreach (var weapon in weapons)
            {
                var compProps = weapon.def.GetCompProperties<CompProperties_AmmoUser>();
                var ammoSetDef = compProps?.ammoSet;
                var ammoTypes = ammoSetDef?.ammoTypes;
                var randomLink = ammoTypes?.RandomElement();
                // Log.Message($"TraderStockPatches: for weapon {weapon.def.defName}, found ammo set {randomLink?.ammo.defName}.");
                if (randomLink != null)
                {
                    var ammoDef = randomLink.ammo;
                    if (ammoDef != null && ammosToAdd.All(a => a.defName != ammoDef.defName))
                    {
                        ammosToAdd.Add(ammoDef);
                    }
                }
            }

            foreach (var ammoDef in ammosToAdd)
            {
                // Log.Message($"[LostCat.TraderWithAmmo]TraderStockPatches: determined to add ammo {ammoDef.defName} for trader stock.");
            }

            foreach (var ammoDef in ammosToAdd)
            {
                if (list.Any(t => t.def.defName == ammoDef.defName))
                {
                    continue;
                }
                var method = __originalMethod.DeclaringType.GetMethod("RandomCountOf", BindingFlags.Instance | BindingFlags.NonPublic);
                // var generic = method?.MakeGenericMethod(__instance.GetType());
                var countObj = method?.Invoke(__instance, new object[] { ammoDef });
                int count = 0;
                if (countObj is int ci) count = ci;
                else if (countObj is long cl) count = (int)cl;
                else if (countObj is short cs) count = cs;
                else if (countObj is byte cb) count = cb;
                else count = 100;

                if (count<10)
                {
                    count *= 100;
                    count = Math.Min(count, 1000);
                }

                var ammoThings = StockGeneratorUtility.TryMakeForStock(ammoDef, count, faction);
                if (ammoThings is Thing tAmmo)
                {
                    list.Add(tAmmo);
                }
                else if (ammoThings is IEnumerable<Thing> things)
                {
                    list.AddRange(things);
                }
                Log.Message($"[LostCat.TraderWithAmmo]TraderStockPatches: added {count} of {ammoDef.defName} to trader stock for weapon ammo.");
            }

            __result = list;
        }
    }

    // Allow all trader kinds to treat Combat Extended ammo as tradable so settlements actually sell it.
    [HarmonyPatch(typeof(TraderKindDef), nameof(TraderKindDef.WillTrade))]
    public static class TraderKindWillTradeAmmoPatch
    {
        static bool Prefix(ThingDef td, ref bool __result)
        {
            if (td is AmmoDef)
            {
                __result = true;
                return false; // Skip original to force allowance for ammo.
            }

            return true;
        }
    }

}

