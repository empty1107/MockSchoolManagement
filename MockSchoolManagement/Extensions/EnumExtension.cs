using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MockSchoolManagement.Extensions
{
    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public static class EnumExtension
    {
        public static string GetDisplayName(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] infos = type.GetMember(en.ToString());
            if (infos!=null && infos.Length > 0)
            {
                object[] attrs = infos[0].GetCustomAttributes(typeof(DisplayAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DisplayAttribute)attrs[0]).Name;
                }
            }
            return en.ToString();
        }
    }
}
