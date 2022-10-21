using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EdB.PrepareCarefully {
    public static class ExtensionsBackstoryDef {

        private static HashSet<string> ProblemBackstories = new HashSet<string>() {
            "pirate king"
        };

        public static string CheckedDescriptionFor(this BackstoryDef BackstoryDef, Pawn pawn) {
            if (ProblemBackstories.Contains(BackstoryDef.untranslatedTitle)) {
                    return PartialDescriptionFor(BackstoryDef);
            }
            string description = BackstoryDef.FullDescriptionFor(pawn).Resolve();
            if (description.StartsWith("Could not resolve")) {
                return PartialDescriptionFor(BackstoryDef);
                //Logger.Debug("Failed to resolve description for BackstoryDef with this pawn: " + BackstoryDef.title + ", " + BackstoryDef.identifier);
            }
            return description;
        }

        // EVERY RELEASE:
        // This is a copy of BackstoryDef.FullDescriptionFor() that only includes the disabled work types and the skill adjustments.
        // Every release, we should evaluate that method to make sure that the logic has not changed.
        public static string PartialDescriptionFor(this BackstoryDef BackstoryDef) {
            StringBuilder stringBuilder = new StringBuilder();
            List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
            for (int i = 0; i < allDefsListForReading.Count; i++) {
                SkillDef skillDef = allDefsListForReading[i];
                if (BackstoryDef.skillGains.ContainsKey(skillDef)) {
                    stringBuilder.AppendLine(skillDef.skillLabel.CapitalizeFirst() + ":   " + BackstoryDef.skillGains[skillDef].ToString("+##;-##"));
                }
            }
            stringBuilder.AppendLine();
            foreach (WorkTypeDef current in BackstoryDef.DisabledWorkTypes) {
                stringBuilder.AppendLine(current.gerundLabel.CapitalizeFirst() + " " + "DisabledLower".Translate());
            }
            foreach (WorkGiverDef current2 in BackstoryDef.DisabledWorkGivers) {
                stringBuilder.AppendLine(current2.workType.gerundLabel.CapitalizeFirst() + ": " + current2.LabelCap + " " + "DisabledLower".Translate());
            }
            string str = stringBuilder.ToString().TrimEndNewlines();
            return str;
        }
    }
}
