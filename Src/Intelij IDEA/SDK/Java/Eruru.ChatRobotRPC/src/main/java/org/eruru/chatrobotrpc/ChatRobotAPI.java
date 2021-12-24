package org.eruru.chatrobotrpc;

import java.lang.reflect.Method;

public class ChatRobotAPI {

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