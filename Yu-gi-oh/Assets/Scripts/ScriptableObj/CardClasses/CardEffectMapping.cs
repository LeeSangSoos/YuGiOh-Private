using System.Collections.Generic;

public static class CardEffectMapping
{
	public static Dictionary<string, IEffect> EffectMap = new Dictionary<string, IEffect>
		{
			{"Default", new DefaultEffect() },
			{ "TrapHole", new TrapHole() },
			{ "DarkHole", new DarkHole() },
			{"Fissure", new Fissure() },
		{"BeastFangs", new BeastFangs() },
		{"BookOfSecretArts", new BookOfSecretArts() },
		{"LegendarySword", new LegendarySword() },
		{"PowerOfKaishin", new PowerOfKaishin() },
		{"VioletCrystal", new VioletCrystal() }
		};
}
