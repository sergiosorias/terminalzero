ALTER TABLE [data].[DeliveryNoteItem]
	ADD CONSTRAINT [PK_DeliveryNoteItem]
	PRIMARY KEY ([Code], [TerminalCode], [DeliveryNoteHeaderCode])