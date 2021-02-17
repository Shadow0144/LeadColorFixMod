using Harmony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LeadColorFixMod
{
    /*public static String ModPath()
    {
        return Directory.GetParent(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar.ToString();
    }*/

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
                /*lead.anim.mod = new KAnimFile.Mod();
                lead.anim.mod.textures = new List<Texture2D>();
                lead.anim.mod.textures.Add(leadTexture);
                lead.anim.mod.anim = leadAnim.bytes;
                lead.anim.mod.build = leadBuild.bytes;*/

                lead.anim.Initialize(leadAnim, leadBuild, leadTextures);

                KAnimFile[] anims = new KAnimFile[1];
                lead.GetType().GetField("anims", System.Reflection.BindingFlags.NonPublic
                                                         | System.Reflection.BindingFlags.Instance)
                     .SetValue(lead, anims);

                /*Debug.Log("Trying...");
                
                lead.anim.GetType().GetField("textures", System.Reflection.BindingFlags.NonPublic
                                                         | System.Reflection.BindingFlags.Instance)
                     .SetValue(lead.anim, textures);

                Debug.Log("Trying...");

                lead.anim.GetType().GetField("animBytes", System.Reflection.BindingFlags.NonPublic 
                                                         | System.Reflection.BindingFlags.Instance)
                    .SetValue(lead.anim, leadAnim.bytes);

                Debug.Log("Trying...");

                lead.anim.GetType().GetField("buildBytes", System.Reflection.BindingFlags.NonPublic
                                                         | System.Reflection.BindingFlags.Instance)
                    .SetValue(lead.anim, leadBuild.bytes);*/

                Debug.Log("Texture Success");
            }
            else
            {
                Debug.LogError("Lead texture not loaded!");
            }

            substanceList[lead.elementID] = lead;

            Element leadElement = ElementLoader.FindElementByHash(lead.elementID);
            leadElement.substance.anim.Initialize(leadAnim, leadBuild, leadTextures);

            // The material affects the undug texture
            lead.material = new Material(leadElement.substance.material.shader);
            lead.material.CopyPropertiesFromMaterial(gold.material);
            lead.material.mainTexture = leadTexture;

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
            //substanceTable.GetType().GetMethod("BindAnimList", BindingFlags.NonPublic).Invoke(substanceTable, null);

            /*Material leadMaterial = new Material(lead.material);
            Debug.Log("Original material name: " + lead.material.name);
            Debug.Log("Original material texture: " + lead.material.mainTexture.name);
            leadMaterial.name = "matLead";
            leadMaterial.SetColor("_ShineColour", leadShineColor);
            leadMaterial.SetColor("_ColourTint", leadColor);
            leadMaterial.SetColor("_SpecColor", leadSpecColor);
            lead.material = leadMaterial;

            Element leadElement = ElementLoader.FindElementByHash(SimHashes.Lead);
            Debug.Log("Did it work? " + (leadElement != null));
            leadElement.substance.material = leadMaterial;*/

            Color32 leadColor = new Color32(156, 166, 181, 255);
            Color32 leadShineColor = new Color32(208, 217, 222, 255);
            Color32 leadSpecColor = new Color32(165, 171, 192, 255);
            Color32 leadUIColor = new Color32(102, 2, 60, 255);

            Substance moltenLead = substanceTable.GetSubstance(SimHashes.MoltenLead);
            Substance leadGas = substanceTable.GetSubstance(SimHashes.LeadGas);

            // Color controls the color of the liquid and gas
            // Conduit color controls the color of the pumped liquid and gas
            // UI color controls the overlay color
            lead.uiColour = leadUIColor;
            moltenLead.uiColour = leadUIColor;
            moltenLead.conduitColour = leadUIColor;
            moltenLead.colour = leadUIColor;
            leadGas.uiColour = leadUIColor;
            leadGas.conduitColour = leadUIColor;
            leadGas.colour = leadUIColor;
        }
    }

    // Note: The liquid and gas don't have a material
    // Their anim is their swept container

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

            Element leadElement = ElementLoader.FindElementByHash(SimHashes.Lead);
            leadElement.substance.anim.Initialize(leadAnim, leadBuild, leadTextures);
            /////

            //var foo = typeof(SubstanceTable).GetMethod("MethodName", BindingFlags.NonPublic | BindingFlags.Instance(or Static));
            //foo.Invoke(null, null);

        }
    }
}
