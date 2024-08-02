using System.Collections.Generic;

namespace ColorControl.Services.Common;

public interface IServiceBase
{
    List<string> GetInfo();
    void InstallEventHandlers();
}
