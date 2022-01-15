import org.eruru.chatrobotrpc.ChatRobot;
import org.junit.Test;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.Scanner;
import java.util.concurrent.TimeoutException;

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
				message.reply (message.getText ());
			}
		});
		chatRobot.setOnDisconnected (() -> {
			System.out.println ("连接断开");
			connect ();
		});
		connect ();
		new Scanner (System.in).nextLine ();
	}

	static void connect () {
		try {
			System.out.println ("开始连接");
			chatRobot.connect ("localhost", 19730, "root", "root");
			System.out.println ("连接成功");
		} catch (IOException ioException) {
			ioException.printStackTrace ();
			connect ();
		} catch (TimeoutException timeoutException) {
			System.out.println ("连接成功，但是响应登录请求超时");
			timeoutException.printStackTrace ();
			connect ();
		} catch (Exception exception) {
			exception.printStackTrace ();
		}
	}

}