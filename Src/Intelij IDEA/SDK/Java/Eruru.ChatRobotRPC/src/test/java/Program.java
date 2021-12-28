import com.alibaba.fastjson.JSON;
import javafx.util.Pair;
import org.eruru.chatrobotrpc.ChatRobot;
import org.eruru.chatrobotrpc.ChatRobotAPI;
import org.eruru.chatrobotrpc.ChatRobotVoiceMessageResult;
import org.junit.Test;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.List;
import java.util.Scanner;

public class Program {

	private static final ChatRobot chatRobot = new ChatRobot ();

	@Test
	public void main () {
		chatRobot.setOnReceived (message -> System.out.printf ("收到消息：%s%n", message));
		chatRobot.setOnSend (message -> System.out.printf ("发送消息：%s%n", message));
		chatRobot.setOnReceivedMessage (message -> {
			String text = message.getText ();
			switch (message.getType ()) {
				case friend:
					break;
				case group:
					try {
						if (Files.readAllLines (Paths.get ("D:/Group.txt")).contains (Long.toString (message.getGroup ()))) {
							break;
						}
					} catch (IOException e) {
						e.printStackTrace ();
					}
					return;
				default:
					return;
			}
			if (text.startsWith ("测试")) {

			}
			if (message.isFlashPicture ()) {
				String replyText = chatRobot.groupPictureToFriendPicture (message.getRobot (), ChatRobotAPI.flashPictureToPicture (text));
				chatRobot.sendFriendMessage (message.getRobot (), 1633756198, replyText);
			}
			Pair<Boolean, List<String>> result = message.containsPicture ();
			System.out.println (JSON.toJSONString (result, true));
			if (result.getKey ()) {
				for (int i = 0; i < result.getValue ().size (); i++) {
					result.getValue ().set (i, chatRobot.groupPictureToFriendPicture (message.getRobot (), result.getValue ().get (i)));
				}
				String replyText = String.join ("\n", result.getValue ());
				chatRobot.sendFriendMessage (message.getRobot (), 1633756198, replyText);
			}
			ChatRobotVoiceMessageResult voiceResult = message.isVoice ();
			if (voiceResult.isSuccess ()) {
				chatRobot.sendFriendMessage (message.getRobot (), 1633756198, String.format ("语音识别结果：%s", voiceResult.getIdentifyResult ()));
			}
		});
		chatRobot.setOnDisconnected (() -> {
			System.out.println ("连接断开");
			connect ();
		});
		connect ();
		Scanner scanner = new Scanner (System.in);
		scanner.nextLine ();
	}

	static void connect () {
		try {
			System.out.println ("开始连接");
			chatRobot.connect (Files.readAllLines (Paths.get ("D:/IP.txt")).get (0), 19730, "root", "root");
			System.out.println ("连接成功");
		} catch (IOException ioException) {
			System.out.println ("连接失败");
			ioException.printStackTrace ();
			connect ();
		} catch (Exception exception) {
			exception.printStackTrace ();
		}
	}

}