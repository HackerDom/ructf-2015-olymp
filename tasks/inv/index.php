<?php
error_reporting(0);

define("sqlHost", "127.0.0.1");
define("sqlUser", "ructf");
define("sqlPass", "ctj2710__LLKSAET");
define("sqlDbName", "sqli");
$sqlUsers = "hamsters";
$sqlTrans = "transact";

function connect_db() {
	$sqlID = mysql_connect(sqlHost, sqlUser, sqlPass);
	mysql_select_db(sqlDbName, $sqlID);
}

if (!session_id()) session_start();
if (!isset($_SESSION["user"])) {
	session_unset();
	header("Location: login.php");
	exit();
}

connect_db();
$query = "SELECT SUM(value) as balance FROM `{$sqlTrans}` WHERE `user`='{$_SESSION['user']}'";
$result = mysql_query($query);
$balance = mysql_fetch_object($result)->balance;

function sanitize_date($data) {
	//return "STR_TO_DATE('{$data}', '%Y-%m-%d %H:%i')";
	return "'".$data."'";
}
$between = "";
if (isset($_POST["history"])) {
	$from = sanitize_date($_POST["from"]);
	$to = sanitize_date($_POST["to"]);
	$between = " AND `date` BETWEEN {$from} AND {$to}";
}

$filter = " WHERE `user`='{$_SESSION["user"]}'".$between;
$query = "SELECT SUM(value) as yield FROM `{$sqlTrans}`{$filter}";
$result = mysql_query($query) or die();
//var_dump(mysql_num_rows($result));
$yield = (int) mysql_fetch_object($result)->yield;

$trans = array();
$query = "SELECT * FROM `{$sqlTrans}`{$filter} ORDER BY `date` DESC";
$result = mysql_query($query) or die();
//var_dump(mysql_num_rows($result));
while ($row = mysql_fetch_assoc($result)) {
	$trans[] = $row;
}
?><!DOCTYPE html>
<html>
<head>
	<meta charset="utf-8" />
	<meta http-equiv="Cache-Control" content="no-cache"/>
	<title>31337 Investments</title>
	
	<link href="css/jquery.simple-dtpicker.css" type="text/css" rel="stylesheet" />
	<link rel="stylesheet" type="text/css" href="css/bootstrap.css">
	<link rel="stylesheet" type="text/css" href="css/dashboard.css">
	
	<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js"></script>
	<script type="text/javascript" src="js/jquery.simple-dtpicker.js"></script>
</head>
<body>
<div class="navbar navbar-inverse navbar-fixed-top" role="navigation">
    <div class="container">
        <div class="navbar-header">
            <span class="navbar-brand">31337 Investments</span>
        </div>
        <div class="navbar-collapse collapse">
            <ul class="nav navbar-nav navbar-left">
                <li class="active"><a href="/">Home</a></li>
			</ul>
			<span class="navbar-form navbar-right">
                <button class="btn btn-success" onclick="alert('Уважаемые клиенты!\nСообщаем, что вывод средств временно не работает.')">Вывести</button>
            </span>
            <span class="navbar-form navbar-right">
                <button class="btn btn-success" onclick="alert('Для пополнения баланса отправьте сумму на bitcoin address 15UYRmpTtZCsYufi4SpuVJbGkusygdvls2')">Пополнить</button>
            </span>
        </div>
    </div>
</div>
<div class="container-fluid">
	<div class="row">
		<div class="col-lg-12 col-md-12 col-sm-6 well">
			<h4>Ваш баланс: <?php echo $balance; ?> руб</h4>
		</div>
    </div>

	<div class="row">
	<div class="panel panel-default">
		<div class="panel-heading">
			<form method="POST">
				<h4>История счета за период:</h4>
				<input type="text" id="range_from" name="from">
				<input type="text" id="range_to" name="to">
				<script type="text/javascript">
					$(function(){
						$('#range_from').appendDtpicker({"closeOnSelected": true});
						$('#range_to').appendDtpicker({"closeOnSelected": true});
					});
				</script>
				<input type="submit" name="history" value="Показать">
			</form>
		</div>
		<div class="panel-body">
			<h4>Доходность счета: <?php echo $yield; ?> руб</h4>
			<table class="table table-bordered">
				<thead>
					<tr><th>№</th><th>Операция</th><th>Сумма</th><th>Процент</th><th>Дата</th></tr>
				</thead>
				<tbody>
					<?php
						function td($arr) {
							return "<td>".implode("</td><td>", $arr)."</td>";
						}
						for ($i = 0; $i < count($trans); ++$i) {
							echo "<tr class=\"success\">".
								td(array($i+1, $trans[$i]["type"], $trans[$i]["value"],
									$trans[$i]["percent"], $trans[$i]["date"])).
								"</tr>\r\n";
						}
					?>
				</tbody>
			</table>
		</div>
	</div>
	</div>
</div>
</body>
</html>