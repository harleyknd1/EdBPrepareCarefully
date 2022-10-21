using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EdB.PrepareCarefully.Source;
using RimWorld;
using UnityEngine;
using Verse;

namespace EdB.PrepareCarefully {
    public static class BackstoryDatabase {
        public static Dictionary<string, BackstoryDef> allBackstories = new Dictionary<string, BackstoryDef>();
        private static Dictionary<BackstoryDatabase.CacheKey, List<BackstoryDef>> shuffleableBackstoryList = new Dictionary<BackstoryDatabase.CacheKey, List<BackstoryDef>>();
        private static Regex regex = new Regex("^[^0-9]*");

        public static void Clear() => BackstoryDatabase.allBackstories.Clear();

        public static void ReloadAllBackstories() {
            ModContentPack mod;
            try {
                mod = LoadedModManager.GetMod<PrepareCarefullyMod>().Content;
                Log.Warning($"Folder Location: {mod.FolderName}");
                foreach(var test in mod.foldersToLoadDescendingOrder) {
                    Log.Warning(test);
                }
                DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(mod.foldersToLoadDescendingOrder[0], "Backstories"));
                Log.Warning($"PickedFolder:{directoryInfo.FullName}");
                var xmlAssets = DirectXmlLoader.XmlAssetsInModFolder(mod, $"Backstories");
                Log.Warning($"Loading: {xmlAssets.Length} XML Assets");
                foreach (var asset in xmlAssets) {
                    Log.Warning(asset.ToString());
                    var def = DirectXmlLoader.AllGameItemsFromAsset<BackstoryDef>(asset);
                }
                IEnumerable<BackstoryDef> bsDef = xmlAssets.SelectMany(x => DirectXmlLoader.AllGameItemsFromAsset<BackstoryDef>(x));
                Log.Warning($"Loaded: {bsDef.Count()} backstories.");
                foreach (BackstoryDef bs in bsDef) {
                    DeepProfiler.Start("Backstory.PostLoad");
                    try {
                        bs.PostLoad();
                    }
                    finally {
                        DeepProfiler.End();
                    }
                    DeepProfiler.Start("Backstory.ResolveReferences");
                    try {
                        bs.ResolveReferences();

                    }
                    finally {
                        DeepProfiler.End();
                    }
                    try {
                        foreach (string configError in bs.ConfigErrors())
                            Log.Error(bs.title + ": " + configError);
                    }
                    catch { }
                    DeepProfiler.Start("AddBackstory");
                    try {
                        if(BackstoryDatabase.allBackstories == null) {
                            BackstoryDatabase.allBackstories = new Dictionary<string, BackstoryDef>();
                        }
                        var identifier = bs.title.Replace(" ","-");
                        bs.identifier = $"nry.edb.{identifier}.{Rand.Range(1,10)}{Rand.Range(1, 10)}{Rand.Range(1, 10)}{Rand.Range(1, 10)}{Rand.Range(1, 10)}";
                        BackstoryDatabase.AddBackstory(bs);
                    }
                    finally {
                        DeepProfiler.End();
                    }
                }
                Log.Warning("loading bios");
                SolidBioDatabase.LoadAllBios();
            }
            catch(Exception ex) {
                Log.Error("Could not find the mod by its packageId, somehow....");
                Log.Error(ex.ToString());
            }
            

        }

        public static void AddBackstory(BackstoryDef bs) {
            if (BackstoryDatabase.allBackstories.ContainsKey(bs.identifier)) {
                if (bs == BackstoryDatabase.allBackstories[bs.identifier])
                    Log.Error("Tried to add the same backstory twice " + bs.identifier);
                else
                    Log.Error("Backstory " + bs.title + " has same unique save key " + bs.identifier + " as old backstory " + BackstoryDatabase.allBackstories[bs.identifier].title);
            }
            else {
                BackstoryDatabase.allBackstories.Add(bs.identifier, bs);
                BackstoryDatabase.shuffleableBackstoryList.Clear();
            }
        }

        public static bool TryGetWithIdentifier(
          string identifier,
          out BackstoryDef bs,
          bool closestMatchWarning = true) {
            identifier = BackstoryDatabase.GetIdentifierClosestMatch(identifier, closestMatchWarning);
            return BackstoryDatabase.allBackstories.TryGetValue(identifier, out bs);
        }

        public static string GetIdentifierClosestMatch(string identifier, bool closestMatchWarning = true) {
            if (BackstoryDatabase.allBackstories.ContainsKey(identifier))
                return identifier;
            string str = BackstoryDatabase.StripNumericSuffix(identifier);
            foreach (KeyValuePair<string, BackstoryDef> allBackstory in BackstoryDatabase.allBackstories) {
                BackstoryDef backstory = allBackstory.Value;
                if (BackstoryDatabase.StripNumericSuffix(backstory.identifier) == str) {
                    if (closestMatchWarning)
                        Log.Warning("Couldn't find exact match for backstory " + identifier + ", using closest match " + backstory.identifier);
                    return backstory.identifier;
                }
            }
            Log.Warning("Couldn't find exact match for backstory " + identifier + ", or any close match.");
            return identifier;
        }

        public static BackstoryDef RandomBackstory(BackstorySlot slot) => BackstoryDatabase.allBackstories.Where<KeyValuePair<string, BackstoryDef>>((Func<KeyValuePair<string, BackstoryDef>, bool>)(bs => bs.Value.slot == slot)).RandomElement<KeyValuePair<string, BackstoryDef>>().Value;

        public static List<BackstoryDef> ShuffleableBackstoryList(
          BackstorySlot slot,
          BackstoryCategoryFilter group,
          BackstorySlot? mustBeCompatibleTo = null) {
            BackstoryDatabase.CacheKey key = new BackstoryDatabase.CacheKey(slot, group, mustBeCompatibleTo);
            if (!BackstoryDatabase.shuffleableBackstoryList.ContainsKey(key)) {
                if (!mustBeCompatibleTo.HasValue) {
                    BackstoryDatabase.shuffleableBackstoryList[key] = BackstoryDatabase.allBackstories.Values.Where<BackstoryDef>((Func<BackstoryDef, bool>)(bs => bs.shuffleable && bs.slot == slot && group.Matches(bs))).ToList<BackstoryDef>();
                }
                else {
                    List<BackstoryDef> compatibleBackstories = BackstoryDatabase.ShuffleableBackstoryList(mustBeCompatibleTo.Value, group);
                    BackstoryDatabase.shuffleableBackstoryList[key] = BackstoryDatabase.allBackstories.Values.Where<BackstoryDef>((Func<BackstoryDef, bool>)(bs => bs.shuffleable && bs.slot == slot && group.Matches(bs) && compatibleBackstories.Any<BackstoryDef>((Predicate<BackstoryDef>)(b => !b.requiredWorkTags.OverlapsWithOnAnyWorkType(bs.workDisables))))).ToList<BackstoryDef>();
                }
            }
            return BackstoryDatabase.shuffleableBackstoryList[key];
        }

        public static string StripNumericSuffix(string key) => BackstoryDatabase.regex.Match(key).Captures[0].Value;

        private struct CacheKey : IEquatable<BackstoryDatabase.CacheKey> {
            public BackstorySlot slot;
            public BackstoryCategoryFilter filter;
            public BackstorySlot? mustBeCompatibleTo;

            public CacheKey(
              BackstorySlot slot,
              BackstoryCategoryFilter filter,
              BackstorySlot? mustBeCompatibleTo = null) {
                this.slot = slot;
                this.filter = filter;
                this.mustBeCompatibleTo = new BackstorySlot?();
            }

            public override int GetHashCode() => Gen.HashCombineInt(Gen.HashCombine<BackstoryCategoryFilter>(this.slot.GetHashCode(), this.filter), this.mustBeCompatibleTo.GetHashCode());

            public bool Equals(BackstoryDatabase.CacheKey other) {
                if (this.slot != other.slot || this.filter != other.filter)
                    return false;
                BackstorySlot? mustBeCompatibleTo1 = this.mustBeCompatibleTo;
                BackstorySlot? mustBeCompatibleTo2 = other.mustBeCompatibleTo;
                return mustBeCompatibleTo1.GetValueOrDefault() == mustBeCompatibleTo2.GetValueOrDefault() & mustBeCompatibleTo1.HasValue == mustBeCompatibleTo2.HasValue;
            }

            public override bool Equals(object obj) => obj is BackstoryDatabase.CacheKey other && this.Equals(other);
        }
    }
}
