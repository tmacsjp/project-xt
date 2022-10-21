using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.API.Enum
{
    /// <summary>
    /// 授权验证方式
    /// </summary>
    [Flags]
    public enum AuthorizeType : int
    {
        None = 0,
        /// <summary>
        /// 请求时间
        /// </summary>
        Timestamp = 1,
        /// <summary>
        /// 应用 签名
        /// </summary>
        AppKey = 2,
        /// <summary>
        /// 授权token
        /// </summary>
        Token = 4,
        /// <summary>
        /// 用户
        /// </summary>
        User = 8,
        /// <summary>
        /// 外部API用户
        /// </summary>
        API_NoUser = 1 | 2,
        /// <summary>
        /// 外部API用户
        /// </summary>
        API_User = 1 | 2 | 4 | 8
    }
}
