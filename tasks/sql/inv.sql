--
-- database: `sqli`
--

-- --------------------------------------------------------

CREATE TABLE IF NOT EXISTS `hamsters` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user` varchar(256) NOT NULL,
  `pass` varchar(256) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `login` (`user`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;

INSERT INTO `hamsters` (`user`, `pass`) VALUES
('admin', MD5('NJ%^&@*JFA##$JLL)))')),
('u5296', MD5('LKJSDKT@}}}}SJDHBS$%%!!!(('));

-- --------------------------------------------------------

CREATE TABLE IF NOT EXISTS `transact` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user` varchar(256) NOT NULL,
  `type` varchar(256) NOT NULL,
  `value` int(10) unsigned NOT NULL,
  `percent` float NOT NULL,
  `date` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;

INSERT INTO `transact` (`user`, `type`, `value`, `percent`, `date`) VALUES
('u5296', 'Payment', 1337, 0, '2015-03-12 16:22'),
('u5296', 'Profit', 40, 3, '2015-03-19 22:00'),
('u5296', 'Profit', 68, 5, '2015-03-26 22:00'),
('u5296', 'Profit', 57, 4, '2015-04-03 22:00'),
('u5296', 'Profit', 105, 7, '2015-04-10 22:00'),
('u5296', 'Profit', 32, 2, '2015-04-17 22:00'),
('u5296', 'Profit', 147, 9, '2015-04-24 22:00');
