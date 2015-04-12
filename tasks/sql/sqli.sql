-- phpMyAdmin SQL Dump
-- version 4.0.9
-- http://www.phpmyadmin.net
--
-- Хост: localhost
-- Время создания: Апр 11 2015 г., 22:27
-- Версия сервера: 5.5.34
-- Версия PHP: 5.3.27

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
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user` varchar(256) NOT NULL,
  `pass` varchar(256) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `login` (`user`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=2 ;

--
-- Дамп данных таблицы `hamsters`
--

INSERT INTO `hamsters` (`id`, `user`, `pass`) VALUES
(1, 'admin', '8b55613d6611575b6bfe705e54d2cf2b');

-- --------------------------------------------------------

--
-- Структура таблицы `transactions`
--

CREATE TABLE IF NOT EXISTS `transactions` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user` varchar(256) NOT NULL,
  `value` int(10) unsigned NOT NULL,
  `percent` float NOT NULL,
  `date` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=5 ;

--
-- Дамп данных таблицы `transactions`
--

INSERT INTO `transactions` (`id`, `user`, `value`, `percent`, `date`) VALUES
(1, 'admin', 1337, 0, '2015-04-12 02:47:58'),
(3, 'admin', 2, 0, '2015-04-12 03:26:31'),
(4, 'admin', 0, 0, '2015-04-12 03:26:41');

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
