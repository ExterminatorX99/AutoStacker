using Terraria;
using Terraria.ModLoader;

namespace AutoStacker.Players
{
	public class MinionHouse : ModPlayer
	{
		public override void OnEnterWorld(Player player)
		{
			Worlds.MinionHouse minionHouse = ModContent.GetInstance<Worlds.MinionHouse>();
			minionHouse.Init();
		}
	}
}
