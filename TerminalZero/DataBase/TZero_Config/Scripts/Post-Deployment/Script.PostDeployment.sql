/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


:r .\InitData\status\Type.sql

:r .\InitData\dbo\Module.sql
:r .\InitData\dbo\SystemProperty.sql
:r .\InitData\dbo\ConnectionState.sql
:r .\InitData\dbo\PackStatus.sql

:r .\InitData\data\Tax.sql
:r .\InitData\data\TaxPosition.sql
:r .\InitData\data\StockType.sql
:r .\InitData\data\PaymentInstrument.sql
:r .\InitData\data\SaleType.sql
