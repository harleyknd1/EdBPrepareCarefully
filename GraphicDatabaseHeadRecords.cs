using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Linq;

namespace Verse {
    public static class GraphicDatabaseHeadRecords {
        private static List<GraphicDatabaseHeadRecords.HeadGraphicRecord> heads = new List<GraphicDatabaseHeadRecords.HeadGraphicRecord>();
        private static GraphicDatabaseHeadRecords.HeadGraphicRecord skull;
        private static GraphicDatabaseHeadRecords.HeadGraphicRecord stump;
        private static readonly string[] HeadsFolderPaths = new string[2]
        {
      "Things/Pawn/Humanlike/Heads/Male",
      "Things/Pawn/Humanlike/Heads/Female"
        };
        private static readonly string SkullPath = "Things/Pawn/Humanlike/Heads/None_Average_Skull";
        private static readonly string StumpPath = "Things/Pawn/Humanlike/Heads/None_Average_Stump";

        public static void Reset() {
            GraphicDatabaseHeadRecords.heads.Clear();
            GraphicDatabaseHeadRecords.skull = (GraphicDatabaseHeadRecords.HeadGraphicRecord)null;
            GraphicDatabaseHeadRecords.stump = (GraphicDatabaseHeadRecords.HeadGraphicRecord)null;
        }

        private static void BuildDatabaseIfNecessary() {
            if (GraphicDatabaseHeadRecords.heads.Count > 0 && GraphicDatabaseHeadRecords.skull != null && GraphicDatabaseHeadRecords.stump != null)
                return;
            GraphicDatabaseHeadRecords.heads.Clear();
            foreach (string headsFolderPath in GraphicDatabaseHeadRecords.HeadsFolderPaths) {
                foreach (string str in GraphicDatabaseUtility.GraphicNamesInFolder(headsFolderPath))
                    GraphicDatabaseHeadRecords.heads.Add(new GraphicDatabaseHeadRecords.HeadGraphicRecord(headsFolderPath + "/" + str));
            }
            GraphicDatabaseHeadRecords.skull = new GraphicDatabaseHeadRecords.HeadGraphicRecord(GraphicDatabaseHeadRecords.SkullPath);
            GraphicDatabaseHeadRecords.stump = new GraphicDatabaseHeadRecords.HeadGraphicRecord(GraphicDatabaseHeadRecords.StumpPath);
        }

        public static Graphic_Multi GetHeadNamed(
          string graphicPath,
          Color skinColor,
          bool skinColorOverriden) {
            GraphicDatabaseHeadRecords.BuildDatabaseIfNecessary();
            for (int index = 0; index < GraphicDatabaseHeadRecords.heads.Count; ++index) {
                GraphicDatabaseHeadRecords.HeadGraphicRecord head = GraphicDatabaseHeadRecords.heads[index];
                if (head.graphicPath == graphicPath)
                    return head.GetGraphic(skinColor, skinColorOverriden: skinColorOverriden);
            }
            Log.Message("Tried to get pawn head at path " + graphicPath + " that was not found. Defaulting...");
            return heads.First<GraphicDatabaseHeadRecords.HeadGraphicRecord>().GetGraphic(skinColor, skinColorOverriden: skinColorOverriden);
        }

        public static Graphic_Multi GetSkull() {
            BuildDatabaseIfNecessary();
            return skull.GetGraphic(Color.white, true);
        }

        public static Graphic_Multi GetStump(Color skinColor) {
            BuildDatabaseIfNecessary();
            return stump.GetGraphic(skinColor);
        }

        public static Graphic_Multi GetHeadRandom(
          Gender gender,
          Color skinColor,
          bool skinColorOverriden) {
            BuildDatabaseIfNecessary();
            Predicate<HeadGraphicRecord> predicate = (Predicate<HeadGraphicRecord>)(head => head.gender == gender);
            int num = 0;
            do {
                GraphicDatabaseHeadRecords.HeadGraphicRecord headGraphicRecord = heads.RandomElement<GraphicDatabaseHeadRecords.HeadGraphicRecord>();
                if (predicate(headGraphicRecord))
                    return headGraphicRecord.GetGraphic(skinColor);
                ++num;
            }
            while (num <= 40);
            foreach (HeadGraphicRecord headGraphicRecord in heads.InRandomOrder<GraphicDatabaseHeadRecords.HeadGraphicRecord>()) {
                if (predicate(headGraphicRecord))
                    return headGraphicRecord.GetGraphic(skinColor);
            }
            Log.Error("Failed to find head for gender=" + (object)gender + ". Defaulting...");
            return heads.First<HeadGraphicRecord>().GetGraphic(skinColor);
        }

        private class HeadGraphicRecord {
            public Gender gender;
            public string graphicPath;
            private List<KeyValuePair<Color, Graphic_Multi>> graphics = new List<KeyValuePair<Color, Graphic_Multi>>();

            public HeadGraphicRecord(string graphicPath) {
                this.graphicPath = graphicPath;
                string[] strArray = Path.GetFileNameWithoutExtension(graphicPath).Split('_');
                try {
                    this.gender = ParseHelper.FromString<Gender>(strArray[strArray.Length - 3]);
                }
                catch (Exception ex) {
                    Log.Error("Parse error with head graphic at " + graphicPath + ": " + ex.Message);
                    this.gender = Gender.None;
                }
            }

            public Graphic_Multi GetGraphic(
              Color color,
              bool dessicated = false,
              bool skinColorOverriden = false) {
                Shader shader = !dessicated ? ShaderUtility.GetSkinShader(skinColorOverriden) : ShaderDatabase.Cutout;
                for (int index = 0; index < this.graphics.Count; ++index) {
                    Color colA = color;
                    KeyValuePair<Color, Graphic_Multi> graphic = this.graphics[index];
                    Color key = graphic.Key;
                    if (colA.IndistinguishableFrom(key)) {
                        graphic = this.graphics[index];
                        if ((UnityEngine.Object)graphic.Value.Shader == (UnityEngine.Object)shader) {
                            graphic = this.graphics[index];
                            return graphic.Value;
                        }
                    }
                }
                Graphic_Multi graphic1 = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>(this.graphicPath, shader, Vector2.one, color);
                this.graphics.Add(new KeyValuePair<Color, Graphic_Multi>(color, graphic1));
                return graphic1;
            }
        }
    }
}
