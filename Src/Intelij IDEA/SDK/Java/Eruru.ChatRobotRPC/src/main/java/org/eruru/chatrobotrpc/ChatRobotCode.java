package org.eruru.chatrobotrpc;

public class ChatRobotCode {

	public static final String qq = "[QQ]";

	public static final String split = "[next]";

	public static final String newLine = "[换行]";

	public static final String group = "[gname]";

	public static final String timeInterval = "[TimePer]";

	public static final String random = "[r]";

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

	public static String at (long qq) {
		return at (qq, true);
	}

	public static String atAll (boolean hasSpace) {
		return at (-1, hasSpace);
	}

	public static String atAll () {
		return at (-1, false);
	}

	public static String emoji (String id) {
		return String.format ("[emoji=%s]", id);
	}

	public static String Face (int id) {
		return String.format ("[Face%s.gif]", id);
	}

	public static String bubble (int id) {
		return String.format ("[气泡%d]", id);
	}

	public static String picture (String pathOrURL) {
		return String.format ("[pic=%s]", pathOrURL);
	}

	public static String font (int color, int size) {
		return String.format ("字体[颜色=%d,大小=%d]", color, size);
	}

}