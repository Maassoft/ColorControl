namespace ColorControl.Shared.Services
{
    public class AppContextProvider
    {
        private Common.GlobalContext _appContext;

        public void SetAppContext(Common.GlobalContext appContext)
        {
            _appContext = appContext;
        }

        public Common.GlobalContext GetAppContext()
        {
            return _appContext;
        }

    }
}
