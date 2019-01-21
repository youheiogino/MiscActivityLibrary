using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Windows.Forms;
using System.ComponentModel;

namespace MiscActivityLibrary
{
    [Designer(typeof(ShowMessageBoxDesigner))]
    public sealed class ShowMessageBox : CodeActivity
    {
        [Category("Input")]
        public InArgument<string> Message { get; set; }

        // アクティビティが値を返す場合は、CodeActivity<TResult> から派生して、
        // Execute メソッドから値を返します。
        protected override void Execute(CodeActivityContext context)
        {
            string message = context.GetValue(this.Message);
            MessageBox.Show(message);
        }
    }
}
