public static class UserData
{
    private static bool _isSet;

    private static PlayerModel _modelData;
    
    public static PlayerModel FromModel
    {
        get { return _modelData; }
        set
        {
            if (_isSet) return;
            _isSet = true;
            _modelData = value;
        }
    }
}
