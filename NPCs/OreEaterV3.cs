using Terraria;
using Terraria.ModLoader;

namespace AutoStacker.NPCs
{
	[AutoloadHead]
	public class OreEaterV3 : ModNPC
	{
		public override string Texture => "AutoStacker/NPCs/OreEaterV3";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ore Eater Ver.3");
		}

		public override void SetDefaults()
		{
			NPC.townNPC = true;
			NPC.aiStyle = 0;
			NPC.friendly = true;
			NPC.hide = true;
			NPC.homeless = true;
			NPC.width = 0;
			NPC.height = 0;
			NPC.defense = 10000000;
			NPC.lifeMax = 10000000;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
		}

		public override bool CanTownNPCSpawn(int numTownNPCs, int money) => false;

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

		public override bool CheckDead() => true;

		public override string GetChat() => "";

		public override bool CheckActive() => false;
	}
}
