package org.eruru.chatrobotrpc;

/**
 * 聊天机器人码
 */
public class ChatRobotCode {

	/**
	 * 用于消息中表示当前对象QQ（[QQ]）
	 */
	public static final String qq = "[QQ]";

	/**
	 * 如:早上好[next]吃过早饭了吗？ 将会分成两条信息分别发送 每条信息最多允许使用10个分句标识
	 */
	public static final String split = "[next]";

	/**
	 * 表示换行,也可直接使用\r\n或\n（[换行]）
	 */
	public static final String newLine = "[换行]";

	/**
	 * 用于群消息中表示当前群名（[gname]）
	 */
	public static final String group = "[gname]";

	/**
	 * 表示 上午/下午/中午（[TimePer]）
	 */
	public static final String timeInterval = "[TimePer]";

	/**
	 * 表示一个0-100的随机数（[r]）
	 */
	public static final String random = "[r]";

	/**
	 * 群内@人（[@1633756198]）
	 *
	 * @param qq       -1为全体
	 * @param hasSpace 是否带空格
	 */
	public static String at (long qq, boolean hasSpace) {
		StringBuilder stringBuilder = new StringBuilder ();
		stringBuilder.append ("[@");
		if (qq > -1) {
			stringBuilder.append (qq);
		} else {
			stringBuilder.append ("all");
		}
		stringBuilder.append (']');
		if (hasSpace) {
			stringBuilder.append (' ');
		}
		return stringBuilder.toString ();
	}

	/**
	 * 群内@人，默认带空格（[@1633756198]）
	 *
	 * @param qq -1为全体
	 */
	public static String at (long qq) {
		return at (qq, true);
	}

	/**
	 * 群内@全体（[@all]）
	 *
	 * @param hasSpace 是否带空格
	 */
	public static String atAll (boolean hasSpace) {
		return at (-1, hasSpace);
	}

	/**
	 * 群内@全体，默认带空格（[@all]）
	 */
	public static String atAll () {
		return at (-1, false);
	}

	/**
	 * [emoji=F09F988A]
	 */
	public static String emoji (String id) {
		return String.format ("[emoji=%s]", id);
	}

	/**
	 * 表情（[Face21.gif]）
	 *
	 * @param id 0-170共计171个表情（2014年8月28日为止 将来腾讯方面还会继续添加）
	 */
	public static String Face (int id) {
		return String.format ("[Face%s.gif]", id);
	}

	/**
	 * 自定义本条信息气泡（[气泡10]）
	 */
	public static String bubble (int id) {
		return String.format ("[气泡%d]", id);
	}

	/**
	 * 发送图片（[pic=XXXXXXX]）
	 *
	 * @param pathOrURL 本地绝对路径或网络直链路径
	 */
	public static String picture (String pathOrURL) {
		return String.format ("[pic=%s]", pathOrURL);
	}

	/**
	 * 自定义单条信息字体（[字体[颜色=10,大小=14]]）
	 *
	 * @param color 字体颜色
	 * @param size  字体大小
	 */
	public static String font (int color, int size) {
		return String.format ("字体[颜色=%d,大小=%d]", color, size);
	}

}