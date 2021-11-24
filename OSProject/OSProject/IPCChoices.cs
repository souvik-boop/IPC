using System.ComponentModel;

namespace OSProject
{
    public enum IPCChoices
    {
        [Description("Clipboard")]
        Clipboard,
        [Description("Component Object Model (COM)")]
        COM,
        [Description("Data Copy")]
        DataCopy,
        [Description("File Mapping")]
        FileMapping,
        [Description("Mail Slots")]
        MailSlots,
        [Description("Pipes")]
        Pipes,
        [Description("Remote Procedure Call (RPC)")]
        RPC,
        [Description("Windows Sockets")]
        WindowsSockets,
        [Description("Run it all!")]
        RunItAll
    }
}