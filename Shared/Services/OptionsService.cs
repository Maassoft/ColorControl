using ColorControl.Shared.Contracts;

namespace ColorControl.Shared.Services;

public class OptionsService
{
    //private readonly GlobalContext _globalContext;

    private readonly Config _config;

    public OptionsService()
    {
        //_globalContext = globalContext;

        _config = new Config();
    }

    public Config GetConfig()
    {
        return new Config(_config);
    }

    public void SetConfig(Config config)
    {
        _config.Update(config);
    }
}