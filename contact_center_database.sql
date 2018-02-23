/*
Navicat MySQL Data Transfer

Source Server         : admin
Source Server Version : 50523
Source Host           : localhost:3306
Source Database       : contact_center_database

Target Server Type    : MYSQL
Target Server Version : 50523
File Encoding         : 65001

Date: 2018-02-24 00:32:34
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for `file_system_change`
-- ----------------------------
DROP TABLE IF EXISTS `file_system_change`;
CREATE TABLE `file_system_change` (
  `id_file_system` int(11) NOT NULL AUTO_INCREMENT,
  `way_json_file` char(255) NOT NULL,
  `last_update` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_file_system`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of file_system_change
-- ----------------------------

-- ----------------------------
-- Table structure for `file_systems`
-- ----------------------------
DROP TABLE IF EXISTS `file_systems`;
CREATE TABLE `file_systems` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` char(63) NOT NULL,
  `relative_way` char(255) NOT NULL,
  `policy` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of file_systems
-- ----------------------------

-- ----------------------------
-- Table structure for `policy_file_system`
-- ----------------------------
DROP TABLE IF EXISTS `policy_file_system`;
CREATE TABLE `policy_file_system` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `describe` char(63) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of policy_file_system
-- ----------------------------

-- ----------------------------
-- Table structure for `query`
-- ----------------------------
DROP TABLE IF EXISTS `query`;
CREATE TABLE `query` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `type_query` int(11) NOT NULL,
  `date` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE CURRENT_TIMESTAMP,
  `command` char(63) NOT NULL,
  `additional_information` char(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of query
-- ----------------------------

-- ----------------------------
-- Table structure for `rights`
-- ----------------------------
DROP TABLE IF EXISTS `rights`;
CREATE TABLE `rights` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `describe` char(127) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of rights
-- ----------------------------

-- ----------------------------
-- Table structure for `type_query`
-- ----------------------------
DROP TABLE IF EXISTS `type_query`;
CREATE TABLE `type_query` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `describe` char(63) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of type_query
-- ----------------------------

-- ----------------------------
-- Table structure for `users`
-- ----------------------------
DROP TABLE IF EXISTS `users`;
CREATE TABLE `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `login` char(20) NOT NULL,
  `password` char(20) NOT NULL,
  `right` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of users
-- ----------------------------
