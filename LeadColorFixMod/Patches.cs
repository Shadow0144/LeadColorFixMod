using Harmony;
using UnityEngine;

namespace LeadColorFixMod
{
    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch(nameof(Db.Initialize))]
    public class Patch_Db_Initialize
    {
        public static void Postfix()
        {
            Debug.Log("Fixing lead...");

            Color32 leadColor = new Color32(156, 166, 181, 255);
            Color32 leadUIColor = new Color32(102, 2, 60, 255);
            Color32 leadShineColor = new Color32(214, 220, 255, 255);
            Color32 leadSpecColor = new Color32(206, 215, 231, 255); //new Color32(165, 171, 192, 255);

            // Update the lead map tiles with the new texture and colors// Load the texture for the lead map tiles
            // If placed in the correct subfolders, the game will automatically load any kanims for us
            Substance lead = ElementLoader.FindElementByHash(SimHashes.Lead).substance; // This gets the map tile object
            Texture2D leadMapTexture = Assets.GetAnim("mod_lead_kanim").textureList[0]; // We don't need most of the kanim, just the texture
            Substance tungsten = ElementLoader.FindElementByHash(SimHashes.Tungsten).substance;
            Material leadMaterial = new Material(tungsten.material); // Copy from tungsten
            leadMaterial.name = "matLead";
            leadMaterial.mainTexture = leadMapTexture;
            leadMaterial.SetColor("_ShineColour", leadShineColor);
            leadMaterial.SetColor("_ColourTint", leadColor);
            leadMaterial.SetColor("_SpecColor", leadSpecColor);
            lead.material = leadMaterial;

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

            // Chunk
            KAnimFile leadAnimFile = Assets.GetAnim("lead_kanim");
            if (leadAnimFile != null)
            {
                lead.anim = leadAnimFile;
            }
            else
            {
                Debug.LogError("Lead KAnimFile not found");
            }

            // Liquid
            Color32 liquidColor = new Color32(102, 2, 60, 255);
            moltenLead.uiColour = liquidColor;
            moltenLead.conduitColour = liquidColor;
            moltenLead.colour = liquidColor;

            // Gas
            Color32 gasColor = new Color32(123, 110, 135, 255);
            leadGas.uiColour = gasColor;
            leadGas.conduitColour = gasColor;
            leadGas.colour = gasColor;

            Debug.Log("...Lead fixed.");
        }
    }
}
