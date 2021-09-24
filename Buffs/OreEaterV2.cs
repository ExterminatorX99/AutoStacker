using System.Linq;
using AutoStacker.Players;
using AutoStacker.Projectiles;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace AutoStacker.Buffs
{
	public class OreEaterV2 : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ore Eater Ver.2");
			Description.SetDefault("");
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (!Program.LoadedEverything)
				return;

			OreEater modPlayer = player.GetModPlayer<OreEater>();

			if (!modPlayer.OreEaterEnable)
			{
				modPlayer.OreEaterEnable = true;
				bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.OreEaterV2>()] <= 0;
				if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
					for (int type = TextureAssets.Npc.Length - 1; type >= 0; type--)
					{
						string npcTexture = TextureAssets.Npc[type].ToString();
						if (npcTexture == "AutoStacker/NPCs/OreEaterV2")
						{
							foreach (NPC npc in Main.npc.Where(npc => npc.type == type))
								npc.active = false;
							modPlayer.Index = NPC.NewNPC((int)player.position.X, (int)player.position.Y, type);
							modPlayer.NPC = Main.npc[modPlayer.Index];
							NPC.setNPCName("Ore Eater", modPlayer.NPC.type);

							Projectile.NewProjectile(player.GetProjectileSource_Buff(buffIndex), player.position.X + player.width / 2,
								player.position.Y + player.height / 2, 0f, 0f, ModContent.ProjectileType<Projectiles.OreEaterV2>(), 0, 0f, player.whoAmI);
							//modPlayer.pet.initListA();
							//modPlayer.pet.routeListX.Clear();
							//modPlayer.pet.routeListY.Clear();
							modPlayer.Pet = new PetV2();
							break;
						}
					}
			}
		}
	}
}
