using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Security.CustomTokenProvider
{
    /// <summary>
    /// 自定义邮箱确认令牌有效期
    /// </summary>
    public class CustomEmailConfirmationTokenProviderOptions: DataProtectionTokenProviderOptions
    {

    }
}
