using System;
using System.Runtime.InteropServices;

namespace SampleCOMObjectNameSpace
{
    #region REGISTER COM OBJECT
    [Guid("694C1820-04B6-4988-928F-FD858B95C880")]
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface SampleCOMObject_Interface
    {
        [DispId(1)]
        void SetData(string data);
        [DispId(2)]
        string GetData();
    }

    [ComVisible(true)]
    [Guid("9E5E5FB2-219D-4ee7-AB27-E4DBED8E123E")]
    public class SampleCOMObject : SampleCOMObject_Interface
    {
        private string data;
        public string GetData()
        {
            return this.data;
        }

        public void SetData(string data)
        {
            this.data = data;
        }
    }
    #endregion
}