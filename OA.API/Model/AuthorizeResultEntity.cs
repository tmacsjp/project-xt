using OA.API.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.API.Models
{
    public class AuthorizeResultEntity
    {
        public AuthorizeType AuthorizeType { get; set; } = AuthorizeType.API_User;
        public Dictionary<AuthorizeType, AuthorizeResultItem> Results { get; set; } = new Dictionary<AuthorizeType, AuthorizeResultItem>();
        public RequestAuthorizeContext Context { get; private set; } = new RequestAuthorizeContext();

        /// <summary>
        /// 请求时间
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// 应用
        /// </summary>
        public string AppKey { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string Sign { get; set; }
        /// <summary>
        /// 授权token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public string User { get; set; }

        public bool IsAuthrized { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class RequestAuthorizeContext
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string HeadUrl { get; set; }
        public string AppKey { get; set; }
        public string Token { get; set; }
        public string ClientIP { get; set; }
    }

    //每一个授权的实体值
    public class AuthorizeResultItem
    {
        public AuthorizeType AuthType { get; private set; }
        public bool IsPass { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
        public Dictionary<string, object> Items { get; set; } = new Dictionary<string, object>();
        public object GetItem(string key)
        {
            if (Items.ContainsKey(key))
                return Items[key];
            return null;
        }
        public T GetItem<T>(string key)
        {
            if (Items.ContainsKey(key))
                return (T)Items[key];
            return default(T);
        }
        public AuthorizeResultItem()
        {
        }
        public AuthorizeResultItem(AuthorizeType authType)
        {
            AuthType = authType;
        }
        public static readonly AuthorizeResultItem Pass = new AuthorizeResultItem() { IsPass = true };
    }
}
