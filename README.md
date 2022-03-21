# DbUpgrade
Updates Database executing Upgrade database scripts.

The application expects a folder with all the upgrade scripts placed in a separate fodlers using [semantic versioning](https://docs.npmjs.com/about-semantic-versioning)
```
├── DbUpgradeScripts
    ├── 1.0.0.1
      ├── 01_schema_upgrade.sql
      ├── 02_data_upgrade.sql
    ├── 1.0.0.2
    ├── 1.1.0.0
    └── etc..
```
The folders will be ordering will be defined by the semantic versioning model e.g. 1.02.0.0 > 1.1.11.0.
The files in the folder will be executed ordered by name - hence the naming convention is required: 01_... 02_... etc.

Supports multiple services for database upgrade through configuration.
Sеrvice object should be configured per databse for upgrade.

### Following properties should be specified for each service object:
"Name" - the name of the service. Should be passed as an argument to the application.

"ScriptsRootPath" - The path to the scripts root folder.

"Module" - the module which database will be updated. An application may have multiple databases to update.

"ConnectionString" - Connection string to the database.

"CheckoutLastRepoVersion" - if the DbUpgradeScripts root folder is a git repository this flag will specify it the repository should be pulled first before initiating the upgrade.
