using System;
using System.Linq;
using SapNwRfc.Internal.Fields;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal
{
    internal class StringTableField
    {
        public static TableField<string> Extract(RfcInterop interop, IntPtr dataHandle, string name)
        {
            TableField<StringTableElement> tableField = TableField<StringTableElement>.Extract<StringTableElement>(interop, dataHandle, name);
            return new TableField<string>(name, tableField.Value.Select(v => v.Data).ToArray());
        }
    }
}
