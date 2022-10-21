using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace EdB.PrepareCarefully {
    class PawnGenerationRequestWrapper {
        private PawnKindDef kindDef = Faction.OfPlayer.def.basicMemberKind;
        private Faction faction = Faction.OfPlayer;
        private PawnGenerationContext context = PawnGenerationContext.PlayerStarter;
        private float? fixedBiologicalAge = null;
        private float? fixedChronologicalAge = null;
        private Gender? fixedGender = null;
        private bool worldPawnFactionDoesntMatter = false;
        private bool mustBeCapableOfViolence = false;
        private Ideo fixedIdeology = null;
        public PawnGenerationRequestWrapper() {
        }
        private PawnGenerationRequest CreateRequest() {
            //public PawnGenerationRequest (

            //string fixedBirthName = null, 
            //RoyalTitleDef fixedTitle = null)

            /*
             * PawnKindDef kind
             * Faction faction = null
             * PawnGenerationContext context = PawnGenerationContext.NonPlayer
             * int tile = -1
             * bool forceGenerateNewPawn = false
             * bool newborn = false
             * bool allowDead = false
             * bool allowDowned = false
             * bool canGeneratePawnRelations = true
             * bool mustBeCapableOfViolence = false
             * float colonistRelationChanceFactor = 1
             * bool forceAddFreeWarmLayerIfNeeded = false
             * bool allowGay = true
             * bool allowFood = true
             * bool allowAddictions = true
             * bool inhabitant = false
             * bool certainlyBeenInCryptosleep = false
             * bool forceRedressWorldPawnIfFormerColonist = false
             * bool worldPawnFactionDoesntMatter = false
             * float biocodeWeaponChance = 0
             * float biocodeApparelChance = 0
             * Pawn extraPawnForExtraRelationChance = null
             * float relationWithExtraPawnChanceFactor = 1
             * Predicate<Pawn> validatorPreGear = null
             * Predicate<Pawn> validatorPostGear = null
             * IEnumerable<TraitDef> forcedTraits = null
             * IEnumerable<TraitDef> prohibitedTraits = null
             * float? minChanceToRedressWorldPawn = null
             * float? fixedBiologicalAge = null
             * float? fixedChronologicalAge = null
             * Gender? fixedGender = null
             * float? fixedMelanin = null
             * string fixedLastName = null
             * string fixedBirthName = null
             * RoyalTitleDef fixedTitle = null
             * Ideo fixedIdeo = null
             * bool forceNoIdeo = false
             * bool forceNoBackstory = false);
             */

            return new PawnGenerationRequest(
                kind: kindDef, // PawnKindDef kind
                faction: faction, // Faction faction = null
                context: context, // PawnGenerationContext context = PawnGenerationContext.NonPlayer
                tile: -1, //int tile = -1,
                forceGenerateNewPawn:true, //bool forceGenerateNewPawn = false,
                allowDead: false, //bool allowDead = false,
                allowDowned: false, //bool allowDowned = false,
                canGeneratePawnRelations: false, //bool canGeneratePawnRelations = true,
                mustBeCapableOfViolence:mustBeCapableOfViolence, //bool mustBeCapableOfViolence = false,
                colonistRelationChanceFactor:0f, //float colonistRelationChanceFactor = 1f,
                forceAddFreeWarmLayerIfNeeded:false, //bool forceAddFreeWarmLayerIfNeeded = false,
                allowGay:true, //bool allowGay = true,
                allowFood:false, //bool allowFood = true,
                allowAddictions:false, //bool allowAddictions = true, 
                inhabitant:false, // bool inhabitant = false
                certainlyBeenInCryptosleep:false, // bool certainlyBeenInCryptosleep = false
                forceRedressWorldPawnIfFormerColonist:false, // bool forceRedressWorldPawnIfFormerColonist = false
                worldPawnFactionDoesntMatter:worldPawnFactionDoesntMatter, // bool worldPawnFactionDoesntMatter = false
                biocodeWeaponChance:0f, //float biocodeWeaponChance = 0f, 
                biocodeApparelChance:0f, //float biocodeApparelChance = 0f,
                extraPawnForExtraRelationChance:null, //Pawn extraPawnForExtraRelationChance = null, 
                relationWithExtraPawnChanceFactor:1f, //float relationWithExtraPawnChanceFactor = 1f, 
                validatorPreGear:null, // Predicate < Pawn > validatorPreGear = null
                validatorPostGear:null, // Predicate < Pawn > validatorPostGear = null
                forcedTraits:Enumerable.Empty<TraitDef>(), //IEnumerable<TraitDef> forcedTraits = null, 
                prohibitedTraits:Enumerable.Empty<TraitDef>(), //IEnumerable<TraitDef> prohibitedTraits = null,
                minChanceToRedressWorldPawn:null, // float ? minChanceToRedressWorldPawn = null
                fixedBiologicalAge:fixedBiologicalAge, // float ? fixedBiologicalAge = null
                fixedChronologicalAge:fixedChronologicalAge, // float ? fixedChronologicalAge = null
                fixedGender:fixedGender, // Gender ? fixedGender = null
                fixedLastName:null, // string fixedLastName = null
                fixedBirthName:null, //string fixedBirthName = null, 
                fixedTitle:null, //RoyalTitleDef fixedTitle = null
                fixedIdeo:fixedIdeology, //Ideo fixedIdeo = null
                forceNoIdeo:false, //bool forceNoIdeo = false
                forceNoBackstory:false //bool forceNoBackstory = false
            ) {
                ForbidAnyTitle = true
            };
        }
        public PawnGenerationRequest Request {
            get {
                return CreateRequest();
            }
        }
        public PawnKindDef KindDef {
            set {
                kindDef = value;
            }
        }
        public Faction Faction {
            set {
                faction = value;
            }
        }
        public PawnGenerationContext Context {
            set {
                context = value;
            }
        }
        public bool WorldPawnFactionDoesntMatter {
            set {
                worldPawnFactionDoesntMatter = value;
            }
        }
        public float? FixedBiologicalAge {
            set {
                fixedBiologicalAge = value;
            }
        }
        public float? FixedChronologicalAge {
            set {
                fixedChronologicalAge = value;
            }
        }
        public Gender? FixedGender {
            set {
                fixedGender = value;
            }
        }
        public bool MustBeCapableOfViolence {
            set {
                mustBeCapableOfViolence = value;
            }
        }
        public Ideo FixedIdeology {
            set {
                fixedIdeology = value;
            }
        }
    }
}
