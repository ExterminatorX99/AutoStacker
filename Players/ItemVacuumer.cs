using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AutoStacker.Players
{
	public class ItemVacuumer : ModPlayer
	{
		public static bool VacuumSwitch;

		public ItemVacuumer()
		{
			VacuumSwitch = false;
		}

		public override void SaveData(TagCompound tag)
		{
			tag.Set("vacuumSwitch", VacuumSwitch);
		}

		public override void LoadData(TagCompound tag)
		{
			VacuumSwitch = tag.ContainsKey("vacuumSwitch") && tag.GetBool("vacuumSwitch");
		}

		public override void PreUpdate()
		{
			if (VacuumSwitch)
			{
				Player player = Main.LocalPlayer;
				const int velocity = 12;
				foreach (Item item in Main.item)
					if (item.active && item.noGrabDelay == 0 && !ItemLoader.GrabStyle(item, player) && ItemLoader.CanPickup(item, player))
					{
						int distanceX = (int)player.Center.X - (int)item.position.X;
						int distanceY = (int)player.Center.Y - (int)item.position.Y;
						int distanceSum = Math.Abs(distanceX) + Math.Abs(distanceY);
						if (distanceSum > 0)
						{
							if (item.velocity.X <= 1 && item.velocity.Y <= 1)
							{
								item.velocity.X = (float)(velocity * distanceX / distanceSum + distanceX * 0.0125);
								item.velocity.Y = (float)(velocity * distanceY / distanceSum + distanceY * 0.0125);
								item.position.X += item.velocity.X;
								item.position.Y += item.velocity.Y;
							}
							else
							{
								item.velocity.X = (float)(velocity * distanceX / distanceSum + distanceX * 0.0125);
								item.velocity.Y = (float)(velocity * distanceY / distanceSum + distanceY * 0.0125);
							}
						}
					}
			}
		}
	}
}
