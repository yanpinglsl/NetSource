-- ----------------------------
-- Table structure for tb_pay_log
-- ----------------------------
DROP TABLE IF EXISTS `tb_pay_log`;
CREATE TABLE `tb_pay_log`  (
  `order_id` bigint(20) NOT NULL COMMENT '订单号',
  `total_fee` bigint(20) NULL DEFAULT NULL COMMENT '支付金额（分）',
  `user_id` bigint(20) NULL DEFAULT NULL COMMENT '用户ID',
  `transaction_id` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '微信交易号码',
  `status` tinyint(1) NULL DEFAULT NULL COMMENT '交易状态，1 未支付, 2已支付, 3 已退款, 4 支付错误, 5 已关闭',
  `pay_type` tinyint(1) NULL DEFAULT NULL COMMENT '支付方式，1 微信支付, 2 货到付款',
  `bank_type` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '银行类型',
  `create_time` datetime(0) NULL DEFAULT NULL COMMENT '创建时间',
  `pay_time` datetime(0) NULL DEFAULT NULL COMMENT '支付时间',
  `closed_time` datetime(0) NULL DEFAULT NULL COMMENT '关闭时间',
  `refund_time` datetime(0) NULL DEFAULT NULL COMMENT '退款时间',
  PRIMARY KEY (`order_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of tb_pay_log
-- ----------------------------
INSERT INTO `tb_pay_log` VALUES (1130461223852822528, 279900, 1, '4200000326201905200000048675', 1, 1, 'CFT', '2019-05-20 13:13:50', '2019-05-20 13:16:02', NULL, NULL);
INSERT INTO `tb_pay_log` VALUES (1130463824283553792, 499800, 1, NULL, 0, 1, NULL, '2019-05-20 13:22:34', NULL, NULL, NULL);
