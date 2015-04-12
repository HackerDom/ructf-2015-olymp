<?php
error_reporting(0);

define("sqlHost", "127.0.0.1");
define("sqlUser", "root");
define("sqlPass", "vertrigo");
define("sqlDbName", "sqli");
$sqlUsers = "hamsters";
$sqlTrans = "transactions";

function connect_db() {
	$sqlID = mysql_connect(sqlHost, sqlUser, sqlPass) or die('error 1: '.mysql_error());
	mysql_select_db(sqlDbName, $sqlID) or die('error 2: '.mysql_error());
}
function sanitize_html($data) {
	return htmlspecialchars(stripslashes(trim($data)));
}
function sanitize_mysql($data) {
	return mysql_real_escape_string($data);
}

if (!session_id()) session_start();
if (isset($_SESSION["login"])) {
	header("Location: index.php");
	exit();
}

connect_db();
if (isset($_POST["login"])) {
	$u = sanitize_mysql(sanitize_html($_POST["user"]));
	$p = md5($_POST["pass"]);
	$query = "SELECT * FROM `{$sqlUsers}` WHERE LOWER(`user`)=LOWER('{$u}') AND `pass`='{$p}'";
	$result = mysql_query($query);
	if ($result && mysql_num_rows($result)) {
		$_SESSION["user"] = $u;
		header("Location: index.php");
		die();
	} else {
		echo "Incorrect login or password";
	}
} elseif (isset($_POST["register"])) {	
	$u = sanitize_mysql(sanitize_html($_POST["user"]));
	if (!empty($_POST["user"])) {
		$p = md5($_POST["pass"]);
		$query = "INSERT INTO `{$sqlUsers}` (`user`, `pass`) VALUES ('{$u}', '{$p}')";
		if (mysql_query($query)) {
			$_SESSION["user"] = $u;
			mysql_query("INSERT INTO `{$sqlTrans}` (`user`, `value`, `percent`, `date`) VALUES ".
				"('{$u}', 1337, 0.0, NOW())") or die(mysql_error());
			header("Location: index.php");
			die();
		} else {
			echo mysql_error();
		}
	} else {
		echo "Login empty";
	}
}
?><!DOCTYPE html>
<html>
<head>
	<meta charset="utf-8" />
	<meta http-equiv="Cache-Control" content="no-cache"/>
	<title>31337 Investments</title>
</head>
<body>
<div class="login">
	<form method="POST">
	Ваше имя <input type="text" name="user"><br>
	Пароль <input type="password" name="pass">
	<input type="submit" name="login" value="Логин">
	</form>
</div>
<div class="register">
	<form method="POST">
	Ваше имя <input type="text" name="user"><br>
	Пароль <input type="password" name="pass">
	<input type="submit" name="register" value="Регистрация">
	</form>
</div>
</body>
</html>
