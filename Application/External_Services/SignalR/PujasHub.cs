using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Application.External_Services.SignalR
{
    public class PujasHub : Hub
    {
        public async Task UnirseASubasta(string subastaId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, subastaId);
        }

        public async Task SalirDeSubasta(string subastaId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, subastaId);
        }
    }


}
