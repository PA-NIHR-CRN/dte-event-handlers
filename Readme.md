# DTE Event Handlers

DTE Event Handlers is a project designed to handle scheduled tasks and provide custom Cognito message handling. The project is built with ASP.NET Core and provides functionality required to efficiently manage event-driven jobs.

## Table of Contents
- [Project Description](#project-description)
- [How to Install and Run the Project](#how-to-install-and-run-the-project)
- [Usage](#usage)

## Project Description
DTE Event Handlers is a comprehensive system designed to handle specific scheduled tasks within the project ecosystem. It provides a custom Cognito message handler that allows for specific manipulation and management of Cognito-based messages. 

The scheduled jobs are designed to run at specified intervals, performing various tasks essential to the smooth functioning of the system. The custom Cognito message handler adds a layer of customization to the standard Cognito messages, allowing them to be tailored to specific project requirements.

## How to Install and Run the Project
To run the project, you will need to have the following installed on your machine:

- .NET Core 6 SDK or later

To run the project, follow these steps:
1. Clone the repository to your local machine.
2. Update `appsettings.json` with the appropriate settings.
3. Right click on the DTE Event Handlers project and select properties. When the modal pops up select run/configurations/default and set the environment variable ASPNETCORE_ENVIRONMENT to Development.
4. Start the project from your chosen IDE or from the command line with `dotnet run`.

## Usage
To use the project, you can follow these steps:

1. Set the environment variable `ASPNETCORE_ENVIRONMENT` to `Development`.
2. Start the project.
3. Interact with the project through your chosen IDE, noting that scheduled jobs will run at their designated intervals and the Cognito message handler will process messages as they arrive.
