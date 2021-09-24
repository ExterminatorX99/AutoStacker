using Terraria;
using Terraria.ModLoader;

namespace AutoStacker.Worlds
{
	public class QuickLiquid : ModSystem
	{
		public static LiquidBuffer2[] LiquidBuffer2 = new LiquidBuffer2[100000];

		//int count=0;
		public static bool QuickSwitch = false;

		public QuickLiquid()
		{
			for (int i = 0; i < LiquidBuffer2.Length; i++)
				LiquidBuffer2[i] = new LiquidBuffer2();
		}

		public override void PreUpdateWorld()
		{
			/*
			count++;
			if(count >= 100)
			{
				count = 0;
				Main.NewText(Liquid.numLiquid +","+ LiquidBuffer.numLiquidBuffer + "," + LiquidBuffer2.numLiquidBuffer);
			}
			*/

			if (QuickSwitch)
			{
				Liquid.cycles = 1;
				Liquid.panicCounter = 0;

				while (LiquidBuffer.numLiquidBuffer > 5000 && Worlds.LiquidBuffer2.NumLiquidBuffer != 100000 - 1)
				{
					Worlds.LiquidBuffer2.AddBuffer(Main.liquidBuffer[LiquidBuffer.numLiquidBuffer - 1].x,
						Main.liquidBuffer[LiquidBuffer.numLiquidBuffer - 1].y);
					LiquidBuffer.DelBuffer(LiquidBuffer.numLiquidBuffer - 1);
				}

				while (LiquidBuffer.numLiquidBuffer < 5000 && Worlds.LiquidBuffer2.NumLiquidBuffer != 0)
				{
					//LiquidBuffer.AddBuffer(liquidBuffer2[LiquidBuffer2.numLiquidBuffer].x,liquidBuffer2[LiquidBuffer2.numLiquidBuffer].y);
					Main.liquidBuffer[LiquidBuffer.numLiquidBuffer].x = LiquidBuffer2[Worlds.LiquidBuffer2.NumLiquidBuffer - 1].x;
					Main.liquidBuffer[LiquidBuffer.numLiquidBuffer].y = LiquidBuffer2[Worlds.LiquidBuffer2.NumLiquidBuffer - 1].y;
					LiquidBuffer.numLiquidBuffer++;

					Worlds.LiquidBuffer2.DelBuffer(Worlds.LiquidBuffer2.NumLiquidBuffer - 1);
				}

				Liquid.UpdateLiquid();
			}
		}
	}

	public class LiquidBuffer2 : LiquidBuffer
	{
		public static int NumLiquidBuffer { get; set; }

		public new static void AddBuffer(int x, int y)
		{
			//if (LiquidBuffer2.numLiquidBuffer == 1000000)
			//{
			//	return;
			//}
			//if (Main.tile[x, y].checkingLiquid())
			//{
			//	return;
			//}
			//Main.tile[x, y].checkingLiquid(true);
			QuickLiquid.LiquidBuffer2[NumLiquidBuffer].x = x;
			QuickLiquid.LiquidBuffer2[NumLiquidBuffer].y = y;
			NumLiquidBuffer++;
		}

		public new static void DelBuffer(int l)
		{
			NumLiquidBuffer--;
			QuickLiquid.LiquidBuffer2[l].x = QuickLiquid.LiquidBuffer2[NumLiquidBuffer].x;
			QuickLiquid.LiquidBuffer2[l].y = QuickLiquid.LiquidBuffer2[NumLiquidBuffer].y;
		}
	}
}
