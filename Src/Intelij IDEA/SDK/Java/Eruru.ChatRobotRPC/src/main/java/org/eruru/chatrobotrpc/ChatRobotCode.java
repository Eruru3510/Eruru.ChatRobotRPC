package org.eruru.chatrobotrpc;

public class ChatRobotCode {

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

	public static String QQ () {
		return "[QQ]";
	}

	public static String split () {
		return "[next]";
	}

	public static String newLine () {
		return "[换行]";
	}

	public static String bubble (int id) {
		return String.format ("[气泡%d]", id);
	}

	public static String group () {
		return "[gname]";
	}

	public static String timeInterval () {
		return "[TimePer]";
	}

	public static String random () {
		return "[r]";
	}

	public static String picture (String pathOrURL) {
		return String.format ("[pic=%s]", pathOrURL);
	}

	public static String font (int color, int size) {
		return String.format ("字体[颜色=%d,大小=%d]", color, size);
	}

}