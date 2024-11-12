using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Senjyouhara.Common.Utils;

/// <summary>
/// 类型和值
/// </summary>
class TypeAndValue
{
    /// <summary>
    /// 类型
    /// </summary>
    public Type type { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public object value { get; set; }
}

public class ObjectCopy
{
    /// <summary>
    /// 属性映射(静态对象，无需重复建立属性映射关系，提高效率)
    /// </summary>
    public static Dictionary<string, List<string>> MapDic = new Dictionary<string, List<string>>();

    /// <summary>
    /// S复制到D(创建对象D)
    /// </summary>
    /// <typeparam name="D">输出对象类型</typeparam>
    /// <typeparam name="S">输入对象类型</typeparam>
    /// <param name="s">输入对象</param>
    /// <returns></returns>
    public static D Copy<S, D>(S s) where D : class, new() where S : class, new()
    {
        if (s == null)
        {
            return default(D);
        }

        //使用无参数构造函数，创建指定泛型类型参数所指定类型的实例
        D d = Activator.CreateInstance<D>();
        return Copy<S, D>(s, d);
    }

    /// <summary>
    /// S复制到D(对象D已存在)
    /// </summary>
    /// <typeparam name="S">输入对象类型</typeparam>
    /// <typeparam name="D">输出对象类型</typeparam>
    /// <param name="s">输入对象</param>
    /// <param name="d">输出对象</param>
    /// <returns></returns>
    public static D Copy<S, D>(S s, D d)
        where D : class, new() where S : class, new()
    {
        return Copy<S, D>(s, d, null);
    }

    public static D Copy<S, D>(S s, D d, BindingFlags access)
        where D : class, new() where S : class, new()
    {
        return Copy<S, D>(s, d, null, access);
    }

    public static D Copy<S, D>(S s, D d, List<string>? ignoreField,
        BindingFlags access =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        where D : class, new() where S : class, new()
    {
        if (s == null || d == null)
        {
            return d;
        }

        try
        {
            var sType = s.GetType();
            var dType = typeof(D);
            //属性映射Key
            var mapkey = dType.FullName + "_" + sType.FullName;
            if (MapDic.ContainsKey(mapkey))
            {
                //已存在属性映射
                foreach (var item in MapDic[mapkey])
                {
                    //按照属性映射关系赋值
                    //.net 4
                    // dType.GetProperty(item).SetValue(d, sType.GetProperty(item).GetValue(s, null), null);
                    //.net 4.5
                    dType.GetProperty(item)?.SetValue(d, sType.GetProperty(item)?.GetValue(s));
                }
            }
            else
            {
                //不存在属性映射，需要建立属性映射
                List<string> namelist = new List<string>();
                Dictionary<string, TypeAndValue> dic = new Dictionary<string, TypeAndValue>();
                // var access = IsSetParent
                // 	? BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic 
                // 	: BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic  | BindingFlags.DeclaredOnly;
                //遍历获取输入类型的属性（属性名称，类型，值）
                foreach (PropertyInfo sP in sType.GetProperties(access))
                {
                    //.net 4
                    // dic.Add(sP.Name, new TypeAndValue() { type = sP.PropertyType, value = sP.GetValue(s, null) });
                    //.net 4.5
                    dic.Add(sP.Name, new TypeAndValue() { type = sP.PropertyType, value = sP.GetValue(s) });
                }

                //遍历输出类型的属性，并与输入类型（相同名称和类型的属性）建立映射，并赋值
                foreach (PropertyInfo dP in dType.GetProperties(access))
                {
                    if (dic.Keys.Contains(dP.Name))
                    {
                        if (dP.PropertyType == dic[dP.Name].type && dP.CanWrite)
                        {
                            if ((ignoreField?.Contains(dP.Name)).GetValueOrDefault())
                            {
                                continue;
                            }

                            namelist.Add(dP.Name);
                            //.net 4
                            // dP.SetValue(d, dic[dP.Name].value, null);
                            //.net 4.5
                            dP.SetValue(d, dic[dP.Name].value);
                        }
                    }
                }

                //保存映射
                if (!MapDic.ContainsKey(mapkey))
                {
                    MapDic.Add(mapkey, namelist);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Log.Error("error", ex);
        }

        return d;
    }

    /// <summary>
    /// SList复制到DList
    /// </summary>
    /// <typeparam name="D">输出对象类型</typeparam>
    /// <typeparam name="S">输入对象类型</typeparam>
    /// <param name="sList">输入对象集合</param>
    /// <returns></returns>
    public static IQueryable<D> Copy<S, D>(IQueryable<S> sList) where D : class, new() where S : class, new()
    {
        List<D> dList = new List<D>();
        foreach (var item in sList)
        {
            dList.Add(Copy<S, D>(item));
        }

        return dList.AsQueryable();
    }
}