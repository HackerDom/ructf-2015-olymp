<?php
	if (!session_id()) session_start();
	if (isset($_SESSION["user"])) {
		header("Location: index.php");
		exit();
	}
	/*if (isset($_POST["login"])) {
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
	
	<link rel="stylesheet" type="text/css" href="css/bootstrap.css">
	<link rel="stylesheet" type="text/css" href="css/dashboard.css">
</head>

<body>
<div class="container">
<div class="row">
	<div class="col-lg-offset-4 col-lg-4 col-md-offset-4 col-md-4 col-sm-8">
	<div class="panel panel-info">
		<div class="panel-heading">Вход</div>
		<div class="panel-body">
			<?php
				if (isset($_POST["login"])) {
					echo "<div style='margin-bottom: 5px' class='input-group'>".
						"Неверный логин или пароль!</div>";
				}
			?>
			<form method="post">
				<div style="margin-bottom: 5px" class="input-group">
					<input name="user" type="text" class="form-control" placeholder="Имя">
				</div>
				<div style="margin-bottom: 5px" class="input-group">
					<input name="pass" type="password" class="form-control" placeholder="Пароль">
				</div>
				<button type="submit" name="login" class="btn btn-primary btn-block">Войти</button>
			</form>
		</div>
	</div>
	</div>
</div>
</div>
</body>
</html>