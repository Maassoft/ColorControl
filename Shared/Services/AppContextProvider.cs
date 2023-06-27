namespace ColorControl.Shared.Services
{
    public class AppContextProvider
    {
        private Common.AppContext _appContext;

        public void SetAppContext(Common.AppContext appContext)
        {
            _appContext = appContext;
        }

        public Common.AppContext GetAppContext()
        {
            return _appContext;
        }

    }
}
