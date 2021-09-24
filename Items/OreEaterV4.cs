using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.Items
{
	public class OreEaterV4 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ore Eater Ver.4");
			const string str = "Summons a Pet Ore Eater Ver.4\n" +
							   "[status] \n" +
							   "ore serch range      : 40\n" +
							   "speed                : 5\n" +
							   "pick in water        : enable\n" +
							   "pick in lava         : enable\n" +
							   "through block        : enable\n" +
							   "through unreveal map : disenable\n" +
							   "light                : very bright";
			Tooltip.SetDefault(str);
		}

		public override void SetDefaults()
		{
			Item.damage = 0;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.shoot = ModContent.ProjectileType<Projectiles.OreEaterV4>();
			Item.width = 16;
			Item.height = 30;
			Item.UseSound = SoundID.Item2;
			Item.useAnimation = 20;
			Item.useTime = 20;
			Item.rare = ItemRarityID.Yellow;
			Item.noMelee = true;
			Item.value = Item.sellPrice(0, 5, 50);
			Item.buffType = ModContent.BuffType<Buffs.OreEaterV4>();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(null, "OreEaterV3")
				.AddIngredient(ItemID.WormholePotion, 4)
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
