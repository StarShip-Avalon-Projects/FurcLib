namespace Furcadia.Net.Options
{
    /// <summary>
    /// Configuration Settings for the Furcadia Client to connect to
    /// <para>
    /// We're pretending to be the Furcadia Game Server
    /// </para>
    /// </summary>
    public class ServerOptions
    {
        #region Private Fields

#pragma warning disable CS0169 // The field 'ServerOptions.localhost' is never used
        private string localhost;
#pragma warning restore CS0169 // The field 'ServerOptions.localhost' is never used
#pragma warning disable CS0169 // The field 'ServerOptions.LocalPort' is never used
        private int LocalPort;
#pragma warning restore CS0169 // The field 'ServerOptions.LocalPort' is never used

        #endregion Private Fields
    }
}