-- phpMyAdmin SQL Dump
-- version 4.3.13
-- http://www.phpmyadmin.net
--
-- Хост: localhost
-- Время создания: Апр 13 2015 г., 17:54
-- Версия сервера: 5.6.23
-- Версия PHP: 5.4.39

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- База данных: `sqli`
--

-- --------------------------------------------------------

--
-- Структура таблицы `hamsters`
--

CREATE TABLE IF NOT EXISTS `hamsters` (
  `id` int(11) NOT NULL,
  `user` varchar(256) NOT NULL,
  `pass` varchar(256) NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;

--
-- Дамп данных таблицы `hamsters`
--

INSERT INTO `hamsters` (`id`, `user`, `pass`) VALUES
(1, 'admin', '8b55613d6611575b6bfe705e54d2cf2b'),
(2, 'u5296', '68c5787297cba0baf5e96f1eeb6cb1a7');

-- --------------------------------------------------------

--
-- Структура таблицы `transact`
--

CREATE TABLE IF NOT EXISTS `transact` (
  `id` int(11) NOT NULL,
  `user` varchar(256) NOT NULL,
  `type` varchar(256) NOT NULL,
  `value` int(10) unsigned NOT NULL,
  `percent` float NOT NULL,
  `date` datetime NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;

--
-- Дамп данных таблицы `transact`
--

INSERT INTO `transact` (`id`, `user`, `type`, `value`, `percent`, `date`) VALUES
(1, 'u5296', 'Пополнение', 1337, 0, '2015-04-13 16:22:43'),
(2, 'u5296', 'Прибыль', 40, 3, '2015-04-13 16:23:17'),
(3, 'u5296', 'Прибыль', 68, 5, '2015-04-13 16:25:28');

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `hamsters`
--
ALTER TABLE `hamsters`
  ADD PRIMARY KEY (`id`), ADD UNIQUE KEY `login` (`user`);

--
-- Индексы таблицы `transact`
--
ALTER TABLE `transact`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `hamsters`
--
ALTER TABLE `hamsters`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=3;
--
-- AUTO_INCREMENT для таблицы `transact`
--
ALTER TABLE `transact`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=4;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
