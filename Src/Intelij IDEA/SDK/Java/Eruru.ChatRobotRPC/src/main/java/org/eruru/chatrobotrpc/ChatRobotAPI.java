package org.eruru.chatrobotrpc;

import javafx.util.Pair;

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class ChatRobotAPI {

	/// <summary>
	/// 是否为语音消息
	/// </summary>
	/// <param name="message">消息文本</param>
	/// <param name="guid">提取出来的语音GUID</param>
	/// <param name="identifyResult">提取出来的语音识别结果</param>
	/// <returns></returns>
	public static ChatRobotVoiceMessageResult isVoiceMessage (String message) {
		Matcher matcher = Pattern.compile ("(\\[Voi=\\{.+?}.+?])(\\[识别结果:(.+?)])?").matcher (message);
		if (matcher.find ()) {
			return new ChatRobotVoiceMessageResult (matcher.group (1), matcher.group (2));
		}
		return new ChatRobotVoiceMessageResult ();
	}

	/// <summary>
	/// 消息中是否包含图片
	/// </summary>
	/// <param name="message">消息文本</param>
	/// <param name="guids">提取出来的图片GUID</param>
	/// <returns></returns>
	public static Pair<Boolean, List<String>> containsPictureInMessage (String message) {
		Matcher matcher = Pattern.compile ("\\[pic=\\{.+?}.+?]").matcher (message);
		if (!matcher.find (0)) {
			return new Pair<> (false, null);
		}
		int offset;
		List<String> guids = new ArrayList<> ();
		do {
			guids.add (matcher.group (0));
			offset = matcher.end ();
		} while (matcher.find (offset));
		return new Pair<> (true, guids);
	}

	/// <summary>
	/// 是否为闪照消息
	/// </summary>
	/// <param name="message">消息文本</param>
	/// <returns></returns>
	public static boolean isFlashPictureMessage (String message) {
		return Pattern.matches ("\\[FlashPic=\\{.+?}.+?]", message);
	}

	/// <summary>
	/// 将闪照转为图片
	/// </summary>
	/// <param name="flashPictureGUID"></param>
	/// <returns></returns>
	public static String flashPictureToPicture (String flashPictureGUID) {
		return flashPictureGUID.replace ("FlashPic", "pic");
	}

	static <T> T enumParse (Class<T> type, String search) {
		for (T item : type.getEnumConstants ()) {
			if (item.toString ().compareToIgnoreCase (search) == 0) {
				return item;
			}
		}
		return null;
	}

	static <T> T enumGet (Class<T> type, int value) {
		try {
			Method method = type.getDeclaredMethod ("getValue");
			for (T item : type.getEnumConstants ()) {
				if ((int) method.invoke (item) == value) {
					return item;
				}
			}
		} catch (Exception e) {
			e.printStackTrace ();
		}
		return null;
	}

	static byte[] intToBytes (int i) {
		byte[] result = new byte[4];
		result[3] = (byte) ((i >>> 24) & 0xFF);
		result[2] = (byte) ((i >>> 16) & 0xFF);
		result[1] = (byte) ((i >>> 8) & 0xFF);
		result[0] = (byte) (i & 0xFF);
		return result;
	}

	static int bytesToInt (byte[] bytes) {
		int value = 0;
		for (int i = 0; i < 4; i++) {
			int shift = i * 8;
			value += (bytes[i] & 0xFF) << shift;
		}
		return value;
	}

}