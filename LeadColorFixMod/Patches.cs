using Harmony;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LeadColorFixMod
{
    [HarmonyPatch(typeof(LoadScreen))]
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
            Debug.Log("Updating KAnims...");

            Debug.Log("Lead: " + (substanceTable.GetSubstance(SimHashes.Lead) != null));
            Debug.Log("Lead material: " + substanceTable.GetSubstance(SimHashes.Lead).material.name);

            // Tyrian Purple
            Substance lead = substanceTable.GetSubstance(SimHashes.Lead);

            Color32 leadColor = new Color32(156, 166, 181, 255);
            Color32 leadShineColor = new Color32(208, 217, 222, 255);
            Color32 leadSpecColor = new Color32(165, 171, 192, 255);
            Color32 leadUIColor = new Color32(102, 2, 60, 255);

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

            leadGas.material = leadMaterial;
        }
    }
}
