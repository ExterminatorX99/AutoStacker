using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.Items
{
	public class ReflectionField : ModItem
	{
		private const int ReflectDictance = 16 * 8;
		private const int AwayDictance = 16 * 6;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Reflection Field");
			Tooltip.SetDefault("Enemys will be move away from you.");
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.value = Item.buyPrice(0, 10);
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			foreach (NPC npc in Main.npc)
			{
				if (
					npc.townNPC || npc.friendly || npc.damage == 0
				)
					continue;
				Vector2 distance = player.Center - npc.Center;
				float distanceOffset = (float)(Math.Max(npc.width, npc.height) * 0.5);

				if (distanceOffset > Math.Abs(distance.X))
					distance.X += distance.X >= 0 ? -1 : 1;
				else
					distance.X += (distance.X >= 0 ? -1 : 1) * distanceOffset;

				if (distanceOffset > Math.Abs(distance.Y))
					distance.Y += distance.Y >= 0 ? -1 : 1;
				else
					distance.Y += (distance.Y >= 0 ? -1 : 1) * distanceOffset;

				float distanceSum = Math.Abs(distance.X) + Math.Abs(distance.Y);
				if (distanceSum <= ReflectDictance)
				{
					npc.velocity.X = distance.X >= 0 ? -1 * Math.Abs(npc.velocity.X) : Math.Abs(npc.velocity.X);
					npc.velocity.Y = distance.Y >= 0 ? -1 * Math.Abs(npc.velocity.Y) : Math.Abs(npc.velocity.Y);
					if (distanceSum <= AwayDictance)
					{
						npc.velocity.X = distance.X >= 0 ? -32 + distanceSum / AwayDictance * 32 : 32 - distanceSum / AwayDictance * 32;
						npc.velocity.Y = distance.Y >= 0 ? -32 + distanceSum / AwayDictance * 32 : 32 - distanceSum / AwayDictance * 32;

						npc.position.X += distance.X >= 0 ? -32 + distanceSum / AwayDictance * 32 : 32 - distanceSum / AwayDictance * 32;
						npc.position.Y += distance.Y >= 0 ? -32 + distanceSum / AwayDictance * 32 : 32 - distanceSum / AwayDictance * 32;
					}
				}
			}

			foreach (Projectile projectile in Main.projectile)
			{
				if (
					projectile.whoAmI == Main.myPlayer ||
					projectile.damage == 0 ||
					projectile.friendly ||
					projectile.minion && projectile.OwnerMinionAttackTargetNPC is null
				)
					continue;

				Vector2 distance = player.Center - projectile.Center;
				float distanceOffset = (float)(Math.Max(projectile.width, projectile.height) * 0.5);

				if (distanceOffset > Math.Abs(distance.X))
					distance.X += distance.X >= 0 ? -1 : 1;
				else
					distance.X += (distance.X >= 0 ? -1 : 1) * distanceOffset;

				if (distanceOffset > Math.Abs(distance.Y))
					distance.Y += distance.Y >= 0 ? -1 : 1;
				else
					distance.Y += (distance.Y >= 0 ? -1 : 1) * distanceOffset;

				float distanceSum = Math.Abs(distance.X) + Math.Abs(distance.Y);
				if (distanceSum <= ReflectDictance)
				{
					projectile.velocity.X = distance.X >= 0 ? -1 * Math.Abs(projectile.velocity.X) : Math.Abs(projectile.velocity.X);
					projectile.velocity.Y = distance.Y >= 0 ? -1 * Math.Abs(projectile.velocity.Y) : Math.Abs(projectile.velocity.Y);
					if (distanceSum <= AwayDictance)
					{
						projectile.velocity.X = distance.X >= 0 ? -32 + distanceSum / AwayDictance * 32 : 32 - distanceSum / AwayDictance * 32;
						projectile.velocity.Y = distance.Y >= 0 ? -32 + distanceSum / AwayDictance * 32 : 32 - distanceSum / AwayDictance * 32;

						projectile.position.X += distance.X >= 0 ? -32 + distanceSum / AwayDictance * 32 : 32 - distanceSum / AwayDictance * 32;
						projectile.position.Y += distance.Y >= 0 ? -32 + distanceSum / AwayDictance * 32 : 32 - distanceSum / AwayDictance * 32;
					}

					projectile.owner = player.whoAmI;
					projectile.whoAmI = player.whoAmI;
					projectile.damage *= 2;
				}
			}
		}

		public override Color? GetAlpha(Color lightColor) => Color.White;

		public override bool CanEquipAccessory(Player player, int slot) => true;

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.MagicMirror, 3)
				.Register();
		}
	}
}
