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

    [HarmonyPatch(typeof(SubstanceTable))]
    [HarmonyPatch(nameof(SubstanceTable.GetSubstance))]
    internal static class Patch_SubstanceTable_GetSubstance
    {
        private static Substance Postfix(Substance __result, SimHashes substance)
        {
            Debug.Log("Patching:" + substance.ToString());
            if (substance == SimHashes.Lead)
            {
                Debug.Log("Entering lead patch!");
                Texture2D[] allTextures = Resources.FindObjectsOfTypeAll<Texture2D>();
                TextAsset[] allTextAssets = Resources.FindObjectsOfTypeAll<TextAsset>();

                Texture2D leadTexture = null;
                foreach (Texture2D texture in allTextures)
                {
                    if (texture.name.Equals("lead_0"))
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
                    if (text.name == "lead_anim")
                    {
                        leadAnim = text;
                    }
                    else if (text.name == "lead_build")
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
    }


    [HarmonyPatch(typeof(ElementLoader))]
    [HarmonyPatch(nameof(ElementLoader.Load))]
    internal static class Patch_ElementLoader_Load
    {
        public static String ModPath()
        {
            return Directory.GetParent(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar.ToString();
        }

        private static void Postfix(ref Hashtable substanceList, SubstanceTable substanceTable)
        {
            /*Debug.Log("Updating KAnims...");

            //Debug.Log("Lead: " + (substanceTable.GetSubstance(SimHashes.Lead) != null));
            //Debug.Log("Lead material: " + substanceTable.GetSubstance(SimHashes.Lead).material.name);

            // Tyrian Purple
            Substance lead = substanceTable.GetSubstance(SimHashes.Lead);
            Substance gold = substanceTable.GetSubstance(SimHashes.Gold);
            Substance copper = substanceTable.GetSubstance(SimHashes.Copper);

            Texture2D leadTexture = Resources.Load<Texture2D>("Texture2D/lead_0");
            Texture2D[] allTextures = Resources.FindObjectsOfTypeAll<Texture2D>();
            Debug.Log("T: " + (Resources.GetBuiltinResource<Texture2D>("Texture2D/lead_0") == null));
            Debug.Log("nT: " + (Resources.GetBuiltinResource<Texture2D>("lead_0") == null));
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

            bool leadAnimSet = false;
            bool leadBuildSet = false;
            TextAsset leadAnim = null;
            TextAsset leadBuild = null;
            foreach (TextAsset text in allTextAssets)
            {
                if (text.name == "lead_anim")
                {
                    leadAnim = text;
                }
                else if (text.name == "lead_build")
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
                lead.anim.mod = new KAnimFile.Mod();
                lead.anim.mod.textures = new List<Texture2D>();
                lead.anim.mod.textures.Add(leadTexture);
                lead.anim.mod.anim = leadAnim.bytes;
                lead.anim.mod.build = leadBuild.bytes;
                Debug.Log("Texture Success");
            }
            else
            {
                Debug.LogError("Lead texture not loaded!");
            }

            substanceList[lead.elementID] = lead;
            substanceTable.OnAfterDeserialize();*/

            /*Color32 leadColor = new Color32(156, 166, 181, 255);
            Color32 leadShineColor = new Color32(208, 217, 222, 255);
            Color32 leadSpecColor = new Color32(165, 171, 192, 255);
            Color32 leadUIColor = new Color32(102, 2, 60, 255);

            lead.colour = leadColor;
            lead.uiColour = leadUIColor;

            Material leadMaterial = new Material(lead.material);
            Debug.Log("Original material name: " + lead.material.name);
            Debug.Log("Original material texture: " + lead.material.mainTexture.name);
            leadMaterial.name = "matLead";
            leadMaterial.SetColor("_ShineColour", leadShineColor);
            leadMaterial.SetColor("_ColourTint", leadColor);
            leadMaterial.SetColor("_SpecColor", leadSpecColor);
            lead.material = leadMaterial;

            Texture2D[] textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            for (int i = 0; i < textures.Length; i++)
            {
                //Debug.Log(textures[i].name);
                if (textures[i].name.Equals("lead_0"))
                {
                    //leadMaterial.SetTexture("_MainTex", textures[i]);
                    substanceTable.GetSubstance(SimHashes.Lead).anim.textureList[0] = textures[i];
                    lead.anim.textureList[0] = textures[i];
                    Debug.Log("Success");
                    break;
                }
            }
            //lead.RefreshPropertyBlock();

            Element leadElement = ElementLoader.FindElementByHash(SimHashes.Lead);
            Debug.Log("Did it work? " + (leadElement != null));
            leadElement.substance.material = leadMaterial;

            Substance moltenLead = substanceTable.GetSubstance(SimHashes.MoltenLead);

            moltenLead.uiColour.r = 102;
            moltenLead.uiColour.g = 2;
            moltenLead.uiColour.b = 60;

            moltenLead.material = leadMaterial;

            Substance leadGas = substanceTable.GetSubstance(SimHashes.LeadGas);

            leadGas.uiColour.r = 102;
            leadGas.uiColour.g = 2;
            leadGas.uiColour.b = 60;

            leadGas.material = leadMaterial;*/
        }
    }
}
