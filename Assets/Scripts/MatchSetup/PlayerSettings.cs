public class PlayerSetting
{
    private static PlayerSetting current;

    public static PlayerSetting Current
    {
        get
        {
            if (current == null)
                current = new PlayerSetting();

            return current;
        }

        set
        {
            current = value;
        }
    }

    public int startingWeapon;
}