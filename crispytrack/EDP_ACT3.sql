-- MySQL dump 10.13  Distrib 8.0.13, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: fried_chicken
-- ------------------------------------------------------
-- Server version	8.0.13

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
 SET NAMES utf8 ;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `dept_summary`
--

DROP TABLE IF EXISTS `dept_summary`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `dept_summary` (
  `DeptID` int(11) NOT NULL,
  `StaffCount` int(11) DEFAULT '0',
  PRIMARY KEY (`DeptID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dept_summary`
--

LOCK TABLES `dept_summary` WRITE;
/*!40000 ALTER TABLE `dept_summary` DISABLE KEYS */;
INSERT INTO `dept_summary` VALUES (1,4),(2,4),(3,1),(4,1),(5,0);
/*!40000 ALTER TABLE `dept_summary` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `depts`
--

DROP TABLE IF EXISTS `depts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `depts` (
  `ID` int(11) NOT NULL,
  `Name` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `depts`
--

LOCK TABLES `depts` WRITE;
/*!40000 ALTER TABLE `depts` DISABLE KEYS */;
INSERT INTO `depts` VALUES (1,'Kitchen'),(2,'Counter'),(3,'Delivery'),(4,'Storage'),(5,'Cleaning'),(6,'Admin'),(7,'DriveThru'),(8,'Marketing'),(9,'Security'),(10,'Maintenance');
/*!40000 ALTER TABLE `depts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `logs`
--

DROP TABLE IF EXISTS `logs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `logs` (
  `ID` int(11) NOT NULL,
  `StaffID` int(11) DEFAULT NULL,
  `Action` varchar(100) DEFAULT NULL,
  `LogTime` time DEFAULT NULL,
  `OrderID` int(11) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `OrderID` (`OrderID`),
  CONSTRAINT `logs_ibfk_1` FOREIGN KEY (`OrderID`) REFERENCES `orders` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `logs`
--

LOCK TABLES `logs` WRITE;
/*!40000 ALTER TABLE `logs` DISABLE KEYS */;
INSERT INTO `logs` VALUES (1,1,'Oil Changed','08:00:00',NULL),(2,7,'Fryers On','08:15:00',NULL),(3,5,'Chicken Unloaded','09:00:00',NULL),(4,8,'Floor Mopped','10:00:00',NULL),(5,2,'Register Open','10:30:00',NULL),(6,1,'New Batch Cooked','11:00:00',NULL),(7,9,'Lunch Rush Start','12:00:00',NULL),(8,8,'Tables Cleaned','13:00:00',NULL),(9,4,'Spice Rub Prep','14:00:00',NULL),(10,5,'Inventory Check','15:00:00',NULL);
/*!40000 ALTER TABLE `logs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `menu`
--

DROP TABLE IF EXISTS `menu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `menu` (
  `ID` int(11) NOT NULL,
  `Item` varchar(50) DEFAULT NULL,
  `Price` decimal(5,2) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menu`
--

LOCK TABLES `menu` WRITE;
/*!40000 ALTER TABLE `menu` DISABLE KEYS */;
INSERT INTO `menu` VALUES (1,'2pc Combo',8.50),(2,'6pc Wings',10.00),(3,'Family Bucket',25.00),(4,'Chicken Sando',6.50),(5,'Cajun Fries',3.00),(6,'Coleslaw',2.50),(7,'Biscuits',1.50),(8,'Iced Tea',2.00),(9,'Spicy Thigh',3.50),(10,'Drumstick',3.00);
/*!40000 ALTER TABLE `menu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orders`
--

DROP TABLE IF EXISTS `orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `orders` (
  `ID` int(11) NOT NULL,
  `StaffID` int(11) DEFAULT NULL,
  `MenuID` int(11) DEFAULT NULL,
  `Status` varchar(20) DEFAULT NULL,
  `OrderDate` date DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `StaffID` (`StaffID`),
  KEY `MenuID` (`MenuID`),
  CONSTRAINT `orders_ibfk_1` FOREIGN KEY (`StaffID`) REFERENCES `staff` (`id`),
  CONSTRAINT `orders_ibfk_2` FOREIGN KEY (`MenuID`) REFERENCES `menu` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES (101,2,3,'Ready','2026-02-23'),(102,9,1,'Ready','2026-02-23'),(103,10,2,'Delivered','2026-02-23'),(104,2,4,'Ready','2026-02-23'),(105,9,10,'Ready','2026-02-23'),(106,7,3,'Ready','2026-02-23'),(107,3,5,'Delivered','2026-02-23'),(108,9,8,'Ready','2026-02-23'),(109,2,9,'Ready','2026-02-23'),(110,10,2,'Delivered','2026-02-23');
/*!40000 ALTER TABLE `orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `staff`
--

DROP TABLE IF EXISTS `staff`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `staff` (
  `ID` int(11) NOT NULL,
  `Name` varchar(50) DEFAULT NULL,
  `DeptID` int(11) DEFAULT NULL,
  `password` varchar(255) DEFAULT 'password123',
  `email` varchar(100) DEFAULT NULL,
  `status` enum('Active','Inactive') DEFAULT 'Active',
  PRIMARY KEY (`ID`),
  KEY `DeptID` (`DeptID`),
  CONSTRAINT `staff_ibfk_1` FOREIGN KEY (`DeptID`) REFERENCES `depts` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `staff`
--

LOCK TABLES `staff` WRITE;
/*!40000 ALTER TABLE `staff` DISABLE KEYS */;
INSERT INTO `staff` (`ID`, `Name`, `DeptID`, `password`, `email`, `status`) VALUES 
(1,'Allen',1,'password123','Allen@crispytrack.com','Active'),
(2,'Fred',2,'password123','Fred@crispytrack.com','Active'),
(3,'Mheea',3,'password123','Mheea@crispytrack.com','Active'),
(4,'Edgar',1,'password123','Edgar@crispytrack.com','Active'),
(5,'Airo',4,'password123','Airo@crispytrack.com','Active'),
(6,'Geoff',2,'password123','Geoff@crispytrack.com','Active'),
(7,'Peter',1,'password123','Peter@crispytrack.com','Active'),
(9,'Alejandro',2,'password123','Alejandro@crispytrack.com','Active'),
(10,'Ryan',2,'password123','Ryan@crispytrack.com','Active'),
(11,'MARK',1,'password123','MARK@crispytrack.com','Active');
/*!40000 ALTER TABLE `staff` ENABLE KEYS */;
UNLOCK TABLES;

/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `after_staff_insert` AFTER INSERT ON `staff` FOR EACH ROW BEGIN
    UPDATE dept_summary 
    SET StaffCount = StaffCount + 1
    WHERE DeptID = NEW.DeptID;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `after_staff_update` AFTER UPDATE ON `staff` FOR EACH ROW BEGIN
    -- Only update if the department actually changed
    IF OLD.DeptID <> NEW.DeptID THEN
        -- Decrease count in old department
        UPDATE dept_summary 
        SET StaffCount = StaffCount - 1
        WHERE DeptID = OLD.DeptID;
        
        -- Increase count in new department
        UPDATE dept_summary 
        SET StaffCount = StaffCount + 1
        WHERE DeptID = NEW.DeptID;
    END IF;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `after_staff_delete` AFTER DELETE ON `staff` FOR EACH ROW BEGIN
    UPDATE dept_summary 
    SET StaffCount = StaffCount - 1
    WHERE DeptID = OLD.DeptID;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Temporary view structure for view `view_kitchenscreen`
--

DROP TABLE IF EXISTS `view_kitchenscreen`;
/*!50001 DROP VIEW IF EXISTS `view_kitchenscreen`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8mb4;
/*!50001 CREATE VIEW `view_kitchenscreen` AS SELECT 
 1 AS `OrderNum`,
 1 AS `Item`,
 1 AS `Cashier`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `view_salessummary`
--

DROP TABLE IF EXISTS `view_salessummary`;
/*!50001 DROP VIEW IF EXISTS `view_salessummary`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8mb4;
/*!50001 CREATE VIEW `view_salessummary` AS SELECT 
 1 AS `Item`,
 1 AS `TotalSold`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `view_stafflocations`
--

DROP TABLE IF EXISTS `view_stafflocations`;
/*!50001 DROP VIEW IF EXISTS `view_stafflocations`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8mb4;
/*!50001 CREATE VIEW `view_stafflocations` AS SELECT 
 1 AS `Name`,
 1 AS `Area`*/;
SET character_set_client = @saved_cs_client;

--
-- Dumping events for database 'fried_chicken'
--

--
-- Dumping routines for database 'fried_chicken'
--
/*!50003 DROP FUNCTION IF EXISTS `AddTax` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `AddTax`(item_price DECIMAL(5,2)) RETURNS decimal(5,2)
    DETERMINISTIC
BEGIN
    RETURN item_price * 1.10;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `OrderReady` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `OrderReady`(OrderNum INT)
BEGIN
    UPDATE Orders SET Status = 'Ready' WHERE ID = OrderNum;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Final view structure for view `view_kitchenscreen`
--

/*!50001 DROP VIEW IF EXISTS `view_kitchenscreen`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_general_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `view_kitchenscreen` AS select `o`.`ID` AS `OrderNum`,`m`.`Item` AS `Item`,`s`.`Name` AS `Cashier` from ((`orders` `o` join `menu` `m` on((`o`.`MenuID` = `m`.`ID`))) join `staff` `s` on((`o`.`StaffID` = `s`.`ID`))) where (`o`.`Status` = 'Cooking') */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `view_salessummary`
--

/*!50001 DROP VIEW IF EXISTS `view_salessummary`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_general_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `view_salessummary` AS select `m`.`Item` AS `Item`,count(`o`.`ID`) AS `TotalSold` from (`menu` `m` left join `orders` `o` on((`m`.`ID` = `o`.`MenuID`))) group by `m`.`Item` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `view_stafflocations`
--

/*!50001 DROP VIEW IF EXISTS `view_stafflocations`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_general_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `view_stafflocations` AS select `s`.`Name` AS `Name`,`d`.`Name` AS `Area` from (`staff` `s` join `depts` `d` on((`s`.`DeptID` = `d`.`ID`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-03-09  1:10:06