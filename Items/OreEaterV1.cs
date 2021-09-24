using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.Items
{
	public class OreEaterV1 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ore Eater Ver.1");
			const string str = "Summons a Pet Ore Eater Ver.1\n" +
							   "[status] \n" +
							   "ore serch range      : 10\n" +
							   "speed                : 3\n" +
							   "pick in water        : disenable\n" +
							   "pick in lava         : disenable\n" +
							   "through block        : disenable\n" +
							   "through unreveal map : disenable\n" +
							   "light                : none";
			Tooltip.SetDefault(str);
		}

		public override void SetDefaults()
		{
			Item.damage = 0;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.shoot = ModContent.ProjectileType<Projectiles.OreEaterV1>();
			Item.width = 16;
			Item.height = 30;
			Item.UseSound = SoundID.Item2;
			Item.useAnimation = 20;
			Item.useTime = 20;
			Item.rare = ItemRarityID.Yellow;
			Item.noMelee = true;
			Item.value = Item.sellPrice(gold: 5, silver: 50);
			Item.buffType = ModContent.BuffType<Buffs.OreEaterV1>();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.IronPickaxe)
				.AddTile(TileID.WorkBenches)
				.Register();

			CreateRecipe()
				.AddIngredient(ItemID.LeadPickaxe)
				.AddTile(TileID.WorkBenches)
				.Register();
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame)
		{
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
			{
				player.ClearBuff(ModContent.BuffType<Buffs.OreEaterV1>());
				player.ClearBuff(ModContent.BuffType<Buffs.OreEaterV2>());
				player.ClearBuff(ModContent.BuffType<Buffs.OreEaterV3>());
				player.ClearBuff(ModContent.BuffType<Buffs.OreEaterV4>());
				player.ClearBuff(ModContent.BuffType<Buffs.OreEaterV5>());

				player.AddBuff(Item.buffType, 3600);
			}
		}
	}
}
