/*
 Navicat Premium Data Transfer

 Source Server         : localhost
 Source Server Type    : MySQL
 Source Server Version : 50732 (5.7.32-log)
 Source Host           : localhost:3306
 Source Schema         : game

 Target Server Type    : MySQL
 Target Server Version : 50732 (5.7.32-log)
 File Encoding         : 65001

 Date: 29/12/2024 20:48:10
*/

SET NAMES utf8mb4;
SET
FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for account
-- ----------------------------
DROP TABLE IF EXISTS `account`;
CREATE TABLE `account`
(
    `userId`    varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `openId`    varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `token`     varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `loginTime` datetime NULL DEFAULT NULL,
    PRIMARY KEY (`userId`) USING BTREE,
    UNIQUE INDEX `idx_open_id`(`openId`) USING BTREE,
    UNIQUE INDEX `idx_token`(`token`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for gamedata
-- ----------------------------
DROP TABLE IF EXISTS `gamedata`;
CREATE TABLE `gamedata`
(
    `userId`    varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `data`   blob NULL,
    PRIMARY KEY (`userId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

SET
FOREIGN_KEY_CHECKS = 1;
