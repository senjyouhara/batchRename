using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Common.Utils
{
    public class Util
    {
        public static T DeepCopyByReflect<T>(T obj)
        {
            //如果是字符串或值类型则直接返回
            if (obj == null || (obj is string) || (obj.GetType().IsValueType)) return obj;

            object objCopy = null;

            MemoryStream stream = new MemoryStream();
            BinaryFormatter binFormatter = new BinaryFormatter();
            binFormatter.Serialize(stream, obj);
            stream.Position = 0;
            objCopy = (T)binFormatter.Deserialize(stream);
            stream.Close();
            return (T)objCopy;
        }

    }
}
