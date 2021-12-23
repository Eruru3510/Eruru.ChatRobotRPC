package org.eruru.chatrobotrpc;

public class ChatRobotAPI {

	static <T extends Enum<?>> T enumParse (Class<T> enumeration, String search) {
		for (T each : enumeration.getEnumConstants ()) {
			if (each.name ().compareToIgnoreCase (search) == 0) {
				return each;
			}
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