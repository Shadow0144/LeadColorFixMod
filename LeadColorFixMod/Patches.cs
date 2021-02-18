using Harmony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LeadColorFixMod
{
    /*[HarmonyPatch(typeof(Assets))]
    [HarmonyPatch(nameof(Assets.GetSprite))]
   
    internal static class Patch_Assets_Test
    {
        private static void Prefix(HashedString name)
        {
            Debug.Log(name);
        }
    }*/

    /*[HarmonyPatch(typeof(LoadScreen))]
    [HarmonyPatch(nameof(LoadScreen.Activate))]
    internal static class Patch_ElementTester_Load
    {
        private static void Prefix()
        {
            Texture2D[] textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            for (int i = 0; i < textures.Length; i++)
            {
                //Debug.Log(textures[i].name);
                if (textures[i].name.Equals("lead_0"))
                {
                    //leadMaterial.SetTexture("_MainTex", textures[i]);
                    Assets.SubstanceTable.GetSubstance(SimHashes.Lead).anim.textureList[0] = textures[i];
                    //lead.anim.textureList[0] = textures[i];
                    Debug.Log("Success?");
                    break;
                }
            }
        }
    }*/

    /*[HarmonyPatch(typeof(SubstanceTable))]
    [HarmonyPatch(nameof(SubstanceTable.GetSubstance))]
    internal static class Patch_SubstanceTable_GetSubstance
    {
        private static Substance Postfix(Substance __result, SimHashes substance)
        {
            if (substance == SimHashes.Lead)
            {
                Debug.Log("Entering lead patch!");
                Texture2D[] allTextures = Resources.FindObjectsOfTypeAll<Texture2D>();
                TextAsset[] allTextAssets = Resources.FindObjectsOfTypeAll<TextAsset>();

                Texture2D leadTexture = null;
                foreach (Texture2D texture in allTextures)
                {
                    if (texture.name.Equals("aluminum_0"))
                    {
                        leadTexture = texture;
                        break;
                    }
                    else { }
                }

                bool leadAnimSet = false;
                bool leadBuildSet = false;
                TextAsset leadAnim = null;
                TextAsset leadBuild = null;
                foreach (TextAsset text in allTextAssets)
                {
                    if (text.name == "aluminum_anim")
                    {
                        leadAnim = text;
                    }
                    else if (text.name == "aluminum_build")
                    {
                        leadBuild = text;
                    }
                    else { }
                    if (leadAnimSet && leadBuildSet)
                    {
                        break;
                    }
                    else { }
                }

                if (leadTexture != null && leadAnim != null && leadBuild != null)
                {
                    __result.anim.mod = new KAnimFile.Mod();
                    __result.anim.mod.textures = new List<Texture2D>();
                    __result.anim.mod.textures.Add(leadTexture);
                    __result.anim.mod.anim = leadAnim.bytes;
                    __result.anim.mod.build = leadBuild.bytes;
                    Debug.Log("Texture Success");
                }
                else
                {
                    Debug.LogError("Lead texture not loaded!");
                }
            }
            else { }

            return __result;
        }
    }*/


    [HarmonyPatch(typeof(ElementLoader))]
    [HarmonyPatch(nameof(ElementLoader.Load))]
    internal static class Patch_ElementLoader_Load
    {
        public static String ModPath()
        {
            return Directory.GetParent(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar.ToString();
        }

        public static Texture2D LoadTexture(String filePath)
        {
            Texture2D texture2D = null;

            if (File.Exists(filePath))
            {
                texture2D = Resources.Load(filePath) as Texture2D;
            }
            else { }

            return texture2D;
        }

        private static void Postfix(ref Hashtable substanceList, SubstanceTable substanceTable)
        {
            Debug.Log("Updating KAnims...");

            //Debug.Log("Lead: " + (substanceTable.GetSubstance(SimHashes.Lead) != null));
            //Debug.Log("Lead material: " + substanceTable.GetSubstance(SimHashes.Lead).material.name);

            // Tyrian Purple
            Substance lead = substanceTable.GetSubstance(SimHashes.Lead);
            Substance gold = substanceTable.GetSubstance(SimHashes.Gold);
            Substance copper = substanceTable.GetSubstance(SimHashes.Copper);

            Texture2D leadTexture = null;
            Texture2D[] allTextures = Resources.FindObjectsOfTypeAll<Texture2D>();
            TextAsset[] allTextAssets = Resources.FindObjectsOfTypeAll<TextAsset>();
            
            foreach (Texture2D texture in allTextures)
            {
                if (texture.name.Equals("lead_0"))
                {
                    leadTexture = texture;
                    break;
                }
                else { }
            }
            List<Texture2D> leadTextures = new List<Texture2D>();
            leadTextures.Add(leadTexture);

            bool leadAnimSet = false;
            bool leadBuildSet = false;
            TextAsset leadAnim = null;
            TextAsset leadBuild = null;
            foreach (TextAsset text in allTextAssets)
            {
                if (text.name.Equals("lead_anim"))
                {
                    leadAnim = text;
                    leadAnimSet = true;
                }
                else if (text.name.Equals("lead_build"))
                {
                    leadBuild = text;
                    leadBuildSet = true;
                }
                else { }
                if (leadAnimSet && leadBuildSet)
                {
                    Debug.Log("TextAssets found");
                    break;
                }
                else { }
            }

            if (leadTexture != null && leadAnim != null && leadBuild != null)
            {
                lead.anim.Initialize(leadAnim, leadBuild, leadTextures);

                KAnimFile[] anims = new KAnimFile[1];
                lead.GetType().GetField("anims", System.Reflection.BindingFlags.NonPublic
                                                         | System.Reflection.BindingFlags.Instance)
                     .SetValue(lead, anims);
                Debug.Log("Texture Success");
            }
            else
            {
                Debug.LogError("Lead texture not loaded!");
            }

            substanceList[lead.elementID] = lead;

            Element leadElement = ElementLoader.FindElementByHash(lead.elementID);
            leadElement.substance.anim.Initialize(leadAnim, leadBuild, leadTextures);

            //Debug.Log(ModPath() + "lead.png");
            //String path = ModPath() + "lead.png";
            //if (File.Exists(path))
            {
                //KAnimFile leadMatTexture = Assets.GetAnim("mod_lead");
                
                /*Texture2D leadMatTexture = new Texture2D(1024, 1024, TextureFormat.DXT5, false);
                leadMatTexture.Apply(true, true);*/
                /*if (leadMatTexture == null)
                {
                    Debug.LogError("Error!!!");
                }
                else 
                {
                    Debug.LogError("Wow!!!");
                }*/
            }
            /*else
            {
                Debug.Log("File does not exist");
            }*/

            // The material affects the undug texture
            /*lead.material = new Material(leadElement.substance.material);
            lead.material.CopyPropertiesFromMaterial(gold.material);
            lead.material.mainTexture = leadMatTexture;*/

            //BindAnimList()
            var list = substanceTable.GetList();
            foreach (Substance s in list)
            {
                if (s.name.Equals("Lead"))
                {
                    s.anim.Initialize(leadAnim, leadBuild, leadTextures);
                    Debug.Log("Anim: " + s.anim.name);
                }
            }

            
        }
    }


    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch(nameof(Db.Initialize))]
    public class Patch_Db_Initialize
    {
        public static void Postfix()
        {
            Debug.Log("Db.Initialize");
            Dictionary<HashedString, KAnimFile> table = Traverse.Create<Assets>().Field("AnimTable").GetValue<Dictionary<HashedString, KAnimFile>>();

            Debug.Log("Contains: " + table.ContainsKey("lead_kanim"));

            Debug.Log("Texture: " + table["lead_kanim"].textureList[0].name);

            table["lead_kanim"] = Assets.GetAnim("copper_refined_kanim");
            table["gold_refined_kanim"] = Assets.GetAnim("aluminum_refined_kanim");

            ////
            Texture2D leadTexture = null;
            Texture2D[] allTextures = Resources.FindObjectsOfTypeAll<Texture2D>();
            TextAsset[] allTextAssets = Resources.FindObjectsOfTypeAll<TextAsset>();

            leadTexture = Assets.GetAnim("mod_lead_kanim").textureList[0];
            List<Texture2D> leadTextures = new List<Texture2D>();
            leadTextures.Add(leadTexture);

            bool leadAnimSet = false;
            bool leadBuildSet = false;
            TextAsset leadAnim = null;
            TextAsset leadBuild = null;
            foreach (TextAsset text in allTextAssets)
            {
                if (text.name.Equals("lead_anim"))
                {
                    leadAnim = text;
                    leadAnimSet = true;
                }
                else if (text.name.Equals("lead_build"))
                {
                    leadBuild = text;
                    leadBuildSet = true;
                }
                else { }
                if (leadAnimSet && leadBuildSet)
                {
                    Debug.Log("TextAssets found");
                    break;
                }
                else { }
            }

            Substance lead = ElementLoader.FindElementByHash(SimHashes.Lead).substance;
            lead.anim.Initialize(leadAnim, leadBuild, leadTextures);

            Color32 leadColor = new Color32(156, 166, 181, 255);
            Color32 leadShineColor = new Color32(214, 220, 255, 255);
            Color32 leadSpecColor = new Color32(206, 215, 231, 255); //new Color32(165, 171, 192, 255);

            Substance tungsten = ElementLoader.FindElementByHash(SimHashes.Tungsten).substance;
            Material leadMaterial = new Material(tungsten.material);
            leadMaterial.name = "matLead";
            leadMaterial.mainTexture = leadTexture;
            /*Color shine = leadMaterial.GetColor("_ShineColour");
            Debug.Log("Original shine color: " + shine.r + ", " + shine.g + ", " + shine.b);*/
            leadMaterial.SetColor("_ShineColour", leadShineColor);
            /*Color tint = leadMaterial.GetColor("_TintColour");
            Debug.Log("Original tint color: " + tint.r + ", " + tint.g + ", " + tint.b);*/
            leadMaterial.SetColor("_ColourTint", leadColor);
            /*Color spec = leadMaterial.GetColor("_SpecColour");
            Debug.Log("Original spec color: " + spec.r + ", " + spec.g + ", " + spec.b);*/
            leadMaterial.SetColor("_SpecColor", leadSpecColor);
            lead.material = leadMaterial;

            Color32 leadUIColor = new Color32(102, 2, 60, 255);

            // Fix the liquid and gas colors
            Substance moltenLead = ElementLoader.FindElementByHash(SimHashes.MoltenLead).substance;
            Substance leadGas = ElementLoader.FindElementByHash(SimHashes.LeadGas).substance;

            // Note: The liquid and gas don't have a material
            // Their anim is their swept container
            // Color controls the color of the liquid and gas
            // Conduit color controls the color of the pumped liquid and gas
            // UI color controls the overlay color

            // Solid
            lead.uiColour = leadUIColor;
            lead.conduitColour = leadUIColor;
            lead.colour = Color.white; // leadUIColor;

            // Liquid
            moltenLead.uiColour = leadUIColor;
            moltenLead.conduitColour = leadUIColor;
            moltenLead.colour = leadUIColor;

            // Gas
            leadGas.uiColour = leadUIColor;
            leadGas.conduitColour = leadUIColor;
            leadGas.colour = leadUIColor;
        }
    }
}
