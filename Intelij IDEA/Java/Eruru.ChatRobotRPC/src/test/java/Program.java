import org.eruru.chatrobotrpc.ChatRobotAPI;
import org.eruru.chatrobotrpc.ChatRobotCode;
import org.eruru.chatrobotrpc.ChatRobotMessage;
import org.junit.Test;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

public class Program {

	private final List<ChatRobotMessage> messages = new ArrayList<> ();

	@Test
	public void main () throws IOException, InterruptedException {
		ChatRobotAPI.setOnReceived (this::onReceived);
		ChatRobotAPI.setOnSent (this::onSent);
		ChatRobotAPI.setOnReceivedMessage (this::onReceivedMessage);
		ChatRobotAPI.setOnGroupMessageRevoke (this::onGroupMessageRevoke);
		ChatRobotAPI.setOnDisconnected (this::onDisconnected);
		System.out.println ("开始连接服务器");
		ChatRobotAPI.connect ("localhost", 19730, "root", "root");
		System.out.println ("连接服务器成功");
		while (true) {
			Thread.sleep (1);
		}
	}

	private void onReceived (String text) {
		System.out.println (String.format ("收到消息：%s", text));
	}

	private void onSent (String text) {
		System.out.println (String.format ("发送消息：%s", text));
	}

	private void onGroupMessageRevoke (long robotQQ, long group, long qq, long messageNumber, long messageID) {
		Calendar calendar = Calendar.getInstance ();
		calendar.add (Calendar.MINUTE, -3);
		Date expiry = calendar.getTime ();
		SimpleDateFormat simpleDateFormat = new SimpleDateFormat ("yyyy-MM-dd HH:mm:ss");
		for (int i = 0; i < messages.size (); i++) {
			if (messages.get (i).getNumber () == messageNumber && messages.get (i).getId () == messageID) {
				ChatRobotAPI.sendGroupMessage (robotQQ, group, String.format (
						"%s撤回了%s的消息：%s",
						ChatRobotCode.at (qq),
						simpleDateFormat.format (messages.get (i).getDate ()),
						messages.get (i).getText ()
				));
				messages.remove (i);
				break;
			}
			if (messages.get (i).getDate ().compareTo (expiry) < 0) {
				messages.remove (i--);
			}
		}
	}

	private void onReceivedMessage (ChatRobotMessage message) {
		try {
			messages.add (message);
			if (message.getText ().startsWith ("复读")) {
				message.Reply (message.getText ().substring ("复读".length ()));
				return;
			}
			switch (message.getText ()) {
				case "一言":
					message.Reply (httpGet ("http://api.sdtro.cn/API/yiy/yiy.php"));
					break;
				case "来首歌":
					message.replyJson (httpGet ("http://api.qfyu.top/API/wysj.php").substring ("json:".length ()));
					break;
				case "群名":
					message.Reply (ChatRobotAPI.getGroupName (message.getRobotQQ (), message.getGroup ()));
					break;
			}
		} catch (Exception exception) {
			exception.printStackTrace ();
		}
	}

	private void onDisconnected () {
		System.out.println ("连接已断开");
	}

	private String httpGet (String url) throws IOException {
		HttpURLConnection httpURLConnection = (HttpURLConnection) new URL (url).openConnection ();
		httpURLConnection.connect ();
		try (InputStream inputStream = httpURLConnection.getInputStream ()) {
			try (ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream ()) {
				byte[] buffer = new byte[1024];
				while (true) {
					int length = inputStream.read (buffer, 0, buffer.length);
					if (length == -1) {
						break;
					}
					byteArrayOutputStream.write (buffer, 0, length);
				}
				httpURLConnection.disconnect ();
				return byteArrayOutputStream.toString ("utf-8");
			}
		}
	}

}