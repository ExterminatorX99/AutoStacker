using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AutoStacker.Players
{
	public class ItemVacuumerV2 : ModPlayer
	{
		public static bool VacuumSwitch;
		public int SerchNumber;
		private Vector2 _prePosition = Main.LocalPlayer.Center;

		public ItemVacuumerV2()
		{
			VacuumSwitch = false;
		}

		public override void SaveData(TagCompound tag)
		{
			tag.Set("vacuumSwitchV2", VacuumSwitch);
		}

		public override void LoadData(TagCompound tag)
		{
			VacuumSwitch = tag.ContainsKey("vacuumSwitchV2") && tag.GetBool("vacuumSwitchV2");
		}

		public override void PreUpdate()
		{
			if (VacuumSwitch)
			{
				Item item = Main.item[SerchNumber];
				Player player = Main.LocalPlayer;
				if (item.active && item.noGrabDelay == 0 && !ItemLoader.GrabStyle(item, player) && ItemLoader.CanPickup(item, player))
				{
					item.position = player.Center + (player.Center - _prePosition);
					if (item.position.X < 0)
						item.position.X = 0;
					else if (item.position.X > Main.rightWorld)
						item.position.X = Main.rightWorld;
					if (item.position.Y < 0)
						item.position.Y = 0;
					else if (item.position.Y > Main.bottomWorld)
						item.position.Y = Main.bottomWorld;
				}

				SerchNumber += 1;
				if (SerchNumber >= Main.item.Length)
					SerchNumber = 0;
			}

			_prePosition = Main.LocalPlayer.Center;
		}
	}
}
