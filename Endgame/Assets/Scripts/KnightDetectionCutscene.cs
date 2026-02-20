using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class KnightDetectHMCutscene : CutsceneBase
{
    private void Update()
    {
        if (state == 0 && !txt)
        {
            frames++;
            if (frames == 1)
            {
                GameObject.Find("Goner").GetComponent<Animator>().enabled = true;
            }
            if (frames == 82)
            {
                PlaySFX("sounds/snd_weaponpull");
            }
            if (frames == 200)
            {
                Destroy(GameObject.Find("Goner"));
                pawn.InitiateBattle(1);
                EndCutscene(enablePlayerMovement: false);
            }
        }
    }

    public override void StartCutscene(params object[] par)
    {
        base.StartCutscene(par);
        gm.DisablePlayerMovement(deactivatePartyMembers: true);
        gm.SetFlag(123, 1);
        GameObject.Find("Goner").transform.parent = GameObject.Find("NPC").transform;
        Destroy(FindFirstObjectByType<CutsceneStart>().gameObject);
        StartText(new string[2] { "* I FEEL THE PRESENCE\n\b  OF THE INTERLOPER", "* YOU WILL FEEL MY\n\b  BLADE, FRIEND" }, new string[2] { "snd_txtgnr", "snd_txtgnr" }, new int[2] {1,1}, new string[2] { "", "" });
    }
}
