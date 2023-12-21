public class GGTest
{
	private static string stagesDBNameString = "StagesDBName";

	private static string settingsDBNameString = "SettingsDBName";

	private static string match3ParticlesVariantString = "Match3ParticlesVariant";

	private static string adaptiveShowMatchString = "adaptiveShowMatch";

	public static string stagesDBName => GGAB.GetString(stagesDBNameString, null);

	public static string settingsDBName => GGAB.GetString(settingsDBNameString, null);

	public static int match3ParticlesVariant => GGAB.GetInt(match3ParticlesVariantString, 0);

	public static bool showAdaptiveShowMatch => GGAB.GetBool(adaptiveShowMatchString, defaultValue: false);
}
