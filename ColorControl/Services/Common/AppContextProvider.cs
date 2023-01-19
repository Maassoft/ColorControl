namespace ColorControl.Services.Common
{
    public class AppContextProvider
    {
        public AppContext GetAppContext()
        {
            return Program.AppContext;
        }

    }
}
