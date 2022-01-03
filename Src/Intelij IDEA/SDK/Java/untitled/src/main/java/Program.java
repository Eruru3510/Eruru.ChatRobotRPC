import org.eruru.chatrobotrpc.ChatRobot;
import org.junit.Test;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;
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
			chatRobot.connect ("localhost", 19730, "root", "root");
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