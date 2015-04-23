https://www.digitalocean.com/community/tutorials/how-to-install-linux-nginx-mysql-php-lemp-stack-on-ubuntu-14-04

apt-get update
apt-get install nginx
apt-get install mysql-server
/usr/bin/mysql_secure_installation
	New password: jvpsKJ__q2wm
	Remove anonymous users? [Y/n] y
	Disallow root login remotely? [Y/n] y
	Remove test database and access to it? [Y/n] y

apt-get install php5-fpm php5-mysql
nano /etc/php5/fpm/php.ini
	cgi.fix_pathinfo=0
service php5-fpm restart

nano /etc/nginx/sites-available/default
mysql --user=root --password=jvpsKJ__q2wm
	CREATE USER 'ructf' IDENTIFIED BY 'ctj2710__LLKSAET';
	CREATE DATABASE sqli;
	insert...
	GRANT SELECT ON sqli.* TO ructf;
	exit	
	