using System.Linq;
using AutoStacker.Projectiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AutoStacker.Players
{
	public class OreEater : ModPlayer
	{
		public bool OreEaterEnable;
		public int Type = 0;
		public int Index = 0;
		public NPC NPC;

		public PetBase Pet;
		public bool FindRoute = false;

		public override void ResetEffects()
		{
			OreEaterEnable = false;
		}

		public override void SaveData(TagCompound tag)
		{
			foreach (NPC npc in Main.npc.Where(npc => npc.type == Type))
				npc.active = false;
		}
	}
}
