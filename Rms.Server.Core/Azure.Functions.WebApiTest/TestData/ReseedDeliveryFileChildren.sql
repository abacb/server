/*�z�M�t�@�C���E�z�M�t�@�C���^���E�z�M�O���[�v�E�z�M���ʁE�K�p���ʃf�[�^�폜(on delete cascade������̂Ŕz�M�t�@�C�������폜��OK)*/
delete core.DT_DELIVERY_FILE;

/*����SID=1�ɂ���*/
DBCC CHECKIDENT ('core.DT_DELIVERY_FILE', RESEED, 0);
DBCC CHECKIDENT ('core.DT_DELIVERY_MODEL', RESEED, 0);
DBCC CHECKIDENT ('core.DT_DELIVERY_GROUP', RESEED, 0);
DBCC CHECKIDENT ('core.DT_DELIVERY_RESULT', RESEED, 0);
DBCC CHECKIDENT ('core.DT_INSTALL_RESULT', RESEED, 0);