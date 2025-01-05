-- phpMyAdmin SQL Dump
-- version 5.1.1
-- https://www.phpmyadmin.net/
--
-- Hôte : localhost
-- Généré le : ven. 03 jan. 2025 à 15:09
-- Version du serveur : 5.7.33
-- Version de PHP : 7.4.19

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de données : `unisphere`
--

-- --------------------------------------------------------

--
-- Structure de la table `cap_activity_log`
--

CREATE TABLE `cap_activity_log` (
  `id` int(11) NOT NULL,
  `activity_type` varchar(50) NOT NULL,
  `details` text NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `cap_like`
--

CREATE TABLE `cap_like` (
  `id_post` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `post_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Déclencheurs `cap_like`
--
DELIMITER $$
CREATE TRIGGER `after_like_insert` AFTER INSERT ON `cap_like` FOR EACH ROW BEGIN
    INSERT INTO cap_activity_log (activity_type, details)
    VALUES ('Like Added', CONCAT('Post ID: ', NEW.post_id, ', User ID: ', NEW.user_id));
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `before_like_insert` BEFORE INSERT ON `cap_like` FOR EACH ROW BEGIN
    DECLARE like_exists INT;
    SELECT COUNT(*) INTO like_exists
    FROM cap_like
    WHERE user_id = NEW.user_id AND post_id = NEW.post_id;

    IF like_exists > 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'User cannot like the same post multiple times';
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Structure de la table `cap_post`
--

CREATE TABLE `cap_post` (
  `id_post` int(11) NOT NULL,
  `contenu` text NOT NULL,
  `post_id` int(11) DEFAULT NULL,
  `user_id` int(11) NOT NULL,
  `CreatedAt` timestamp NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Déclencheurs `cap_post`
--
DELIMITER $$
CREATE TRIGGER `after_post_delete` AFTER DELETE ON `cap_post` FOR EACH ROW BEGIN
    DELETE FROM cap_like WHERE post_id = OLD.id_post;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `after_post_insert` AFTER INSERT ON `cap_post` FOR EACH ROW BEGIN
    INSERT INTO cap_activity_log (activity_type, details)
    VALUES ('Post Created', CONCAT('Post ID: ', NEW.id_post, ', User ID: ', NEW.user_id));
END
$$
DELIMITER ;

--
-- Index pour les tables déchargées
--

--
-- Index pour la table `cap_activity_log`
--
ALTER TABLE `cap_activity_log`
  ADD PRIMARY KEY (`id`);

--
-- Index pour la table `cap_like`
--
ALTER TABLE `cap_like`
  ADD PRIMARY KEY (`id_post`),
  ADD KEY `user_id` (`user_id`),
  ADD KEY `post_id` (`post_id`);

--
-- Index pour la table `cap_post`
--
ALTER TABLE `cap_post`
  ADD PRIMARY KEY (`id_post`),
  ADD KEY `post_id` (`post_id`),
  ADD KEY `user_id` (`user_id`);

--
-- AUTO_INCREMENT pour les tables déchargées
--

--
-- AUTO_INCREMENT pour la table `cap_activity_log`
--
ALTER TABLE `cap_activity_log`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `cap_like`
--
ALTER TABLE `cap_like`
  MODIFY `id_post` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `cap_post`
--
ALTER TABLE `cap_post`
  MODIFY `id_post` int(11) NOT NULL AUTO_INCREMENT;

--
-- Contraintes pour les tables déchargées
--

--
-- Contraintes pour la table `cap_like`
--
ALTER TABLE `cap_like`
  ADD CONSTRAINT `cap_like_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `user` (`id_user`) ON DELETE CASCADE,
  ADD CONSTRAINT `cap_like_ibfk_2` FOREIGN KEY (`post_id`) REFERENCES `cap_post` (`id_post`) ON DELETE CASCADE;

--
-- Contraintes pour la table `cap_post`
--
ALTER TABLE `cap_post`
  ADD CONSTRAINT `cap_post_ibfk_1` FOREIGN KEY (`post_id`) REFERENCES `cap_post` (`id_post`) ON DELETE CASCADE,
  ADD CONSTRAINT `cap_post_ibfk_2` FOREIGN KEY (`user_id`) REFERENCES `user` (`id_user`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
