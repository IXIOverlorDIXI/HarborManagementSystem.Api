using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IMqttHandler
{
    public Task<KeyValuePair<bool, string>> MqttAddHandle(string payload);
}