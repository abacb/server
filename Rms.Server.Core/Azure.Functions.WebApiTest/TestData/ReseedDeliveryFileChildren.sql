/*配信ファイル・配信ファイル型式・配信グループ・配信結果・適用結果データ削除(on delete cascadeがあるので配信ファイルだけ削除でOK)*/
delete core.DT_DELIVERY_FILE;

/*次回SID=1にする*/
DBCC CHECKIDENT ('core.DT_DELIVERY_FILE', RESEED, 0);
DBCC CHECKIDENT ('core.DT_DELIVERY_MODEL', RESEED, 0);
DBCC CHECKIDENT ('core.DT_DELIVERY_GROUP', RESEED, 0);
DBCC CHECKIDENT ('core.DT_DELIVERY_RESULT', RESEED, 0);
DBCC CHECKIDENT ('core.DT_INSTALL_RESULT', RESEED, 0);
