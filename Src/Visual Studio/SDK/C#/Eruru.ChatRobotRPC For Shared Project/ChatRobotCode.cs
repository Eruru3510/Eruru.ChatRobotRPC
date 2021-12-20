using System.Text;

namespace Eruru.ChatRobotRPC {

	public class ChatRobotCode {

		/// <summary>
		/// 群内@人（[@1633756198]）
		/// </summary>
		/// <param name="qq">-1为全体</param>
		/// <param name="hasSpace">是否带空格</param>
		/// <returns></returns>
		public static string At (long qq, bool hasSpace = true) {
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.Append ("[@");
			if (qq > -1) {
				stringBuilder.Append (qq);
			} else {
				stringBuilder.Append ("all");
			}
			stringBuilder.Append (']');
			if (hasSpace) {
				stringBuilder.Append (' ');
			}
			return stringBuilder.ToString ();
		}

		/// <summary>
		/// 群内@全体（[@all]）
		/// </summary>
		/// <param name="hasSpace">是否带空格</param>
		/// <returns></returns>
		public static string AtALL (bool hasSpace = true) {
			return At (-1, hasSpace);
		}

		/// <summary>
		/// [emoji=F09F988A]
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static string Emoji (string id) {
			return $"[emoji={id}]";
		}

		/// <summary>
		/// 表情（[Face21.gif]）
		/// </summary>
		/// <param name="id">0-170共计171个表情（2014年8月28日为止 将来腾讯方面还会继续添加）</param>
		/// <returns></returns>
		public static string Face (int id) {
			return $"[Face{id}.gif]";
		}

		/// <summary>
		/// 用于消息中表示当前对象QQ（[QQ]）
		/// </summary>
		/// <returns></returns>
		public static string QQ () {
			return "[QQ]";
		}

		/// <summary>
		/// 如:早上好[next]吃过早饭了吗？ 将会分成两条信息分别发送 每条信息最多允许使用10个分句标识
		/// </summary>
		/// <returns></returns>
		public static string Split () {
			return "[next]";
		}

		/// <summary>
		/// 表示换行,也可直接使用\r\n或\n（[换行]）
		/// </summary>
		/// <returns></returns>
		public static string NewLine () {
			return "[换行]";
		}

		/// <summary>
		/// 自定义本条信息气泡（[气泡10]）
		/// </summary>
		/// <returns></returns>
		public static string Bubble (int id) {
			return $"[气泡{id}]";
		}

		/// <summary>
		/// 用于群消息中表示当前群名（[gname]）
		/// </summary>
		/// <returns></returns>
		public static string Group () {
			return "[gname]";
		}

		/// <summary>
		/// 表示 上午/下午/中午（[TimePer]）
		/// </summary>
		/// <returns></returns>
		public static string TimeInterval () {
			return "[TimePer]";
		}

		/// <summary>
		/// 表示一个0-100的随机数（[r]）
		/// </summary>
		/// <returns></returns>
		public static string Random () {
			return "[r]";
		}

		/// <summary>
		/// 发送图片（[pic=XXXXXXX]）
		/// </summary>
		/// <param name="pathOrURL">本地绝对路径或网络直链路径</param>
		/// <returns></returns>
		public static string Picture (string pathOrURL) {
			return $"[pic={pathOrURL}]";
		}

		/// <summary>
		/// 自定义单条信息字体（[字体[颜色=10,大小=14]]）
		/// </summary>
		/// <param name="color">字体颜色</param>
		/// <param name="size">字体大小</param>
		/// <returns></returns>
		public static string Font (int color, int size) {
			return $"字体[颜色={color},大小={size}]";
		}

	}

}