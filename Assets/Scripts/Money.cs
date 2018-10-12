public class Money
{
    // The player's current money value
    private static int _currentMoney = 20;

    // Fixed money investment/return constants
    public const int SEED_BUY_PRICE = 5;
    public const int CROP_SELL_PRICE = 20;

    // Set the current money value
    public static void Set( int value )
    {
        _currentMoney = value;
    }

    // Add to the current money value (or subtract by using negative values)
    public static void Add( int value )
    {
        _currentMoney += value;
    }
    
    // Get the current money value
    public static int Get()
    {
        return _currentMoney;
    }
}
