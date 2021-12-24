import org.eruru.chatrobotrpc.ChatRobot;
import org.eruru.chatrobotrpc.enums.ChatRobotMessageType;
import org.junit.Test;

import java.io.IOException;
import java.util.Scanner;

public class Example {

	private static final ChatRobot chatRobot = new ChatRobot ();

	@Test
	public void main () {
		chatRobot.setOnReceivedMessage (message -> {//当收到消息
			if (message.getType () == ChatRobotMessageType.friend) {//如果是好友消息
				message.reply ("%s发送了：%s", chatRobot.getName (message.getRobot (), message.getQQ ()), message);//复读
			}
		});
		chatRobot.setOnDisconnected (() -> {//当与机器人框架RPC插件断开了连接
			System.out.println ("连接断开");
			connect ();//重连
		});
		connect ();//连接机器人框架RPC插件
		Scanner scanner = new Scanner (System.in);
		scanner.nextLine ();//保持运行
	}

	static void connect () {
		try {
			System.out.println ("开始连接");
			chatRobot.connect ("127.0.0.1", 19730, "root", "root");
			System.out.println ("连接成功");
		} catch (IOException ioException) {
			System.out.println ("连接失败");
			ioException.printStackTrace ();
			connect ();
		}
	}

}