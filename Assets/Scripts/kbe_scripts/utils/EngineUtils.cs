using UnityEngine;
using System.Collections;
using System.Threading;
namespace KBEngine
{
    public class EngineUtils
    {
        #region 服务器允许调用的方法

        #region LoginApp
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="datas"></param>
        public static void Login(string account, string password, byte[] datas)
        {
            KBEngine.Event.fireIn("login", account, password, datas);
        }
        /// <summary>
        /// 创建一个账户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="datas"></param>
        public static void CreateAccount(string userName, string password, byte[] datas)
        {
            KBEngine.Event.fireIn("createAccount", userName, password, datas);
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="userName"></param>
        public static void ResetPassword(string userName)
        {
            KBEngine.Event.fireIn("resetPassword", userName);
        }

        #region BaseApp

        /// <summary>
        /// 重登陆baseapp
        /// </summary>
        public static void ReloginBaseapp()
        {
            KBEngine.Event.fireIn("reloginBaseapp");
        }
        public static void BindAccountEmail(string emailAddress)
        {
            KBEngine.Event.fireIn("bindAccountEmail", emailAddress);
        }
        public static void ChangePassword(string oldPassword, string newPassword)
        {
            KBEngine.Event.fireIn("newPassword", oldPassword, newPassword);
               
        }

		#endregion

		#endregion

		#endregion

		#region 辅助工具

		public static bool EnsureComponent<T>(Entity entity, ref T component) where T : Component
		{
			if (entity.renderObj == null)
			{
				component = null;
				return false;
			}
			GameObject go = entity.renderObj as GameObject;
			if (go == null)
			{
				component = null;
				return false;
			}
			component = go.GetComponent<T>();
			return component != null;
		}
		public static T EnsureComponent<T>(Entity entity) where T : Component
		{
			if (entity.renderObj == null)
				return null;
			GameObject go = entity.renderObj as GameObject;
			if (go == null)
				return null;
			return go.GetComponent<T>();
		}
		/// <summary>
		/// 引擎的Direction转换成unity的direction
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		public static Vector3 EngineDirectionToUnity(Vector3 direction)
		{
			return new Vector3(direction.y, direction.z, direction.x);
		}

		#endregion
	}

}
