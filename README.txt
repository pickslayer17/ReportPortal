How to run
backend:
	- install .net SDK
	- backend needs a database 
		- install SQL Express on localhost
		- add a default database named ReportPortal
		- create an admin user:
			CREATE LOGIN ReportPortalUser WITH PASSWORD = 'sgbn3w805tyh-hgb-9h345bg';
			GO
			USE ReportPortal;
			GO
			CREATE USER ReportPortalUser FOR LOGIN ReportPortalUser;
			GO
			ALTER ROLE db_owner ADD MEMBER ReportPortalUser;
			GO
		- on ReportPortal.DAL project run a migration:
			dotnet ef database update --project ReportPortal.DAL --startup-project ReportPortal
	- on RerportPortal project run dotnet "publish -c Release -o out" for IIS or don't do it if you plan run project through Visual Studio
	- for IIS set the port 5002
frontend:
	- install node.js & npm
	- if you're planning run project through Visual Studio:
		- Build -> Configuration Manager -> Check Deplot checkbox
	- if you're planning to deploy:	
		- run "npm run build" on reactfe.client project 
		- install nginx 
		- set up nginx on the output folder and run on port 100 