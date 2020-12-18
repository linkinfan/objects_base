-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: localhost    Database: work
-- ------------------------------------------------------
-- Server version	8.0.14

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `groupw`
--

DROP TABLE IF EXISTS `groupw`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `groupw` (
  `idgroup` int(11) NOT NULL AUTO_INCREMENT,
  `nameGroup` varchar(45) NOT NULL,
  PRIMARY KEY (`idgroup`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `groupw`
--

LOCK TABLES `groupw` WRITE;
/*!40000 ALTER TABLE `groupw` DISABLE KEYS */;
INSERT INTO `groupw` VALUES (1,'Admin'),(2,'ПТО'),(3,'Бухгалтерия'),(4,'Гараж'),(5,'КИПиА');
/*!40000 ALTER TABLE `groupw` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `regdata`
--

DROP TABLE IF EXISTS `regdata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regdata` (
  `idregdata` int(11) NOT NULL AUTO_INCREMENT,
  `firstname` varchar(45) NOT NULL,
  `surname` varchar(45) NOT NULL,
  `patronymic` varchar(45) NOT NULL,
  `login` int(11) NOT NULL,
  `pass` varchar(45) NOT NULL,
  `pol` varchar(1) DEFAULT NULL,
  `groupReg` int(11) NOT NULL,
  `status` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`idregdata`),
  UNIQUE KEY `login_UNIQUE` (`login`),
  KEY `groupreg_idx` (`groupReg`),
  CONSTRAINT `groupreg` FOREIGN KEY (`groupReg`) REFERENCES `groupw` (`idgroup`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `regdata`
--

LOCK TABLES `regdata` WRITE;
/*!40000 ALTER TABLE `regdata` DISABLE KEYS */;
INSERT INTO `regdata` VALUES (17,'Шамшиев','Владимир','Федорович',2222,'2222','М',1,0),(18,'Васильев','Евгений','Иванович',1111,'1111','М',2,0),(19,'Зонова','Ульяна','Александровна',3333,'3333','Ж',3,0),(20,'Петров','Владимир','Иванович',2747,'123','М',2,0);
/*!40000 ALTER TABLE `regdata` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sessions`
--

DROP TABLE IF EXISTS `sessions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sessions` (
  `idsessions` int(11) NOT NULL AUTO_INCREMENT,
  `idtasks` int(11) DEFAULT NULL,
  `datetimeStart` datetime DEFAULT NULL,
  `datetimeEnd` datetime DEFAULT NULL,
  `sessionsTime` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL,
  `isFinished` tinyint(1) DEFAULT '0',
  `iduser` int(11) NOT NULL,
  PRIMARY KEY (`idsessions`),
  KEY `idtaskskey_idx` (`idtasks`),
  KEY `iduser_idx` (`iduser`),
  CONSTRAINT `idtaskskey` FOREIGN KEY (`idtasks`) REFERENCES `tasks` (`idtasks`) ON DELETE SET NULL ON UPDATE SET NULL,
  CONSTRAINT `iduser` FOREIGN KEY (`iduser`) REFERENCES `regdata` (`idregdata`)
) ENGINE=InnoDB AUTO_INCREMENT=43 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sessions`
--

LOCK TABLES `sessions` WRITE;
/*!40000 ALTER TABLE `sessions` DISABLE KEYS */;
INSERT INTO `sessions` VALUES (40,NULL,'2019-02-05 07:26:10',NULL,'99999999',0,19),(41,NULL,'2019-02-05 08:58:38','2019-02-05 08:58:48','99999999',1,19),(42,NULL,'2019-02-06 04:38:50','2019-02-06 04:38:59','8',1,19);
/*!40000 ALTER TABLE `sessions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tasks`
--

DROP TABLE IF EXISTS `tasks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tasks` (
  `idtasks` int(11) NOT NULL AUTO_INCREMENT,
  `nameTask` varchar(200) NOT NULL,
  `note` varchar(300) NOT NULL,
  `whensend` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `whenstart` date NOT NULL,
  `whenstop` date NOT NULL,
  `archive` varchar(100) DEFAULT NULL,
  `whodo` varchar(100) DEFAULT NULL,
  `whodid` varchar(100) DEFAULT NULL,
  `whosend` int(11) NOT NULL,
  PRIMARY KEY (`idtasks`),
  KEY `whosendkey_idx` (`whosend`),
  CONSTRAINT `whosendkey` FOREIGN KEY (`whosend`) REFERENCES `regdata` (`idregdata`)
) ENGINE=InnoDB AUTO_INCREMENT=64 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tasks`
--

LOCK TABLES `tasks` WRITE;
/*!40000 ALTER TABLE `tasks` DISABLE KEYS */;
INSERT INTO `tasks` VALUES (61,'фывыв','фывфывфывыфв','2019-02-06 17:58:58','2019-02-06','2019-02-07','17|20|','17|20|',NULL,17);
/*!40000 ALTER TABLE `tasks` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-02-06 23:12:33
