using Furcadia.IO;
using Furcadia.Movement;
using Furcadia.Net.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furcadia.Net.DirectConnection
{
    /// <summary>
    /// Read settings from Ini files
    /// </summary>
    public class IniParser
    {
        private IniFile LoginIni;

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public ClientOptions Options { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniParser"/> class.
        /// </summary>
        public IniParser()
        {
            LoginIni = new IniFile();
            Options = new ClientOptions();
        }

        /// <summary>
        /// Loads the options from ini.
        /// </summary>
        /// <param name="IniFile">The ini file.</param>
        public ClientOptions LoadOptionsFromIni(string IniFile)
        {
            if (string.IsNullOrEmpty(IniFile))
                File.Create(IniFile);
            LoginIni.Load(IniFile);
            var MainSettings = LoginIni.GetSection("Main");
            var ConnectionSettings = LoginIni.GetSection("Connetion");
            Options = new ClientOptions()
            {
                // Version = MainSettings.GetKey("Version").Value,
                // Account = MainSettings.GetKey("Account").Value,
                CharacterName = MainSettings.GetKey("CharacterName").Value.TrimEnd('"'),
                Password = MainSettings.GetKey("Password").Value.TrimEnd('"'),
                Description = MainSettings.GetKey("Desc").Value.TrimEnd('"'),
                Colors = MainSettings.GetKey("Colors").Value.TrimEnd('"'),
                // Costume = MainSettings.GetKey("Costume").Value,
            };

            return Options;
        }
    }
}