#region << 版本注释-v1 >>
/*
 * ========================================================================
 * 版本：v1
 * 时间：2020-04-08
 * 作者：lith
 * 邮箱：serset@yeah.net
 * 说明： 
 * ========================================================================
*/
#endregion

using System;

namespace Vit.ConsoleUtil
{
    #region CommandAttribute
    /// <summary>
    /// 命令名称，若不指定内容则使用函数名
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CommandAttribute : System.Attribute
    {
        public string Value { get; set; }

        public CommandAttribute(string Value = null)
        {
            this.Value = Value;
        }
    }
    #endregion

    #region RemarksAttribute
    /// <summary>
    /// 命令说明，用以提示用户
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class RemarksAttribute : System.Attribute
    {
        public string Value { get; set; }

        public RemarksAttribute(string Value)
        {
            this.Value = Value;
        }
    }
    #endregion
}
