/*配信グループ・配信結果・適用結果データ削除(on delete cascadeがあるので配信グループだけ削除でOK)*/
delete core.DT_DELIVERY_GROUP;

/*次回SID=1にする*/
DBCC CHECKIDENT ('core.DT_DELIVERY_GROUP', RESEED, 0);
DBCC CHECKIDENT ('core.DT_DELIVERY_RESULT', RESEED, 0);
DBCC CHECKIDENT ('core.DT_INSTALL_RESULT', RESEED, 0);
