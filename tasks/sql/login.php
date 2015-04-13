<?php
	/**/if (isset($_POST["login"])) {
		session_start();
		session_unset();
		$_SESSION["user"] = "u5296";
		header("Location: index.php");
		exit();
	}/**/
?><!DOCTYPE html>
<html>
<head>
	<meta charset="utf-8" />
	<meta http-equiv="Cache-Control" content="no-cache"/>
	<title>31337 Investments</title>
</head>
<body>
<div class="login">
	<?php
		if (isset($_POST["login"])) {
			echo "Неверный логин или пароль!<br>";
		}
	?>
	<form method="POST">
		Ваше имя <input type="text" name="user"><br>
		Пароль <input type="password" name="pass"><br>
		<input type="submit" name="login" value="Логин">
	</form>
</div>
</body>
</html>
