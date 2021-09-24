using Terraria.ModLoader;

namespace AutoStacker
{
	public class AutoStacker : Mod
	{
		public static bool MagicStorageLoaded { get; private set; }

		public override void Load()
		{
			MagicStorageLoaded = ModLoader.HasMod("MagicStorage");
		}
	}
}
