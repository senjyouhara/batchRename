using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Common.Exceptions
{
    public class BusinessRunTimeException : Exception
    {
        private string error;
        private StackTrace st;

        public override string StackTrace => st.ToString();


        //无参数构造函数
        public BusinessRunTimeException()
        {

        }
        //带一个字符串参数的构造函数
        public BusinessRunTimeException(string msg): base(msg)
        {
            error = msg;
            st = new StackTrace();
        }

        //带有一个字符串参数和一个内部异常信息参数的构造函数
        public string GetError()
        {
            return error;
        }

    }
}
